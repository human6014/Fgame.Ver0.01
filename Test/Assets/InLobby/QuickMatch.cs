using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class QuickMatch : MonoBehaviour
{
    [SerializeField] GameObject playerName;
    public void Start()
    {
        GameManager.Instance().stateIndex = -1;
        GameManager.Instance().playerName = "Default";
    }
    public void OnClicked()
    {
        if (string.IsNullOrEmpty(playerName.GetComponent<Text>().text)) return;
        GameManager.Instance().playerName = playerName.GetComponent<Text>().text;
        GameManager.Instance().stateIndex = 0;
        SceneManager.LoadScene("InGameScene");
    }
}