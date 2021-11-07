using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Button1 : MonoBehaviour
{
    public void start() => SceneManager.LoadScene("InGameScene");
}
