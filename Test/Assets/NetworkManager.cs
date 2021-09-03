using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    private byte maxPlayer=2;
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
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayer;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("Player", new Vector3(0,2,0), Quaternion.identity);

        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayer);
        Time.timeScale = 0;

    }
    void OnPhotonPlayerConnected()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayer);
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayer)
            Time.timeScale = 1;
    }
}
