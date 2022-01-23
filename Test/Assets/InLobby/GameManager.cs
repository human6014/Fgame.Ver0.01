using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class GameManager : MonoBehaviour
{
    private string playerName = "Default";
    private string roomCode = "Default";
    private int stateIndex = -1;

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
    public void SetPlayerName(string pn) => playerName = pn;
    public void SetRoomCode(string rc) => roomCode = rc;
    public void SetStateIndex(int si) => stateIndex = si;
    public string GetPlayerName() => playerName;
    public string GetRoomCode() => roomCode;
    public int GetStateIndex() => stateIndex;
    #endregion
    public void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
    }
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
    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }
}