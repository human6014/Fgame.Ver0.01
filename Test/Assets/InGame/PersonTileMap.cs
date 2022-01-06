using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PersonTileMap : MonoBehaviour
{
    public GameManager gameManager_script;
    public NetworkManager networkManager_script;
    public AllTileMap allTileMap;
    public SphereCollider sphere;
    private MeshCollider meshCollider;
    private new Rigidbody rigidbody;
    private new Renderer renderer;
    float initRadius;
    bool outPlayer;

    private void Start()
    {
        sphere = GetComponent<SphereCollider>();
        if (gameObject.CompareTag("Floor7")) sphere.radius = 5;
        else sphere.radius = gameManager_script.myField[1, gameManager_script.playerNum];
        gameManager_script.playerNum++;
        initRadius = sphere.radius;
    }
    private void Update()
    {
        if (gameObject.CompareTag("Floor7")) return;
        allTileMap.childCount[int.Parse(transform.name.Substring(13, 1)) - 1] = transform.childCount - 1; //위치 수정 보류
        if (sphere.radius >= 0)
        {
            if (networkManager_script.isFull) sphere.radius -= Time.deltaTime * Time.time / 1000;
        }
        else if (!outPlayer)
        {
            Transform[] child = GetComponentsInChildren<Transform>();

            foreach (Transform iter in child)
            {
                if (iter.name != "HexTileMap" && iter != transform)
                    StartCoroutine(FallWaiting(iter.gameObject));
            }
            for (int i = 0; i < 2; i++) Destroy(allTileMap.childPortal[int.Parse(transform.name.Substring(13, 1)) - 1, i].gameObject);
            outPlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tag))
        {
            if (sphere.radius == initRadius) Destroy(other.gameObject);
            else
            {
                StartCoroutine(FallWaiting(other.gameObject));
            }
        }
    }
    IEnumerator FallWaiting(GameObject other)
    {
        renderer = other.GetComponent<Renderer>();
        renderer.material.color = new Color(255 / 255f, 25 / 255f, 25 / 255f);
        yield return new WaitForSeconds(3f); //밑에 순서 중요함
        if (!other) yield break;
        meshCollider = other.GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        rigidbody = other.GetComponent<Rigidbody>();
        rigidbody.mass = 0.5f;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }
}
