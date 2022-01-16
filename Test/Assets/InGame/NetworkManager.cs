using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class NetworkManager : MonoBehaviourPunCallbacks//,IPunObservable
{
    private int cMaxPlayer;
    public int cPlayerCount;
    private bool start;
    private int count;
    private int stateIndex;
    private string roomCode = string.Empty;
    private string playerName = string.Empty;
    public bool isFull = false;
    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text timerToStartDisplay;
    [SerializeField] Button matchDown;
    [SerializeField] PhotonView view;
    [SerializeField] AllTileMap allTileMap;
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
    LobbyManager lobbyManager;
    
    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;
    private void Start()
    {
        //PhotonNetwork.IsMessageQueueRunning = true;
        lobbyManager = FindObjectsOfType<LobbyManager>()[0];
        stateIndex = lobbyManager.stateIndex;
        playerName = lobbyManager.playerName; //Build and Run에서 정상 작동
        roomCode = lobbyManager.roomCode;
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.NickName = playerName; //미완성
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("NetworkManagerStart");
    }
    public void CreateRoom()
    {
        Debug.Log("CreateRoom");
        //PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 2 });
    }
    public override void OnConnectedToMaster()
    {
        if (stateIndex == 0) PhotonNetwork.JoinRandomRoom();
        else if (stateIndex == 1) if(roomCode != "Default" && roomCode !="") PhotonNetwork.CreateRoom(roomCode, roomOptions);
        else if (stateIndex == 2) if (roomCode != "Default" && roomCode != "") PhotonNetwork.JoinRoom(roomCode);
        else
        {
            Debug.LogError("서버 입장 불가");
        }
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
        GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.childSpawner[0, cPlayerCount - 1].position + Vector3.up, Quaternion.identity);
        allTileMap.PlusPlayer(PhotonNetwork.NickName);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        view.RPC(nameof(PunUpdate), RpcTarget.All);
        //allTileMap.MinusPlayer();
        Debug.Log("OnPlayerLeftRoom");
    }
    public void DisconnectPlayer() => StartCoroutine(nameof(DisconnectNetwork));
    IEnumerator DisconnectNetwork()
    {
        PhotonNetwork.Disconnect();
        yield return new WaitUntil(() => PhotonNetwork.IsConnected == false);
        SceneManager.LoadScene(0);
    }
    [PunRPC]
    void PunUpdate()
    {
        cPlayerCount = PhotonNetwork.PlayerList.Length;
        roomCountDisplay.text = cPlayerCount + " / " + cMaxPlayer;
        
        if (cPlayerCount == cMaxPlayer)
        {
            //roomOptions.IsOpen = false;
            
            isFull = true;
        }
        
    }
}
