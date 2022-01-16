using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class QuickMatch : MonoBehaviour
{
    [SerializeField] GameObject playerName;
    public void Start()
    {
        GameManager.Instance().ResetInfo();
    }
    public void OnClicked()
    {
        GameManager.Instance().SceneMove(0, playerName.GetComponent<Text>().text, "");
    }
}
