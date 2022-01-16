using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class ExitRoom : MonoBehaviour
{
    public void OnClicked() => GameManager.Instance().OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
}
