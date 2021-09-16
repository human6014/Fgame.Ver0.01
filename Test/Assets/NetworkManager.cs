using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks//,IPunObservable
{
    private int roomSize = 2;
    private int playerCount;

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
        roomOptions.MaxPlayers = (byte)roomSize;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom run");
        PhotonNetwork.Instantiate("Player", new Vector3(0, 2, 0), Quaternion.identity);
        //Time.timeScale = 0;
        //view.RPC("PlayerCountUpdate", RpcTarget.All);
    }
    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
    */
    /*
[PunRPC]
void PlayerCountUpdate()
{
   Debug.Log("RPC run");
   playerCount = PhotonNetwork.PlayerList.Length;
   roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
   roomCountDisplay.text = playerCount + " / " + roomSize;
   if (playerCount == roomSize)
   {
       Time.timeScale = 1;
   }
}
*/
}
