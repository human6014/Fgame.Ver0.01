using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private string playerName = "";
    private string roomCode = "";
    private int maxPlayer = -1;
    private int stateIndex = -1;
    private const byte MAXPLAYER = 6;

    LobbyManager lobbyManager;
    RoomOptions roomOptions;
    #region 싱글톤
    static GameManager _instance = null;
    public static GameManager Instance() => _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else Destroy(gameObject);
    }
    #endregion
    #region Setter+Getter
    public void SetDefaultInformation()
    {
        playerName = "Default";
        roomCode = "Default";
        stateIndex = -1;
    }
    public string GetPlayerName() => playerName;
    public string GetRoomCode() => roomCode;
    public int GetStateIndex() => stateIndex;
    #endregion
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.GameVersion = "2.0";
    }
    public void OnStartGame(int _stateIndex, string _playerName, string _roomCode, int _maxPlayer)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        if (PhotonNetwork.NetworkingClient.Server != ServerConnection.MasterServer) return;
        stateIndex = _stateIndex;
        roomCode   = _roomCode;
        playerName = _playerName;
        maxPlayer = _maxPlayer;
        switch (stateIndex)
        {
            case 0:
                PhotonNetwork.JoinRandomRoom();
                break;
            case 1:
                roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayer };
                PhotonNetwork.CreateRoom(roomCode, roomOptions);
                break;
            case 2:
                PhotonNetwork.JoinRoom(roomCode);
                break;
            default:
                Debug.LogError("Error");
                break;
        }
    }
    public override void OnCreatedRoom(){ if (stateIndex == 1) PhotonNetwork.CurrentRoom.IsVisible = false; }
    public override void OnJoinedRoom() => PhotonNetwork.LoadLevel("InGameScene");
    public override void OnJoinRandomFailed(short returnCode, string message) => PhotonNetwork.CreateRoom(null, roomOptions = new RoomOptions { MaxPlayers = MAXPLAYER });
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (lobbyManager == null) lobbyManager = FindObjectOfType<LobbyManager>();
        lobbyManager.IsValidate(0);
    }
    public override void OnLeftRoom()
    {
        SetDefaultInformation();
        PhotonNetwork.LoadLevel("Lobby");
    }
    private void OnApplicationQuit() => PhotonNetwork.Disconnect();
}