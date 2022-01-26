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

public class GeneralManager : MonoBehaviourPunCallbacks,IPunObservable, IChatClientListener
{
    private int stateIndex;
    private string roomCode = string.Empty;
    private string playerName = string.Empty;

    private bool isRoomFull = false;
    private bool isCreateTile = false;

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
    public void SetIsCreateTile(bool _isCreateTile) => isCreateTile = _isCreateTile;
    private void Start()
    {
        view = photonView;
        stateIndex = GameManager.Instance().GetStateIndex();
        roomCode = GameManager.Instance().GetRoomCode();
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance().GetPlayerName();
        PhotonNetwork.LocalPlayer.SetPlayerNumber(PhotonNetwork.PlayerList.Length);

        chatClient = new ChatClient(this);
        channelName = "test";
        chatClient.Connect("1225a90f-4d35-49e8-9a03-40d8f1a0f2ec", "1.0", new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName));
        AddLine(string.Format("연결", PhotonNetwork.LocalPlayer.NickName));
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
            for(int i = 0; i < 6; i++) stream.SendNext(allTileMap.GetPersonTileRadius(i));
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                allTileMap.SetPersonTileRadius(i, (float)stream.ReceiveNext());
            }
        }
    }
    #endregion

    #region 채팅
    void Update()
    {
        chatClient.Service();
    }
    void AddLine(string lineString)
    {
        outputText.text += lineString + "\r\n";
    }
    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        if (level == DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
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

    void IChatClientListener.OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange = " + state);
    }

    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
        }
    }

    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage : " + message);
    }

    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
        AddLine(string.Format("채널 입장 ({0})", string.Join(",", channels)));
    }

    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));
    }

    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
    }

    void IChatClientListener.OnUserSubscribed(string channel, string user) { }
    void IChatClientListener.OnUserUnsubscribed(string channel, string user) { }
    #endregion
}