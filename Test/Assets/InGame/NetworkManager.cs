using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks//,IPunObservable
{
    private int cMaxPlayer;
    private int cPlayerCount;
    private bool start;
    private int count;
    public new string name = string.Empty;
    public bool isFull = false;
    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text timerToStartDisplay;
    [SerializeField] Button matchDown;
    [SerializeField] PhotonView view;
    [SerializeField] AllTileMap allTileMap;
    RoomOptions roomOptions;
    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;
    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        PhotonNetwork.GameVersion = "1.0";
        //name = GameObject.Find("LobbyManager").GetComponent<LobbyManager>().inGameName; //Build and Run에서 정상 작동
        PhotonNetwork.NickName = name; //미완성
        PhotonNetwork.ConnectUsingSettings();
        view = PhotonView.Get(this); //뭘까 이건
        Debug.Log("NetworkManagerStart");
    }
    public override void OnConnectedToMaster() => PhotonNetwork.JoinRandomRoom();
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        roomOptions = new RoomOptions{MaxPlayers = 2};
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom run");
        cMaxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        view.RPC("PunUpdate", RpcTarget.All);
        GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.childSpawner[0, cPlayerCount - 1].position + Vector3.up, Quaternion.identity);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        view.RPC("PunUpdate", RpcTarget.All);
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
