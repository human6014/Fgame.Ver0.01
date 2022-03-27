using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CreateRoom : MonoBehaviour
{
    [SerializeField] Text playerName;
    [SerializeField] Text roomName;
    [SerializeField] Text maxPlayer;
    public void OnClicked()
    {
        if (string.IsNullOrEmpty(playerName.text) ||
            string.IsNullOrEmpty(roomName.text) ||
            string.IsNullOrEmpty(maxPlayer.text)) return;
        string tempPlayerName = playerName.text,
               tempRoomCode = roomName.text;
        int tempMaxPlayer = int.Parse(maxPlayer.text);
        if (tempMaxPlayer < 2 || tempMaxPlayer > 6) return;
        GameManager.Instance().OnStartGame(1, tempPlayerName, tempRoomCode, tempMaxPlayer);
    }
}