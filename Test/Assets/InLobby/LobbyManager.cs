using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class LobbyManager : MonoBehaviour
{
    public string inGameName;
    private void Awake()
    {
        var obj = FindObjectsOfType<LobbyManager>(); 
        if (obj.Length == 1) DontDestroyOnLoad(gameObject); 
        else Destroy(gameObject); 

        //PhotonNetwork.IsMessageQueueRunning = false;
    }
}
