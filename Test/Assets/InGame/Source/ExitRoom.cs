using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class ExitRoom : MonoBehaviour
{
    [SerializeField] GeneralManager generalManager;
    public void OnClicked()
    {
        PhotonNetwork.LeaveRoom();
        generalManager.SetChatClear();
    }
}