using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class GameManager : MonoBehaviour
{
    public string playerName = "Default";
    public string roomCode = "Default";
    public int stateIndex = -1;
    static GameManager _instance = null;
    public static GameManager Instance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else Destroy(gameObject);
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
}