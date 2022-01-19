using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GameManager : MonoBehaviourPunCallbacks
{
    #region 싱글톤
    static GameManager _instance = null;
    public static GameManager Instance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this.gameObject);
    }
    #endregion
    public string roomCode = string.Empty;
    public string playerName = string.Empty;
    public int stateIndex = -1;
    public bool isFull = false;
    //public byte maxPlayers = 3;
    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text timerToStartDisplay;
    PhotonView view;
    AllTileMap allTileMap;
    GameObject AllTileMap;
    GameObject[] list;
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };

    public bool master() => PhotonNetwork.LocalPlayer.IsMasterClient;
    public void SetPos(Transform Tr, Vector3 target) //respawn
    {
        Tr.position = target;
    }
    public void SetTag(string key, object value, Photon.Realtime.Player player = null)
    {
        if (player == null) player = PhotonNetwork.LocalPlayer;
        player.SetCustomProperties(new Hashtable { { key, value } });
    }
    public bool AllhasTag(string key)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i].CustomProperties[key] == null) return false;
        return true;
    }
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        //PhotonNetwork.IsMessageQueueRunning = true;
        PhotonNetwork.GameVersion = "1.0";

        view = photonView;
        Debug.Log("NetworkManager Start");
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, roomOptions);
        //PhotonNetwork.CurrentRoom.MaxPlayers = maxPlayers;
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("해당 방 없음");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom run");
        //view.RPC(nameof(PunUpdate), RpcTarget.All);

    }
    /*
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //view.RPC(nameof(PunUpdate), RpcTarget.All);
        PhotonNetwork.LoadLevel(0);
        //allTileMap.MinusPlayer();
        Debug.Log("OnPlayerLeftRoom");
    }
    */
    [PunRPC]
    void PunUpdate()
    {
        //roomCountDisplay.text = cPlayerCount + " / " + cMaxPlayer;

        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            //roomOptions.IsOpen = false;

            isFull = true;
        }
    }
    public void ResetInfo()
    {
        stateIndex = -1;
        playerName = "";
        roomCode = "";
    }
    public void SceneMove(int stateIndex, string playerName, string roomCode)
    {
        this.stateIndex = stateIndex;
        this.playerName = playerName;
        this.roomCode = roomCode;
        PhotonNetwork.NickName = playerName;

        switch (this.stateIndex)
        {
            case 0:
                PhotonNetwork.JoinRandomRoom();
                break;
            case 1:
                PhotonNetwork.CreateRoom(roomCode, roomOptions);
                break;
            case 2:
                PhotonNetwork.JoinRoom(roomCode);
                break;
            default:
                break;
        }
        PhotonNetwork.LoadLevel(1);
    }
    /*
    public void SetAllTileMap(GameObject obj)
    {
        allTileMap = obj.GetComponent<AllTileMap>();
        PhotonNetwork.Instantiate("Player", allTileMap.childSpawner[0, PhotonNetwork.PlayerList.Length].position + Vector3.up, Quaternion.identity);
        allTileMap.PlusPlayer(PhotonNetwork.NickName);
    }
    */
    void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 20;
        GUILayout.BeginVertical("Box", GUILayout.Width(200), GUILayout.MinHeight(100));


        GUILayout.Label("서버시간 : " + PhotonNetwork.Time);
        GUILayout.Label("상태 : " + PhotonNetwork.NetworkClientState);
        GUILayout.Label("씬 : " + SceneManager.GetActiveScene().name);
        //GUILayout.Label("최대 인원 : " + PhotonNetwork.CurrentRoom.MaxPlayers);
        //GUILayout.Label("현재 인원 : " + PhotonNetwork.CurrentRoom.PlayerCount);
        GUILayout.EndVertical();
    }
}
