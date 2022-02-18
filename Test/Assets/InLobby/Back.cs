using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Back : MonoBehaviour
{
    [SerializeField] GameObject StartPanel;
    [SerializeField] GameObject ReadyPanel;
    public void OnClicked()
    {
        StartPanel.SetActive(true);
        ReadyPanel.SetActive(false);
    }
}
