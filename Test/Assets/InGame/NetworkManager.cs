using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class NetworkManager : MonoBehaviourPunCallbacks,IPunObservable
{
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
    PhotonView view;
    [SerializeField] AllTileMap allTileMap;
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4 };

    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;
    private void Start()
    {
        //PhotonNetwork.IsMessageQueueRunning = true;
        stateIndex = GameManager.Instance().stateIndex;
        playerName = GameManager.Instance().playerName; //Build and Run에서 정상 작동
        roomCode = GameManager.Instance().roomCode;
        view = photonView;
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.NickName = playerName; //미완성
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("NetworkManagerStart");
    }
    public void CreateRoom()
    {
        Debug.Log("CreateRoom");
    }
    public override void OnConnectedToMaster()
    {
        if (stateIndex == 0) PhotonNetwork.JoinRandomRoom();
        else if (stateIndex == 1)
        {
            PhotonNetwork.CreateRoom(roomCode, roomOptions);
        }
        else if (stateIndex == 2)
        {
            PhotonNetwork.JoinRoom(roomCode);
            roomOptions.IsVisible = false;
        }
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
        Debug.Log("JoinRoom Failed");
        DisconnectPlayer();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom run");
        view.RPC(nameof(PunUpdate), RpcTarget.All);
        
        //allTileMap.PlusPlayer(PhotonNetwork.NickName);
    }
    public void CreatePlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.GetSpawner(0, PhotonNetwork.LocalPlayer.ActorNumber - 1).position + Vector3.up, Quaternion.identity);
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
        roomCountDisplay.text = PhotonNetwork.PlayerList.Length + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            //roomOptions.IsOpen = false;
            roomOptions.IsVisible = false;
            isFull = true;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Substring(0, 5) == "Floor") Destroy(other.gameObject);
    }
    #region 블럭 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!isFull) return;
        if (stream.IsWriting)
        {
            for(int i = 0; i < 6; i++)
                stream.SendNext(allTileMap.GetPersonTile(i));
        }
        else
        {
            for (int i = 0; i < 6; i++)
                allTileMap.SetPersonTile(i,(float)stream.ReceiveNext());
        }
    }
    #endregion
}