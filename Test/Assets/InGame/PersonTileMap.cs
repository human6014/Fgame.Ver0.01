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
    char split = ',';
    bool triggerCount = true;
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
        Debug.Log("PersonTileMap FixedUpdate 중");
        if (sphere.radius >= 0 && networkManager_script.isFull && !gameObject.CompareTag("Floor7")) sphere.radius -= Time.deltaTime * Time.time / 1000;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("PersonTileMap OnTriggerExit 시작");
        if (other.CompareTag(tag))
        {
            if (sphere.radius == initRadius)
            {
                Destroy(other.gameObject);
            }
            else
            {
                if(triggerCount) TagChanging(); //여기 아님!
                renderer = other.GetComponent<Renderer>();
                renderer.material.color = new Color(255 / 255f, 25 / 255f, 25 / 255f);
                triggerCount = false;
                StartCoroutine(FallWaiting(other));
            }
        }
        Debug.Log("PersonTileMap OnTriggerExit 끝");
    }
    void TagChanging()
    {
        Transform[] tran = GetComponentsInChildren<Transform>();
        string[] tempString = { "0", "0" };
        Debug.Log("TagChanging");
        foreach(Transform t in tran)
        {
            Debug.Log(transform.GetChild(0));
            /*
            if (t != transform.GetChild(0))
            {
                tempString = t.gameObject.name.Split(split);

                if (t.gameObject.CompareTag("Floor1"))
                {
                    if (int.Parse(tempString[1]) - int.Parse(tempString[0]) == 2)
                    {
                        Debug.Log(tempString[0] + "\t" + tempString[1]);
                        t.gameObject.tag = "Floor7";
                    }
                }
            }
            */
        }
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
    /*
    public void CopyTag()
    {
        Transform[] tran = GetComponentsInChildren<Transform>();
        string[] tempString = new string [2];
        tempString = tran[3].name.Split(split);
        Debug.Log(tempString[0] + "\t" + tempString[1]);
        foreach (Transform t in tran) //효율 계선 필요함
        {
            /*
            if (t!=transform.GetChild(0)) { 
                tempString = t.gameObject.name.Split(split);
                Debug.Log(tempString[0]+"\t"+tempString[1]);
            }
            t.gameObject.tag = tag;
            /*
            if (t.gameObject.CompareTag("Floor1"))
            {
                Debug.Log("if문 걸림");
                if (int.Parse(tempString[1]) - int.Parse(tempString[0]) == 2)
                {
                    Debug.Log("2중 if 걸림");
                    Debug.Log(tempString[0] + "\t" + tempString[1]);
                    t.gameObject.tag = "Floor7";
                }
            }
        }
    }
*/
}
