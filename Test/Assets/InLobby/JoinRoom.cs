using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class JoinRoom : MonoBehaviour
{
    [SerializeField] GameObject playerName;
    [SerializeField] GameObject roomName;
    LobbyManager lobbyManager;
    public void Start()
    {
        lobbyManager = FindObjectsOfType<LobbyManager>()[0];
        lobbyManager.stateIndex = -1;
        lobbyManager.roomCode = "Default";
        lobbyManager.playerName = "Default";
    }
    public void OnClicked()
    {
        lobbyManager.playerName = playerName.GetComponent<Text>().text;
        lobbyManager.roomCode = roomName.GetComponent<Text>().text;
        lobbyManager.stateIndex = 2;
        SceneManager.LoadScene("InGameScene");
    }
}
