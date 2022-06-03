using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Collections;

public class QuickMatch : MonoBehaviour
{
    [SerializeField] LobbyManager lobbyManager;

    [SerializeField] Text playerName;
    public void OnClicked()
    {
        if (!lobbyManager.IsValidate(1)) return;
        string tempPlayerName = playerName.text;
        GameManager.Instance().OnStartGame(0,tempPlayerName,"", -1);
    }
}