using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float[,] myField;
    public int[] tileCount;
    public int playerNum;
    [SerializeField] GameObject AllTileMap;
    private void Start()
    {
        myField = new float[,] {{ 1,2,3,4,5,6 },//플레이어 넘버
                               { 8.5f,8.5f,8.5f,8.5f,8.5f,8.5f}};//플레이어 타일 크기
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Substring(0, 5) == "Floor") Destroy(other.gameObject);
    }
}
