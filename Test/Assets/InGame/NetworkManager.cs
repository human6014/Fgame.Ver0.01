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
    public bool isFull = false;
    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text timerToStartDisplay;
    [SerializeField] Button matchDoun;
    [SerializeField] PhotonView view;
    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;
    private void Start()
    {
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.NickName = "Proto";
        PhotonNetwork.ConnectUsingSettings();
        view = PhotonView.Get(this); //뭘까 이건
        Debug.Log("NetworkManagerStart");
    }
    public override void OnConnectedToMaster() => PhotonNetwork.JoinRandomRoom();
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions{MaxPlayers = 1};
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom run");
        cMaxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        view.RPC("PunUpdate", RpcTarget.All);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) => view.RPC("PunUpdate", RpcTarget.All);
    [PunRPC]
    void PunUpdate()
    {
        cPlayerCount = PhotonNetwork.PlayerList.Length;
        roomCountDisplay.text = cPlayerCount + " / " + cMaxPlayer;
        if (cPlayerCount == cMaxPlayer)
        {
            Debug.Log("플레이어 생성");
            PhotonNetwork.Instantiate("Player", new Vector3(0, 2, 0), Quaternion.identity);
            isFull = true;
        }
    }
}
