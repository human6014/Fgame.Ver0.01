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
        myField = new float[,] {{ 1,2,3,4,5,6 },//�÷��̾� �ѹ�
                               { 5,6,7,8,9,10}};//�÷��̾� Ÿ�� ũ��
    }
    private void Update()
    {
    }
}
