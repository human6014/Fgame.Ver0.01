using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CreateRoom : MonoBehaviour
{
    [SerializeField] GameObject playerName;
    [SerializeField] GameObject roomName;
    public void OnClicked()
    {
        string tempPlayerName = playerName.GetComponent<Text>().text,
               tempRoomCode = roomName.GetComponent<Text>().text;
        if (string.IsNullOrEmpty(tempPlayerName) || string.IsNullOrEmpty(tempRoomCode)) return;
        GameManager.Instance().OnStartGame(1, tempPlayerName, tempRoomCode);
    }
}