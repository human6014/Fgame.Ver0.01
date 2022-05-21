using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] Text playerName;
    [SerializeField] Text roomName;
    [SerializeField] Text maxPlayer;
    [SerializeField] Text nickNameAlert;
    [SerializeField] Text roomNameAlert;
    [SerializeField] Text maxPlayerAlert;
    public bool IsValidate(int level)
    {
        if (string.IsNullOrEmpty(playerName.text))
        {
            StartCoroutine(TextDown(nickNameAlert, "¥–≥◊¿”¿ª º≥¡§«ÿ¡÷ººø‰"));
            return false;
        }
        if (!Regex.IsMatch(playerName.text, "^[0-9a-zA-Z∞°-∆R]*$"))
        {
            StartCoroutine(TextDown(nickNameAlert, "º˝¿⁄ øµπÆ «—±€∏∏ ¿‘∑¬«ÿ¡÷ººø‰"));
            return false;
        }

        if (level == 1) return true;

        if (string.IsNullOrEmpty(roomName.text))
        {
            StartCoroutine(TextDown(roomNameAlert, "πÊ ƒ⁄µÂ∏¶ º≥¡§«ÿ¡÷ººø‰"));
            return false;
        }
        if (!Regex.IsMatch(roomName.text, "^[0-9a-zA-Z∞°-∆R]*$"))
        {
            StartCoroutine(TextDown(roomNameAlert, "º˝¿⁄ øµπÆ «—±€∏∏ ¿‘∑¬«ÿ¡÷ººø‰"));
            return false;
        }

        if (level == 2) return true;

        if (string.IsNullOrEmpty(maxPlayer.text))
        {
            StartCoroutine(TextDown(maxPlayerAlert, "√÷¥Î ¿Œø¯ ºˆ∏¶ º≥¡§«ÿ¡÷ººø‰"));
            return false;
        }
        int tempMaxPlayer = int.Parse(maxPlayer.text);
        if (tempMaxPlayer < 2 || tempMaxPlayer > 6)
        {
            StartCoroutine(TextDown(maxPlayerAlert, "2 ~ 6 ªÁ¿Ã¿« ºˆ∏¶ ¿‘∑¬«ÿ¡÷ººø‰"));
            return false;
        }

        if (level == 3) return true;
        return true;
    }
    public IEnumerator TextDown(Text alert,string text)
    {
        if (alert == null) alert = roomNameAlert;
        alert.text = text;
        yield return new WaitForSeconds(3);
        alert.text = "";
    }
}
