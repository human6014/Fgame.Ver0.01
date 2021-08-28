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
        Debug.Log("PersonTileMap Awake ����");

        playerNum = 0;
        Sphere = GetComponent<SphereCollider>();

        Debug.Log("PersonTileMap Awake ��");
    }
    private void Start()
    {
        Debug.Log("PersonTileMap Start ����");

        playerNum++;

        Debug.Log("PersonTileMap Start �� playerNum : "+playerNum);
    }
    private void FixedUpdate()
    {
        Debug.Log("PersonTileMap FixedUpdate ��");
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
        Debug.Log("PersonTileMap OnTriggerExit ����");

        if (other.CompareTag(this.tag))
            Destroy(other.gameObject);

        Debug.Log("PersonTileMap OnTriggerExit ��");
    }
    public void CopyTag()
    {
        Debug.Log("PersonTileMap CopyTag ����");

        Transform[] tran = GetComponentsInChildren<Transform>();
        foreach (Transform t in tran)
        {
            t.gameObject.tag = this.tag;
        }

        Debug.Log("PersonTileMap CopyTag ��");
    }
    
}
