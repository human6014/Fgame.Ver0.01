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
    public void Start()
    {
        GameManager.Instance().stateIndex = -1;
        GameManager.Instance().roomCode = "Default";
        GameManager.Instance().playerName = "Default";
    }
    public void OnClicked()
    {
        if (string.IsNullOrEmpty(playerName.GetComponent<Text>().text) || string.IsNullOrEmpty(roomName.GetComponent<Text>().text)) return;
        GameManager.Instance().playerName = playerName.GetComponent<Text>().text;
        GameManager.Instance().roomCode = roomName.GetComponent<Text>().text;
        GameManager.Instance().stateIndex = 2;
        SceneManager.LoadScene("InGameScene");
    }
}