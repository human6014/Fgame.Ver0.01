using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class QuickMatch : MonoBehaviour
{
    [SerializeField] Text playerName;
    public void OnClicked()
    {
        if (string.IsNullOrEmpty(playerName.text)) return;
        string tempPlayerName = playerName.text;
        GameManager.Instance().OnStartGame(0,tempPlayerName,"", -1);
    }
}