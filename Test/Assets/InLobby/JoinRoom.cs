using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class JoinRoom : MonoBehaviour
{
    [SerializeField] GameObject playerName;
    [SerializeField] GameObject roomName;
    public void Start()
    {
        GameManager.Instance().ResetInfo();
    }
    public void OnClicked()
    {
        if (roomName.GetComponent<Text>().text != "")
        {
            Debug.Log("방 코드 입력");
            return;
        }
        GameManager.Instance().SceneMove(2, playerName.GetComponent<Text>().text, roomName.GetComponent<Text>().text);
    }
}
