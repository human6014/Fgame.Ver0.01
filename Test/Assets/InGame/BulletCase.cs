using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BulletCase : MonoBehaviourPunCallbacks
{
    void Start() => Destroy(gameObject, 3);
}
