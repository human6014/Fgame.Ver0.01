using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PersonTileMap : MonoBehaviourPunCallbacks
{
    public Player Player_script;
    public SphereCollider Sphere;

    private float timeSpan = 0.0f;
    private static int playerNum;
    private void Awake()
    {
        Debug.Log("PersonTileMap Awake 시작");

        playerNum = 0;
        Sphere = GetComponent<SphereCollider>();

        Debug.Log("PersonTileMap Awake 끝");
    }
    private void Start()
    {
        Debug.Log("PersonTileMap Start 시작");

        playerNum++;

        Debug.Log("PersonTileMap Start 끝 playerNum : "+playerNum);
    }
    private void FixedUpdate()
    {
        Debug.Log("PersonTileMap FixedUpdate 중");
        if (Player_script.myField > 0)
        {
            Player_script.myField -= (int)(Time.time * 0.125f);
            Debug.Log(Player_script.myField);
        }
        else
        {
            timeSpan += Time.deltaTime;
            if (Sphere.radius >= 0)
            {
                Sphere.radius -= timeSpan / 3000;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("PersonTileMap OnTriggerExit 시작");

        if (other.CompareTag(this.tag))
            Destroy(other.gameObject);

        Debug.Log("PersonTileMap OnTriggerExit 끝");
    }
    public void CopyTag()
    {
        Debug.Log("PersonTileMap CopyTag 시작");

        Transform[] tran = GetComponentsInChildren<Transform>();
        foreach (Transform t in tran)
        {
            t.gameObject.tag = this.tag;
        }

        Debug.Log("PersonTileMap CopyTag 끝");
    }
    
}
