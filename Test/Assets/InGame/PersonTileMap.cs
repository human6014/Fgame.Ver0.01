using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PersonTileMap : MonoBehaviour
{
    public GameManager gameManager_script;
    public NetworkManager networkManager_script;
    public SphereCollider sphere;
    private MeshCollider meshCollider;
    private new Rigidbody rigidbody;
    private new Renderer renderer;
    float initRadius;
    private void Start()
    {
        Debug.Log("PersonTileMap Start시작");

        sphere = GetComponent<SphereCollider>();
        if (gameObject.CompareTag("Floor7")) sphere.radius = 5;
        else sphere.radius = gameManager_script.myField[1, gameManager_script.playerNum];

        initRadius = sphere.radius;
        gameManager_script.playerNum++;

        Debug.Log("PersonTileMap Start 끝");
    }
    private void Update()
    {
        Debug.Log("PersonTileMap Update 중");
        if (sphere.radius >= 0 && networkManager_script.isFull && !gameObject.CompareTag("Floor7")) sphere.radius -= Time.deltaTime * Time.time / 1000;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("PersonTileMap OnTriggerExit 시작");
        if (other.CompareTag(tag))
        {
            if (sphere.radius == initRadius) Destroy(other.gameObject);
            else
            {
                renderer = other.GetComponent<Renderer>();
                renderer.material.color = new Color(255 / 255f, 25 / 255f, 25 / 255f);
                StartCoroutine(FallWaiting(other));
            }
        }
        Debug.Log("PersonTileMap OnTriggerExit 끝");
    }
    IEnumerator FallWaiting(Collider other)
    {
        yield return new WaitForSeconds(3f); //밑에 순서 중요함
        meshCollider = other.GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        rigidbody = other.GetComponent<Rigidbody>();
        rigidbody.mass = 0.5f;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }
}
