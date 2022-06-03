using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    [SerializeField] GameObject keyManuaPanel;
    [SerializeField] GameObject explainPanel1;
    [SerializeField] GameObject explainPanel2;
    bool isOnKeyManual;
    bool isFirstPage;
    public void OnKeyManual()
    {
        isOnKeyManual = !isOnKeyManual;
        keyManuaPanel.SetActive(isOnKeyManual);
        if (!isOnKeyManual)
        {
            explainPanel1.SetActive(isFirstPage);
            explainPanel2.SetActive(!isFirstPage);
        }
        else
        {
            explainPanel1.SetActive(false);
            explainPanel2.SetActive(false);
        }
    }
    public void SetDefault()
    {
        keyManuaPanel.SetActive(false);
        explainPanel1.SetActive(true);
        explainPanel2.SetActive(false);
        isOnKeyManual = false;
        isFirstPage = true;
    }
    public void SetPage(bool _isFirstPage) => this.isFirstPage = _isFirstPage;
    
}
