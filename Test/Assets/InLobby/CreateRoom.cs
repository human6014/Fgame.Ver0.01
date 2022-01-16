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
        GameManager.Instance().ResetInfo();
    }
    public void OnClicked()
    {
        GameManager.Instance().SceneMove(1, playerName.GetComponent<Text>().text, roomName.GetComponent<Text>().text);
    }
}
