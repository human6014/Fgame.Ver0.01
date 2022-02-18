using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Start : MonoBehaviour
{
    [SerializeField] GameObject StartPanel;
    [SerializeField] GameObject ReadyPanel;
    public void OnClicked()
    {
        StartPanel.SetActive(false);
        ReadyPanel.SetActive(true);
    }
}
