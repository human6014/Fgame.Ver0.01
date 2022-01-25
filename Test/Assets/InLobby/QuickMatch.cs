using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class QuickMatch : MonoBehaviour
{
    [SerializeField] GameObject playerName;
    public void OnClicked()
    {
        string tempPlayerName = playerName.GetComponent<Text>().text;
        if (string.IsNullOrEmpty(tempPlayerName)) return;
        GameManager.Instance().OnStartGame(0,tempPlayerName,"");
    }
}