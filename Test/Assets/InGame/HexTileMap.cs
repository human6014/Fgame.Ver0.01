using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileMap : MonoBehaviour
{
    public GameObject HexTilePrefab;
    public PersonTileMap PersonTileMap_script;
    public Transform PersonTileMap_transform;

    [SerializeField] int mapWidth,
                         mapHeight;

    private const float tileXOffset = 1.00725f,
                        tileZOffset = 0.87f;
    void Start()
    {
        Debug.Log("HexTileMap Start 시작");

        CreateHexTileMap();
        PersonTileMap_script.CopyTag();
        
        Debug.Log("HexTileMap Start 끝");
    }

    void CreateHexTileMap()
    {
        float mapXMin = -mapWidth / 2;
        float mapXMax = mapWidth / 2;

        float mapZMin = -mapHeight / 2;
        float mapZMax = mapHeight / 2;

        for (float x = mapXMin; x < mapXMax; x++)
        {
            for (float z = mapZMin; z < mapZMax; z++)
            {
                GameObject TempGo = Instantiate(HexTilePrefab,PersonTileMap_transform) as GameObject;
                TempGo.tag = "Floor0";
                Vector3 pos;

                if (z % 2 == 0)
                {
                    pos = new Vector3(PersonTileMap_transform.position.x+x * tileXOffset, 0, PersonTileMap_transform.position.z+z * tileZOffset);
                }
                else
                {
                    pos = new Vector3(PersonTileMap_transform.position.x+x * tileXOffset + tileXOffset / 2, 0, PersonTileMap_transform.position.z + z * tileZOffset);
                }
                StartCoroutine(SetTileInfo(TempGo, x, z, pos));
            }
        }
    }
    IEnumerator SetTileInfo(GameObject TempGo, float x, float z, Vector3 pos)
    {
        Debug.Log("HexTileMap SetTileInfo 시작");
        
        TempGo.transform.parent = PersonTileMap_transform;
        TempGo.name = x.ToString() + "," + z.ToString();
        yield return new WaitForSeconds(0.000001f);
        TempGo.transform.position = pos;

        Debug.Log("HexTileMap SetTileInfo 끝");
    }
}
