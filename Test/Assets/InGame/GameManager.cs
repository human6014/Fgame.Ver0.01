using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float[,] myField;
    public int playerNum;
    public void Start()
    {
        myField = new float[,] {{ 1,2,3,4,5,6 },
                               { 5,6,7,8,9,10}};
    }
}
