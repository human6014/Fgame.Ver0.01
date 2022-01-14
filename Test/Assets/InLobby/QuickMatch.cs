using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class QuickMatch : MonoBehaviour
{
    [SerializeField] GameObject playerName;
    LobbyManager lobbyManager;
    public void Start()
    {
        lobbyManager = FindObjectsOfType<LobbyManager>()[0];
        lobbyManager.stateIndex = -1;
        lobbyManager.playerName = "Default";
    }
    public void OnClicked()
    {
        lobbyManager.playerName = playerName.GetComponent<Text>().text;
        lobbyManager.stateIndex = 0;
        SceneManager.LoadScene("InGameScene");
    }
}
