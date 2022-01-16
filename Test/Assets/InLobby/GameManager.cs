using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class GameManager : MonoBehaviourPunCallbacks
{
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

    private int cMaxPlayer;
    private int cPlayerCount;
    public string roomCode = string.Empty;
    public string playerName = string.Empty;
    public int stateIndex = -1;
    public bool isFull = false;
    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text timerToStartDisplay;
    PhotonView view;
    AllTileMap allTileMap;
    GameObject AllTileMap;
    GameObject[] list;
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 1 };

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
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("해당 방 없음");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom run");
        cMaxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        view.RPC(nameof(PunUpdate), RpcTarget.All);

    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        view.RPC(nameof(PunUpdate), RpcTarget.All);
        SceneManager.LoadScene(0);
        //allTileMap.MinusPlayer();
        Debug.Log("OnPlayerLeftRoom");
    }
    [PunRPC]
    void PunUpdate()
    {
        cPlayerCount = PhotonNetwork.PlayerList.Length;
        //roomCountDisplay.text = cPlayerCount + " / " + cMaxPlayer;

        if (cPlayerCount == cMaxPlayer)
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
        SceneManager.LoadScene(1);
    }
    public void SetAllTileMap(GameObject obj)
    {
        allTileMap = obj.GetComponent<AllTileMap>();
        PhotonNetwork.Instantiate("Player", allTileMap.childSpawner[0, cPlayerCount - 1].position + Vector3.up, Quaternion.identity);
        allTileMap.PlusPlayer(PhotonNetwork.NickName);
    }
}
