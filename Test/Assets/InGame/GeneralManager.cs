using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using System.Linq;

public class GeneralManager : MonoBehaviourPunCallbacks, IPunObservable, IChatClientListener
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

    [SerializeField] AllTileMap allTileMap;
    PhotonView view;

    ChatClient chatClient;
    private string channelName;

    public bool GetIsRoomFull() => isRoomFull;
    public bool GetIsCreateTile() => isCreateTile;
    public bool GetIsCreatePlayer() => isCreatePlayer;
    public void SetIsCreateTile(bool _isCreateTile) => isCreateTile = _isCreateTile;
    public void SetIsCreatePlayer(bool _isCreatePlayer) => isCreatePlayer = _isCreatePlayer;
    private void Start()
    {
        view = photonView;
        stateIndex = GameManager.Instance().GetStateIndex();
        roomCode = GameManager.Instance().GetRoomCode();
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance().GetPlayerName();
        PhotonNetwork.LocalPlayer.SetPlayerNumber(PhotonNetwork.PlayerList.Length);

        chatClient = new ChatClient(this);
        channelName = "test";
        chatClient.Connect("99bf5d40-3f94-4f42-b896-4d0c651e9188", "1.0", new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
        view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
    }

    public void CreatePlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.GetSpawner(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1).position + Vector3.up, Quaternion.identity);
        
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //PhotonNetwork.LocalPlayer.SetPlayerNumber(playerNumber++);
        //view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //PhotonNetwork.LocalPlayer.SetPlayerNumber(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1);
        view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
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
        if (!isRoomFull || !isCreateTile) return;
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
        chatClient.Service();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(!string.IsNullOrEmpty(inputField.text)) Input_OnEndEdit();
            inputField.Select();
        }
    }
    void AddLine(string lineString) => outputText.text += lineString + "\r\n";
    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        switch (level)
        {
            case DebugLevel.ERROR: Debug.LogError(message);
                break;
            case DebugLevel.WARNING: Debug.LogWarning(message);
                break;
            default: Debug.Log(message);
                break;
        }
    }
    void IChatClientListener.OnConnected()
    {
        AddLine("서버에 연결되었습니다.");

        chatClient.Subscribe(new string[] { channelName }, 10);
    }
    void IChatClientListener.OnDisconnected()
    {
        AddLine("서버에 연결이 끊어졌습니다.");
    }
    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            AddLine(string.Format("{0}\t: {1}", senders[i], messages[i].ToString()));
        }
    }
    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
        chatClient.PublishMessage(channelName, string.Format("채널 입장 ({0})", string.Join(",", PhotonNetwork.LocalPlayer.NickName)));
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        //AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", PhotonNetwork.LocalPlayer.NickName)));
        chatClient.PublishMessage(channelName, string.Format("채널 퇴장 ({0})", string.Join(",", PhotonNetwork.LocalPlayer.NickName)));
    }
    
    public void Input_OnEndEdit()
    {
        if (chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            chatClient.PublishMessage(channelName, inputField.text);
            inputField.text = "";
        }
    }
    void IChatClientListener.OnChatStateChange(ChatState state) { }
    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName) { }
    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    void IChatClientListener.OnUserSubscribed(string channel, string user) { }
    void IChatClientListener.OnUserUnsubscribed(string channel, string user) { }
    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
        ExitChat();
    }
    public void ExitChat()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }
    #endregion
}