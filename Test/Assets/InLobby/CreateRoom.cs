using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class CreateRoom : MonoBehaviour
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
        GameManager.Instance().stateIndex = 1;
        SceneManager.LoadScene("InGameScene");
    }
}