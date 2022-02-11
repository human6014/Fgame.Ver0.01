using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class GeneralManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private int stateIndex;
    private string roomCode = string.Empty;
    private string playerName = string.Empty;

    private bool isRoomFull = false;
    private bool isCreateTile = false;
    private bool isCreatePlayer = false;


    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Button matchDown;
    [SerializeField] InputField inputField;
    [SerializeField] Text outputText;
    [SerializeField] GameObject content;
    [SerializeField] AllTileMap allTileMap;
    PhotonView view;
    public bool GetIsRoomFull() => isRoomFull;
    public bool GetIsCreateTile() => isCreateTile;
    public bool GetIsCreatePlayer() => isCreatePlayer;
    public void SetIsCreateTile(bool _isCreateTile) => isCreateTile = _isCreateTile;
    public void SetIsCreatePlayer(bool _isCreatePlayer) => isCreatePlayer = _isCreatePlayer;
    private void Start()
    {
        view = photonView;
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance().GetPlayerName();
        PhotonNetwork.LocalPlayer.SetPlayerNumber(PhotonNetwork.PlayerList.Length);
        stateIndex = GameManager.Instance().GetStateIndex();
        roomCode = GameManager.Instance().GetRoomCode();
        playerName = GameManager.Instance().GetPlayerName();

        view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
        OnMasterChatting("연결 성공");
        view.RPC(nameof(OnMasterChatting), RpcTarget.Others, " 님이 입장하였습니다", playerName);
    }
    public void CreatePlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.GetSpawner(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1).position + Vector3.up, Quaternion.identity);
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //OnMasterChatting("님이 입장하였습니다", newPlayer.NickName);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
        OnMasterChatting(" 님이 퇴장하였습니다", otherPlayer.NickName);
    }
    [PunRPC]
    void PunUpdate()
    {
        roomCountDisplay.text = PhotonNetwork.PlayerList.Length + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
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
        {
            for (int i = 0; i < 6; i++) stream.SendNext(allTileMap.GetPersonTileRadius(i));
        }
        else
        {
            for (int i = 0; i < 6; i++) allTileMap.SetPersonTileRadius(i, (float)stream.ReceiveNext());
        }
    }
    #endregion
    #region 채팅
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(inputField.text)) Input_OnEndEdit();
            inputField.Select();
        }
    }
    public void SetChatClear() => inputField.text = string.Empty;
    [PunRPC]
    void OnPlayerChatting(string message,string sender) => outputText.text += sender + " : " + message + "\r\n";
    [PunRPC]
    void OnMasterChatting(string message, string target = "") => outputText.text += target + message + "\r\n";
    public void Input_OnEndEdit()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (inputField.text.Length > 50) outputText.text += "최대 50글자까지 입력 가능합니다.\r\n";
            else view.RPC(nameof(OnPlayerChatting), RpcTarget.All, inputField.text, PhotonNetwork.LocalPlayer.NickName);
            inputField.text = "";
        }
    }
    #endregion
}