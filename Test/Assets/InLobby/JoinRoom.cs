using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;

public class JoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] LobbyManager lobbyManager;

    [SerializeField] Text playerName;
    [SerializeField] Text roomName;
    public void OnClicked()
    {
        if (!lobbyManager.IsValidate(2)) return;
        string tempPlayerName = playerName.text,
               tempRoomCode  = roomName.text;
        GameManager.Instance().OnStartGame(2, tempPlayerName, tempRoomCode, -1);
    }
}