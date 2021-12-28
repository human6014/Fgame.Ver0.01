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
    bool triggerCount = true;
    int count = 1;
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
        if(transform.name == "PersonTileMap1")
            Debug.Log(transform.childCount-1);
        if (triggerCount)
        {
            //StartCoroutine("TagChanging");
            triggerCount = false;
        }
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
    /*
    IEnumerator TagChanging()
    {
        yield return new WaitForSeconds(0.01f); //코루틴 없으면 원형 타일 만들기 전에 태그를 바꿔버림
        Transform[] tran = GetComponentsInChildren<Transform>();
        //string[] tempString = new string[2];
        Debug.Log("TagChanging");
        int [] Num = new int[2];
        int sign = 1;
        string name;
        string[] tempString = new string[2];
        foreach (Transform t in tran)
        {
            if (t != transform.GetChild(0))
            {
                name = t.name;
                tempString[0] = "";
                tempString[1] = "";
                //string[] tempString = name.Split(',');
                for (int i = 0; i < name.Length; i++)
                {
                    if (name[i] == ',')
                    {
                        for (int j = 0; j < i; j++)
                        {
                            tempString[0] += name[j];
                        }
                        for (int j = i + 1; j < name.Length; j++)
                        {
                            tempString[1] += name[j];
                        }
                        break;
                    }
                } //Split으로 만들자!
                for (int i = 0; i < 2; i++) {
                    if (tempString[0] == "" && tempString[1] == "") break;
                    if (tempString[i].StartsWith("-"))
                    {
                        sign = -1;
                        tempString[i].Remove(0);
                    }
                    Num[i] = int.Parse(tempString[i]) * sign;
                    sign = 1;
                }
                
                if (t.gameObject.CompareTag("Floor1"))
                {
                    if (Num[1] == count && Num[1] / 2 == Num[0])
                    {
                        Debug.Log(Num[0] + "," + Num[1]);
                        t.gameObject.tag = "Floor7";
                        count++;
                    }
                }
                
            }
        }
    }
    */
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
