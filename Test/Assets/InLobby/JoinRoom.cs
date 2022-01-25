using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class JoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerName;
    [SerializeField] GameObject roomName;
    public void OnClicked()
    {
        string tempPlayerName = playerName.GetComponent<Text>().text,
               tempRoomCode = roomName.GetComponent<Text>().text;
        if (string.IsNullOrEmpty(tempPlayerName) || string.IsNullOrEmpty(tempRoomCode)) return;
        GameManager.Instance().OnStartGame(2, tempPlayerName, tempRoomCode);
    }
}