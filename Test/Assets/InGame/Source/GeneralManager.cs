using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class GeneralManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private int stateIndex;
    private int remainPlayerCount;
    private int myPlayerIndex;
    private int winnerPlayerIndex = -1;
    private int[] weapon = new int[3] { 0, 3, 6 };

    private string roomCode = string.Empty;
    private string playerName = string.Empty;
    private string[] playersName = new string[6];

    private bool isRoomFull = false;
    private bool isCreateTile = false;
    private bool isCreatePlayer = false;
    private bool isChatOn = false;
    private bool isGameEnd = false;
    private bool isClosedEnd = false;

    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text outputText;
    [SerializeField] Image image;
    [SerializeField] Button matchDown;
    [SerializeField] InputField inputField;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] AllTileMap allTileMap;
    PhotonView view;
    public int[] GetWeapon() => weapon;
    public int GetMeleeIndex() => weapon[0];
    public bool GetIsRoomFull() => isRoomFull;
    public bool GetIsCreateTile() => isCreateTile;
    public bool GetIsCreatePlayer() => isCreatePlayer;
    public bool GetIsChatOn() => isChatOn;
    public bool GetIsGameEnd() => isGameEnd;
    public bool GetIsCloseEnd() => isClosedEnd;
    public int GetWinnerPlayerIndex() => winnerPlayerIndex;
    public string GetPlayerName(int i) => playersName[i];
    public void SetIsClosedEnd() => isClosedEnd = true;
    public void SetIsCreateTile(bool _isCreateTile) => isCreateTile = _isCreateTile;
    public void SetIsCreatePlayer(bool _isCreatePlayer) => isCreatePlayer = _isCreatePlayer;
    public void SetWeapon(int index) => weapon[index / 3] = index; //Button에서 호출함!
    public void SetRemainPlayerCount()
    {
        if (isRoomFull) view.RPC(nameof(SetPunRemainPlayerCount), RpcTarget.All);
    }
    [PunRPC]
    public void SetWinnerPlayerIndex(int _winnerPlayerIndex)
    {
        isGameEnd = true;
        winnerPlayerIndex = _winnerPlayerIndex;
    }
    [PunRPC]
    private void SetPunRemainPlayerCount() => remainPlayerCount--;

    private void Start()
    {
        view = photonView;
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance().GetPlayerName();
        PhotonNetwork.LocalPlayer.SetPlayerNumber(PhotonNetwork.PlayerList.Length);
        
        stateIndex = GameManager.Instance().GetStateIndex();
        roomCode = GameManager.Instance().GetRoomCode();
        playerName = GameManager.Instance().GetPlayerName();
        
        view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
        OnMasterChatting("입장");
        view.RPC(nameof(OnMasterChatting), RpcTarget.Others, " 님이 입장하였습니다", playerName);
    }
    public void CreatePlayer()
    {
        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == -1) return;
        if (PhotonNetwork.InRoom)
        {
            GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.GetSpawner(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1).position + Vector3.up, Quaternion.identity);
            player.name = "Player" + PhotonNetwork.LocalPlayer.GetPlayerNumber();
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //OnMasterChatting("님이 입장하였습니다", newPlayer.NickName);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if(!isRoomFull) view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
        if (isRoomFull && !allTileMap.GetIsOutPlayer(otherPlayer.GetPlayerNumber() - 1)) SetPunRemainPlayerCount();
        OnMasterChatting(" 님이 퇴장하였습니다", otherPlayer.NickName);
    }
    [PunRPC]
    private void PunUpdate()
    {
        roomCountDisplay.text = PhotonNetwork.PlayerList.Length + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            remainPlayerCount = PhotonNetwork.CurrentRoom.MaxPlayers;
            myPlayerIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            isRoomFull = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Substring(0, 5) == "Floor") Destroy(other.gameObject);
    }
    #region 블럭 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!isRoomFull) return;
        if (stream.IsWriting)
            for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++) 
                stream.SendNext(allTileMap.GetPersonTileRadius(i));
        else
            for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++) 
                allTileMap.SetPersonTileRadius(i, (float)stream.ReceiveNext());
    }
    #endregion
    #region 채팅 && 게임 끝 여부 관리
    bool tryOnce = true;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isChatOn = !isChatOn;
            inputField.interactable = isChatOn;
            inputField.ActivateInputField();
            if (!string.IsNullOrEmpty(inputField.text)) InputOnEndEdit();
            if (isChatOn) image.fillAmount = 1;
            else image.fillAmount = 0;
        }
        if (!GetIsRoomFull()) return;
        if (tryOnce)
        {
            tryOnce = false;
            int index = 0;
            foreach (Photon.Realtime.Player netPlayer in PhotonNetwork.PlayerList)
                playersName[index++] = netPlayer.NickName;
        }
        if (remainPlayerCount == 1 && !allTileMap.GetIsOutPlayer(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1) && !isGameEnd)
        {
            winnerPlayerIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            view.RPC(nameof(SetWinnerPlayerIndex), RpcTarget.All, winnerPlayerIndex);
            isGameEnd = true;
        }

    }
    public void SetChatClear() => inputField.text = string.Empty;
    [PunRPC]
    private void OnPlayerChatting(string message, string sender)
    {
        outputText.text += sender + " : " + message + "\r\n";
        rectTransform.sizeDelta = new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + 12);
    }
    [PunRPC]
    private void OnMasterChatting(string message, string target = "") => outputText.text += target + message + "\r\n";
    private void InputOnEndEdit()
    {
        if (PhotonNetwork.IsConnected)
        {
            view.RPC(nameof(OnPlayerChatting), RpcTarget.All, inputField.text, PhotonNetwork.LocalPlayer.NickName);
            inputField.text = "";
        }
    }
    #endregion
}