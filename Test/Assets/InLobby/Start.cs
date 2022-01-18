using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Start : MonoBehaviour
{
    [SerializeField] GameObject FirstCanvas;
    [SerializeField] GameObject SecondCanvas;
    public void OnClicked()
    {
        GameManager.Instance().Connect();
        FirstCanvas.SetActive(false);
        SecondCanvas.SetActive(true);
    }
}
