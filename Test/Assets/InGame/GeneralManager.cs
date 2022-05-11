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
    private string roomCode = string.Empty;
    private string playerName = string.Empty;

    private bool isRoomFull = false;
    private bool isCreateTile = false;
    private bool isCreatePlayer = false;
    private bool isChatOn = false;
    private bool isGameEnd = false;

    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text outputText;
    [SerializeField] Image image;
    [SerializeField] Button matchDown;
    [SerializeField] InputField inputField;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] AllTileMap allTileMap;
    [SerializeField] Transform players;
    PhotonView view;
    public bool GetIsRoomFull() => isRoomFull;
    public bool GetIsCreateTile() => isCreateTile;
    public bool GetIsCreatePlayer() => isCreatePlayer;
    public bool GetIsChatOn() => isChatOn;
    public bool GetIsGameEnd() => isGameEnd;
    [PunRPC]
    public int SetWinnerPlayerIndex()
    {
        return winnerPlayerIndex;
    }
    [PunRPC]
    private void SetPunRemainPlayerCount() => remainPlayerCount--;
    public void SetRemainPlayerCount()
    {
        if (isRoomFull) view.RPC(nameof(SetPunRemainPlayerCount), RpcTarget.All);
    }
    public void SetIsCreateTile(bool _isCreateTile) => isCreateTile = _isCreateTile;
    public void SetIsCreatePlayer(bool _isCreatePlayer) => isCreatePlayer = _isCreatePlayer;

    private void Start()
    {
        view = photonView;
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance().GetPlayerName();
        PhotonNetwork.LocalPlayer.SetPlayerNumber(PhotonNetwork.PlayerList.Length);
        myPlayerIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        stateIndex = GameManager.Instance().GetStateIndex();
        roomCode = GameManager.Instance().GetRoomCode();
        playerName = GameManager.Instance().GetPlayerName();

        view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
        OnMasterChatting("����");
        view.RPC(nameof(OnMasterChatting), RpcTarget.Others, " ���� �����Ͽ����ϴ�", playerName);
    }
    public void CreatePlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.GetSpawner(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1).position + Vector3.up, Quaternion.identity);
        player.name = "Player" + PhotonNetwork.LocalPlayer.GetPlayerNumber();
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //OnMasterChatting("���� �����Ͽ����ϴ�", newPlayer.NickName);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
        SetRemainPlayerCount();
        OnMasterChatting(" ���� �����Ͽ����ϴ�", otherPlayer.NickName);
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
            isRoomFull = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Substring(0, 5) == "Floor") Destroy(other.gameObject);
    }
    private void OnApplicationQuit()
    {
        Debug.Log("GeneralManager");
        SetRemainPlayerCount();
    }
    #region �� ����ȭ
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
    #region ä��
    private void Update()
    {
        //Debug.Log("remainPlayerCounts : " + remainPlayerCount);
        //Debug.Log("isGameEnd : " + isGameEnd);
        if (remainPlayerCount == 1 && allTileMap.GetIsOutPlayer(myPlayerIndex - 1))
        {
            //Debug.Log("Winner is " + winnerPlayerIndex);
            winnerPlayerIndex = myPlayerIndex;
            isGameEnd = true;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isChatOn = !isChatOn;
            inputField.interactable = isChatOn;
            inputField.ActivateInputField();
            if (!string.IsNullOrEmpty(inputField.text)) InputOnEndEdit();
            if (isChatOn) image.fillAmount = 1;
            else image.fillAmount = 0;
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