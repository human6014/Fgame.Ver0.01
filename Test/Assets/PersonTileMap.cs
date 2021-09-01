using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PersonTileMap : MonoBehaviourPunCallbacks
{
    public Player Player_script;
    public GameManager GameManager_script;
    public SphereCollider Sphere;

    private float timeSpan = 0.0f;
    private void Start()
    {
        Debug.Log("PersonTileMap Start시작");

        this.Sphere = this.GetComponent<SphereCollider>();
        this.Sphere.radius = GameManager_script.myField[1, GameManager_script.i];
        Debug.Log(this.Sphere.radius);
        GameManager_script.i++;

        Debug.Log("PersonTileMap Start 끝");
    }
    private void Update()
    {
        Debug.Log("PersonTileMap FixedUpdate 중");

        if (this.Sphere.radius >= 0)
        {
            this.Sphere.radius -= Time.deltaTime *Time.time/10;
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
