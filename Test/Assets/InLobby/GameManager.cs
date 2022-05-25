using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private string playerName = "Default";
    private string roomCode = "Default";
    private int maxPlayer = -1;
    private int stateIndex = -1;
    private const byte MAXPLAYER = 1;

    RoomOptions roomOptions;
    //싱글톤 필요 없을듯?
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
        PhotonNetwork.GameVersion = "1.0";
    }
    public override void OnConnectedToMaster() { }
    public void OnStartGame(int _stateIndex, string _playerName, string _roomCode, int _maxPlayer)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
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
    public override void OnJoinedLobby() => Debug.Log("OnJoinedLobby");
    public override void OnLeftLobby() => Debug.Log("OnLeftLobby");
    public override void OnCreatedRoom(){ if (stateIndex == 1) PhotonNetwork.CurrentRoom.IsVisible = false; }
    public override void OnJoinedRoom() => PhotonNetwork.LoadLevel("InGameScene");
    public override void OnCreateRoomFailed(short returnCode, string message) => Debug.Log("OnCreateRoomFailed");
    public override void OnJoinRandomFailed(short returnCode, string message) => PhotonNetwork.CreateRoom(null, roomOptions = new RoomOptions { MaxPlayers = MAXPLAYER });
    public override void OnJoinRoomFailed(short returnCode, string message) => FindObjectOfType<LobbyManager>().IsValidate(0);
    public override void OnLeftRoom()
    {
        SetDefaultInformation();
        PhotonNetwork.LoadLevel("Lobby");
    }
    /*
    void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 20;
        GUILayout.BeginVertical("Box", GUILayout.Width(200), GUILayout.MinHeight(100));

        GUILayout.Label("서버시간 : " + PhotonNetwork.Time);
        GUILayout.Label("상태 : " + PhotonNetwork.NetworkClientState);
        GUILayout.Label("씬 : " + SceneManager.GetActiveScene().name);

        GUILayout.EndVertical();
    }
    */
    private void OnApplicationQuit() => PhotonNetwork.Disconnect();
}