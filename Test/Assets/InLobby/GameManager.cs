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
    #region ½Ì±ÛÅæ
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
    public byte maxPlayers = 1;
    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text timerToStartDisplay;
    PhotonView view;
    AllTileMap allTileMap;
    GameObject AllTileMap;
    GameObject[] list;
    RoomOptions roomOptions;

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
        roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("ÇØ´ç ¹æ ¾øÀ½");
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


        GUILayout.Label("¼­¹ö½Ã°£ : " + PhotonNetwork.Time);
        GUILayout.Label("»óÅÂ : " + PhotonNetwork.NetworkClientState);
        GUILayout.Label("¾À : " + SceneManager.GetActiveScene().name);

        GUILayout.EndVertical();
    }
}
