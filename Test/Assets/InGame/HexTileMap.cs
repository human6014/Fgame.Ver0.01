using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileMap : MonoBehaviour
{
    public GameObject HexTilePrefab;
    public Transform PersonTileMap_transform;
    int count;
    [SerializeField]
    int mapWidth,  //18
        mapHeight; //22

    private const float tileXOffset = 1.00725f,
                        tileZOffset = 0.87f;
    string parentName;
    void Start()
    {
        Debug.Log("HexTileMap Start ����");
        parentName = transform.parent.name;
        count = 1;
        CreateHexTileMap();
        Debug.Log("HexTileMap Start ��");
    }

    void CreateHexTileMap()
    {
        int mapXMin = -mapWidth / 2;
        int mapXMax = mapWidth / 2;

        int mapZMin = -mapHeight / 2;
        int mapZMax = mapHeight / 2;

        for (int x = mapXMin; x < mapXMax; x++)
        {
            for (int z = mapZMin; z < mapZMax; z++)
            {
                GameObject TempGo = Instantiate(HexTilePrefab, PersonTileMap_transform);
                TempGo.tag = "Floor0";
                Vector3 pos;

                if (z % 2 == 0) pos = new Vector3(PersonTileMap_transform.position.x + x * tileXOffset, 0, PersonTileMap_transform.position.z + z * tileZOffset);
                else pos = new Vector3(PersonTileMap_transform.position.x + x * tileXOffset + tileXOffset / 2, 0, PersonTileMap_transform.position.z + z * tileZOffset);
                StartCoroutine(SetTileInfo(TempGo, x, z, pos));
            }
        }
    }
    IEnumerator SetTileInfo(GameObject TempGo, int x, int z, Vector3 pos)
    {
        Debug.Log("HexTileMap SetTileInfo ����");

        TempGo.transform.parent = PersonTileMap_transform;
        TempGo.name = x.ToString() + "," + z.ToString();
        TagChecking(TempGo, x, z);

        yield return new WaitForSeconds(0.000001f);
        TempGo.transform.position = pos;
        Debug.Log("HexTileMap SetTileInfo ��");
    }
    void TagChecking(GameObject TempGo, int x, int z) //�̿ϼ�
    {
        //Debug.Log(parentName+"�� count : "+count);
        switch (parentName)
        {
            case "PersonTileMap1" when (x == z / 2 && z == count):
                Debug.Log("PersonTileMap1�尨");
                TagChanging(TempGo);
                break;
            case "PersonTileMap2":
                Debug.Log("PersonTileMap2�尨");
                break;
            case "PersonTileMap3" when (x == -count && z == 0):
                TagChanging(TempGo);
                Debug.Log("PersonTileMap3�尨");
                break;
            case "PersonTileMap4":
                Debug.Log("PersonTileMap4�尨");
                break;
            case "PersonTileMap5":
                Debug.Log("PersonTileMap5�尨");
                break;
            case "PersonTileMap6" when (x == count && z == 0):
                Debug.Log("PersonTileMap6�尨");
                TagChanging(TempGo);
                break;
            default:
                Debug.Log(parentName);
                TempGo.tag = transform.parent.tag;
                break;
        }
        /*
        if (z == count && z / 2 == x && TempGo.transform.parent.name=="PersonTileMap1")
        {
            Debug.Log(x + "," + z);
            TempGo.tag = "Floor7";
            count++;
        }
        else TempGo.tag = transform.parent.tag;
        */
    }
    void TagChanging(GameObject TempGo)
    {
        TempGo.tag = "Floor7";
        count++;
    }
}
