using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyManager : MonoBehaviour
{
    public string playerName = "Default";
    public string roomCode = "Default";
    public int stateIndex = -1;
    private void Awake()
    {
        var obj = FindObjectsOfType<LobbyManager>(); 
        if (obj.Length == 1) DontDestroyOnLoad(gameObject); 
        else Destroy(gameObject); 

        //PhotonNetwork.IsMessageQueueRunning = false;
    }
    /*
    public void SetDefaultName()
    {
        playerName = "Default";
    }
    public void SetDefaultCode()
    {
        roomCode = "Default";
    }
    */
}
