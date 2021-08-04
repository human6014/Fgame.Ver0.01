using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllTileMap : MonoBehaviour
{
    public GameObject PersonTileMap_obj;
    public PersonTileMap PersonTileMap_script;
    public Transform AllTileMap_Position;
    private int TagNum=1;
    private void Start()
    {
        Debug.Log("AllTileMap Start Ω√¿€");

        float x, z;
        for (int i = 1; i < 6; i++)
        {
            if (i == 1)
            {
                x = 1;
                z = 0;
            }
            else if (i == 2)
            {
                x = 1.5f;
                z = 0.865f;
            }
            else if (i == 3)
            {
                x = 1;
                z = 1.73f;
            }
            else if (i == 4)
            {
                x = 0;
                z = 1.73f;
            }
            else
            {
                x = -0.5f;
                z = 0.865f;
            }
            GameObject AllTile = Instantiate(PersonTileMap_obj, new Vector3
                (x* PersonTileMap_script.Sphere.radius*3, 0, z * PersonTileMap_script.Sphere.radius*3),Quaternion.identity);
            AllTile.transform.parent = AllTileMap_Position;
            TagNum++;
            AllTile.tag = "Floor" + TagNum.ToString();

        }

        Debug.Log("AllTileMap Start ≥°");
    }
}
