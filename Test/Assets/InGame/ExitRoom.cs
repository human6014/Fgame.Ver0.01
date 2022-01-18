using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class ExitRoom : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    public void OnClicked() => networkManager.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
}
