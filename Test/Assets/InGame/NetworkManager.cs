using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class NetworkManager : MonoBehaviourPunCallbacks,IPunObservable
{
    private int stateIndex;
    private string roomCode = string.Empty;
    private string playerName = string.Empty;
    public int playerNumber;
    public bool isFull = false;
    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text timerToStartDisplay;
    [SerializeField] Button matchDown;
    [SerializeField] AllTileMap allTileMap;
    PhotonView view;
    
    private void Awake() { }
    private void Start()
    {
        stateIndex = GameManager.Instance().GetStateIndex();
        roomCode = GameManager.Instance().GetRoomCode();
        //playerName = GameManager.Instance().GetPlayerName(); //Build and Run에서 정상 작동
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance().GetPlayerName();
        view = photonView;
    }
    public void CreatePlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.GetSpawner(0, PhotonNetwork.LocalPlayer.ActorNumber - 1).position + Vector3.up, Quaternion.identity);
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        PhotonNetwork.LocalPlayer.SetPlayerNumber(playerNumber++);
        view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        PhotonNetwork.LocalPlayer.SetPlayerNumber(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1);
        view.RPC(nameof(PunUpdate), RpcTarget.AllBuffered);
    }
    [PunRPC]
    void PunUpdate()
    {
        roomCountDisplay.text = PhotonNetwork.PlayerList.Length + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
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
        if (!isFull || !allTileMap.GetIsCreateTile()) return;
        if (stream.IsWriting)
        {
            for(int i = 0; i < 6; i++) stream.SendNext(allTileMap.GetPersonTileRadius(i));
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                allTileMap.SetPersonTileRadius(i, (float)stream.ReceiveNext());
            }
        }
    }
    #endregion
}