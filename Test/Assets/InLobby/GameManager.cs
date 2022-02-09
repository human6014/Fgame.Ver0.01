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
    private int stateIndex = -1;
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };
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
    //public void SetPlayerName(string pn) => playerName = pn;
    //public void SetRoomCode(string rc) => roomCode = rc;
    //public void SetStateIndex(int si) => stateIndex = si;
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
    public void OnStartGame(int _stateIndex, string _playerName, string _roomCode)
    {
        stateIndex = _stateIndex;
        roomCode   = _roomCode;
        playerName = _playerName;
        switch (stateIndex)
        {
            case 0:
                PhotonNetwork.JoinRandomRoom();
                break;
            case 1:
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
    public override void OnJoinRandomFailed(short returnCode, string message) => PhotonNetwork.CreateRoom(null, roomOptions);
    public override void OnJoinRoomFailed(short returnCode, string message) => Debug.Log("OnJoinRoomFailed");
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