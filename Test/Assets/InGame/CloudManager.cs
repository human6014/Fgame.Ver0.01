using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField] Transform fog;
    [SerializeField] GameObject[] cloud = new GameObject[6];

    const int numOfCloud = 20;
    int startX, endX;
    int startY, endY;
    int startZ, endZ;
    public float GetFogStartPositionX() => startX;
    public float GetFogEndPositionX() => endX;
    public int GetRandomY() => Random.Range(startY, endY);
    public int GetRandomZ() => Random.Range(startZ, endZ);
    void Start()
    {
        startX = (int)fog.position.x - (int)fog.localScale.x * 5;
        startY = (int)fog.position.y - (int)fog.localScale.y / 2 + 5;
        startZ = (int)fog.position.z - (int)fog.localScale.z * 5;

        endX = (int)fog.position.x + (int)fog.localScale.x * 5;
        endY = (int)fog.position.y - (int)fog.localScale.y / 6;
        endZ = (int)fog.position.z + (int)fog.localScale.z * 5;

        Debug.Log("startX : " + startX + " endX : " + endX);
        Debug.Log("startY : " + startY + " endY : " + endY);
        Debug.Log("startZ : " + startZ + " endZ : " + endZ);

        int cloudIndex;
        int randomX, randomY, randomZ;

        for(int i = 0; i < numOfCloud; i++)
        {
            cloudIndex = Random.Range(0,6);
            randomX = Random.Range(startX, endX);
            randomY = GetRandomY();
            randomZ = GetRandomZ();
            GameObject tempCloud = Instantiate(cloud[cloudIndex], new Vector3(randomX, randomY, randomZ), Quaternion.Euler(0,90,0));
            tempCloud.transform.parent = transform;
        }
    }
}
