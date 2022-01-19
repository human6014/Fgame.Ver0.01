using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitRoom : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    public void OnClicked() => networkManager.DisconnectPlayer();
}