using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class NewBehaviourScript : MonoBehaviour
{
    private void Awake()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
    }
    
}
