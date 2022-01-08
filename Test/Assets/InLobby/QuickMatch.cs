using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class QuickMatch : MonoBehaviour
{
    [SerializeField] GameObject playerName;
    [SerializeField] LobbyManager lobbyManager;
    public void OnClicked()
    {
        lobbyManager.inGameName = playerName.GetComponent<Text>().text;
        SceneManager.LoadScene("InGameScene");
    }
}
