using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonTileMap : MonoBehaviour
{
    
    public SphereCollider Sphere;

    private float timeSpan = 0.0f;
    private void Awake()
    {
        Debug.Log("PersonTileMap Awake 시작");

        Sphere = GetComponent<SphereCollider>();

        Debug.Log("PersonTileMap Awake 끝");
    }
    private void FixedUpdate()
    {
        Debug.Log("PersonTileMap FixedUpdate 중");

        timeSpan += Time.deltaTime;
        if (Sphere.radius > 0)
        {
            Sphere.radius -= timeSpan / 3000;
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
