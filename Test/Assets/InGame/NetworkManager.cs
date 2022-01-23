using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class NetworkManager : MonoBehaviourPunCallbacks,IPunObservable
{
    private int stateIndex;
    private string roomCode = string.Empty;
    private string playerName = string.Empty;
    public bool isFull = false;
    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text timerToStartDisplay;
    [SerializeField] Button matchDown;
    [SerializeField] AllTileMap allTileMap;
    PhotonView view;
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };

    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        stateIndex = GameManager.Instance().GetStateIndex();
        roomCode = GameManager.Instance().GetRoomCode();
        //playerName = GameManager.Instance().GetPlayerName(); //Build and Run���� ���� �۵�
        PhotonNetwork.LocalPlayer.NickName = GameManager.Instance().GetPlayerName();
        PhotonNetwork.GameVersion = "1.0";
        view = photonView;

        Debug.Log("NetworkManagerStart");
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
            Debug.LogError("���� ���� �Ұ�");
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
    }
    public void CreatePlayer()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", allTileMap.GetSpawner(0, PhotonNetwork.LocalPlayer.ActorNumber - 1).position + Vector3.up, Quaternion.identity);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        view.RPC(nameof(PunUpdate), RpcTarget.All);
        Debug.Log("OnPlayerLeftRoom");
    }
    public void DisconnectPlayer() => StartCoroutine(nameof(DisconnectNetwork));
    IEnumerator DisconnectNetwork()
    {
        PhotonNetwork.Disconnect();
        yield return new WaitUntil(() => PhotonNetwork.IsConnected == false);
        GameManager.Instance().SetDefaultInformation();
        SceneManager.LoadScene(0);
    }
    [PunRPC]
    void PunUpdate()
    {
        roomCountDisplay.text = PhotonNetwork.PlayerList.Length + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            roomOptions.IsOpen = false;
            roomOptions.IsVisible = false;
            isFull = true;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Substring(0, 5) == "Floor") Destroy(other.gameObject);
    }
    #region �� ����ȭ
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!isFull) return;
        if (stream.IsWriting)
        {
            for(int i = 0; i < 6; i++) stream.SendNext(allTileMap.GetPersonTileRadius(i));
        }
        else
        {
            for (int i = 0; i < 6; i++) allTileMap.SetPersonTileRadius(i,(float)stream.ReceiveNext());
        }
    }
    #endregion
}