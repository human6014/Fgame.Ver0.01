using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PersonTileMap : MonoBehaviour
{
    public GameManager gameManager_script;
    public NetworkManager networkManager_script;
    public SphereCollider sphere;

    private MeshCollider meshCollider;
    private new Rigidbody rigidbody;
    private new Renderer renderer;
    float initRadius;
    bool outPlayer;
    private void Start()
    {
        Debug.Log("PersonTileMap Start����");

        sphere = GetComponent<SphereCollider>();
        if (gameObject.CompareTag("Floor7")) sphere.radius = 5;
        //else sphere.radius = GameManager.Instance().myField[1, gameManager_script.playerNum];

        initRadius = sphere.radius;
        //gameManager_script.playerNum++;

        Debug.Log("PersonTileMap Start ��");
    }
    private void Update()
    {
        Debug.Log("PersonTileMap Update ��");
        if (sphere.radius >= 0)
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers==PhotonNetwork.PlayerList.Length && !gameObject.CompareTag("Floor7")) sphere.radius -= Time.deltaTime * Time.time / 10;
        }
        else
        {
            if (outPlayer) return;
            Transform[] child = GetComponentsInChildren<Transform>();

            foreach (Transform iter in child)
            {
                // �θ�(this.gameObject)�� ���� ���� �ʱ� ���� ó��
                if (iter.name != "HexTileMap" && iter!=transform)
                {
                    StartCoroutine(FallWaiting(iter.gameObject));
                }
            }
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
        yield return new WaitForSeconds(3f); //�ؿ� ���� �߿���
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
