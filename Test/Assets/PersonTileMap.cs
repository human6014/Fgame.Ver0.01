using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonTileMap : MonoBehaviour
{
    
    public SphereCollider Sphere;

    private float timeSpan = 0.0f;
    private void Awake()
    {
        Debug.Log("PersonTileMap Awake ����");

        Sphere = GetComponent<SphereCollider>();

        Debug.Log("PersonTileMap Awake ��");
    }
    private void FixedUpdate()
    {
        Debug.Log("PersonTileMap FixedUpdate ��");

        timeSpan += Time.deltaTime;
        if (Sphere.radius > 0)
        {
            Sphere.radius -= timeSpan / 3000;
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
