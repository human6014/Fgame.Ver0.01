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
            StartCoroutine(TextDown(roomNameAlert, "방 코드가 잘못되었습니다"));
            return false;
        }
        if (string.IsNullOrEmpty(playerName.text))
        {
            StartCoroutine(TextDown(nickNameAlert, "닉네임을 설정해주세요"));
            return false;
        }
        if (!Regex.IsMatch(playerName.text, "^[0-9a-zA-Z가-힣]*$"))
        {
            StartCoroutine(TextDown(nickNameAlert, "올바른 숫자 영문 한글만 입력해주세요"));
            return false;
        }

        if (level == 1) return true;

        if (string.IsNullOrEmpty(roomName.text))
        {
            StartCoroutine(TextDown(roomNameAlert, "방 코드를 설정해주세요"));
            return false;
        }
        if (!Regex.IsMatch(roomName.text, "^[0-9a-zA-Z가-힣]*$"))
        {
            StartCoroutine(TextDown(roomNameAlert, "올바른 숫자 영문 한글만 입력해주세요"));
            return false;
        }

        if (level == 2) return true;

        if (string.IsNullOrEmpty(maxPlayer.text))
        {
            StartCoroutine(TextDown(maxPlayerAlert, "최대 인원 수를 설정해주세요"));
            return false;
        }
        int tempMaxPlayer = int.Parse(maxPlayer.text);
        if (tempMaxPlayer < 2 || tempMaxPlayer > 6)
        {
            StartCoroutine(TextDown(maxPlayerAlert, "2 ~ 6 사이의 수를 입력해주세요"));
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
