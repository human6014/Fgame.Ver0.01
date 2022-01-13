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
        //PhotonNetwork.IsMessageQueueRunning = true;
        PhotonNetwork.GameVersion = "1.0";
        name = GameObject.Find("LobbyManager").GetComponent<LobbyManager>().inGameName; //Build and Run에서 정상 작동
        PhotonNetwork.NickName = name; //미완성
        PhotonNetwork.ConnectUsingSettings();
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
        view.RPC(nameof(PunUpdate), RpcTarget.All);
        GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.childSpawner[0, cPlayerCount - 1].position + Vector3.up, Quaternion.identity);
        allTileMap.SetPlayer(PhotonNetwork.NickName);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        view.RPC(nameof(PunUpdate), RpcTarget.All);
        Debug.Log("나감");
        allTileMap.setPlayer();
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
