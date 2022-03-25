using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class JoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] Text playerName;
    [SerializeField] Text roomName;
    public void OnClicked()
    {
        if (string.IsNullOrEmpty(playerName.text) || string.IsNullOrEmpty(roomName.text)) return;
        string tempPlayerName = playerName.text,
               tempRoomCode  = roomName.text;
        GameManager.Instance().OnStartGame(2, tempPlayerName, tempRoomCode, -1);
    }
}