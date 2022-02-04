using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Grenade : MonoBehaviourPunCallbacks
{
    private int destroyBlockCount;
    public GameObject particle;
    public MeshRenderer meshRenderer;
    public PhotonView pv;
    public int damage;
    private void Start()
    {
        Destroy(gameObject, 3);
    }
    private void OnTriggerEnter(Collider other)
    {

    }
}
