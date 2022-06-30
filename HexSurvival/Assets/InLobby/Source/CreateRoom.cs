using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text.RegularExpressions;

public class CreateRoom : MonoBehaviour
{
    [SerializeField] LobbyManager lobbyManager;

    [SerializeField] Text playerName;
    [SerializeField] Text roomName;
    [SerializeField] Text maxPlayer;
    public void OnClicked()
    {
        if (!lobbyManager.IsValidate(3)) return;
        string tempPlayerName = playerName.text,
               tempRoomCode = roomName.text;
        int tempMaxPlayer = int.Parse(maxPlayer.text);
        GameManager.Instance().OnStartGame(1, tempPlayerName, tempRoomCode, tempMaxPlayer);
    }
}