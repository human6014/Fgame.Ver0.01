using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllTileMap : MonoBehaviour
{
    public GameObject personTileMap_obj;
    public PersonTileMap personTileMap_script;
    public Transform [,] childPortal = new Transform[6,2];
    public int[] childCount = new int[6];
    public int i, j, bottom, top = 5;
    public int personTileCount;
    private int tagNum = 1;
    private void Start()
    {
        float x = 1,
              z = 0;
        for (int i = 1; i < 7; i++)
        {
            switch (i)
            {
                case 2:
                    x = 1.5f;
                    z = 0.865f;
                    break;
                case 3:
                    x = 1;
                    z = 1.73f;
                    break;
                case 4:
                    x = 0;
                    z = 1.73f;
                    break;
                case 5:
                    x = -0.5f;
                    z = 0.865f;
                    break;
                case 6:
                    x = 0.5f;
                    z = 0.865f;
                    break;
            }
            GameObject PersonTile = Instantiate(personTileMap_obj, new Vector3
                (x * personTileMap_script.sphere.radius * 3, 0, z * personTileMap_script.sphere.radius * 3), Quaternion.identity);
            tagNum++;
            PersonTile.transform.parent = transform;
            PersonTile.name = "PerosnTileMap" + (i + 1);
            PersonTile.tag = "Floor" + tagNum;
        }
    }
    private void Update()
    {
        for(int i = 0; i < 6; i++)
        {
            Debug.Log("PersonTileMap"+i+": "+childCount[i]);
        }
    }
}
