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

    [SerializeField] private GameObject delayCancelButton;
    [SerializeField] private Text roomCountDisplay;
    [SerializeField] private Text timerToStartDisplay;
    [SerializeField] private PhotonView view;
    public bool isFull;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.NickName = "Proto";
        PhotonNetwork.ConnectUsingSettings();
        view = PhotonView.Get(this);
        //view= GetComponent<PhotonView>();
        Debug.Log("NetworkManagerStart");
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom run");
        cMaxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        view.RPC("pUpdate", RpcTarget.AllViaServer);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        view.RPC("pUpdate", RpcTarget.AllViaServer);
        base.OnPlayerLeftRoom(otherPlayer);
    }
    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
    */
    [PunRPC]
    void pUpdate()
    {
        cPlayerCount = PhotonNetwork.PlayerList.Length;
        
        roomCountDisplay.text = cPlayerCount + " / " + cMaxPlayer;
        if (cPlayerCount == cMaxPlayer)
        {
            isFull = true;
            PhotonNetwork.Instantiate("Player", new Vector3(0, 2, 0), Quaternion.identity);
        }
    }
}
