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
        if(level == 0)
        {
            StartCoroutine(TextDown(roomNameAlert, "¹æ ÄÚµå°¡ Àß¸øµÇ¾ú½À´Ï´Ù"));
            return false;
        }
        if (string.IsNullOrEmpty(playerName.text))
        {
            StartCoroutine(TextDown(nickNameAlert, "´Ð³×ÀÓÀ» ¼³Á¤ÇØÁÖ¼¼¿ä"));
            return false;
        }
        if (!Regex.IsMatch(playerName.text, "^[0-9a-zA-Z°¡-ÆR]*$"))
        {
            StartCoroutine(TextDown(nickNameAlert, "¿Ã¹Ù¸¥ ¼ýÀÚ ¿µ¹® ÇÑ±Û¸¸ ÀÔ·ÂÇØÁÖ¼¼¿ä"));
            return false;
        }

        if (level == 1) return true;

        if (string.IsNullOrEmpty(roomName.text))
        {
            StartCoroutine(TextDown(roomNameAlert, "¹æ ÄÚµå¸¦ ¼³Á¤ÇØÁÖ¼¼¿ä"));
            return false;
        }
        if (!Regex.IsMatch(roomName.text, "^[0-9a-zA-Z°¡-ÆR]*$"))
        {
            StartCoroutine(TextDown(roomNameAlert, "¿Ã¹Ù¸¥ ¼ýÀÚ ¿µ¹® ÇÑ±Û¸¸ ÀÔ·ÂÇØÁÖ¼¼¿ä"));
            return false;
        }

        if (level == 2) return true;

        if (string.IsNullOrEmpty(maxPlayer.text))
        {
            StartCoroutine(TextDown(maxPlayerAlert, "ÃÖ´ë ÀÎ¿ø ¼ö¸¦ ¼³Á¤ÇØÁÖ¼¼¿ä"));
            return false;
        }
        int tempMaxPlayer = int.Parse(maxPlayer.text);
        if (tempMaxPlayer < 2 || tempMaxPlayer > 6)
        {
            StartCoroutine(TextDown(maxPlayerAlert, "2 ~ 6 »çÀÌÀÇ ¼ö¸¦ ÀÔ·ÂÇØÁÖ¼¼¿ä"));
            return false;
        }

        if (level == 3) return true;
        return true;
    }
    public IEnumerator TextDown(Text alert,string text)
    {
//if (alert == null) alert = roomNameAlert;
        alert.text = text;
        yield return new WaitForSeconds(3);
        //if (alert == null) yield break;
        alert.text = "";
    }
}
