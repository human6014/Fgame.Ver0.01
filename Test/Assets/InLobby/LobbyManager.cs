using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class LobbyManager : MonoBehaviour
{
    public string inGameName;
    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        DontDestroyOnLoad(gameObject);
    }
}
