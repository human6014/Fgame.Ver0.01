using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Back : MonoBehaviour
{
    [SerializeField] GameObject FirstCanvas;
    [SerializeField] GameObject SecondCanvas;
    public void OnClicked()
    {
        FirstCanvas.SetActive(true);
        SecondCanvas.SetActive(false);
    }
}