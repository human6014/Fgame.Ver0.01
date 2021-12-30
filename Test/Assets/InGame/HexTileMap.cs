using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileMap : MonoBehaviour
{
    public GameObject HexTilePrefab;
    public Transform PersonTileMap_transform;
    [SerializeField]
    int mapWidth,  //22 
        mapHeight; //22 (0,0)제외 10타일 길이의 길 생성

    private const float tileXOffset = 1.00725f,
                        tileZOffset = 0.87f;
    string parentTag;
    int count;
    void Start()
    {
        Debug.Log("HexTileMap Start 시작");
        parentTag = transform.parent.tag;
        count = 1;
        CreateHexTileMap();
    }

    void CreateHexTileMap()
    {
        int mapXMin = -mapWidth / 2;
        int mapXMax = mapWidth / 2;

        int mapZMin = -mapHeight / 2;
        int mapZMax = mapHeight / 2;

        Vector3 pos;
        GameObject TempGo;
        for (int x = mapXMin; x < mapXMax; x++)
        {
            for (int z = mapZMin; z < mapZMax; z++)
            {
                TempGo = Instantiate(HexTilePrefab, PersonTileMap_transform);

                if (z % 2 == 0) pos = new Vector3(PersonTileMap_transform.position.x + x * tileXOffset, 0,
                                                  PersonTileMap_transform.position.z + z * tileZOffset);
                else pos =            new Vector3(PersonTileMap_transform.position.x + x * tileXOffset + tileXOffset / 2, 0,
                                                  PersonTileMap_transform.position.z + z * tileZOffset);
                StartCoroutine(SetTileInfo(TempGo, x, z, pos));
            }
        }
    }
    IEnumerator SetTileInfo(GameObject TempGo, int x, int z, Vector3 pos)
    {
        Debug.Log("HexTileMap SetTileInfo 시작");

        TempGo.transform.parent = PersonTileMap_transform;
        TempGo.name = x.ToString() + "," + z.ToString();
        TagChecking(TempGo, x, z);

        yield return new WaitForSeconds(0.000001f);
        TempGo.transform.position = pos;
        Debug.Log("HexTileMap SetTileInfo 끝");
    }
    void TagChecking(GameObject TempGo, int x, int z)
    {
        switch (parentTag)
        {
            case "Floor1" when x == z / 2 && z == count:
                Debug.Log("Floor1");
                TagChanging(TempGo);
                break;
            case "Floor2" when x == (z + 1) / -2 && z == mapHeight / 2 - count + (count % 2 == 0 ? 1 : -1) && z > 0: //입력 값에따라 안될 경우 있음
                Debug.Log("Floor2");
                TagChanging(TempGo);
                break;
            case "Floor3" when x < 0 && z == 0 && x > mapWidth / -2:
                Debug.Log("Floor3");
                TagChanging(TempGo);
                break;
            case "Floor4" when x == (z - 1) / 2 && z == mapHeight / -2 + count && x < 0:
                Debug.Log("Floor4");
                TagChanging(TempGo);
                break;
            case "Floor5" when x == z / -2 && z == -count + (count % 2 == 0 ? 2 : 0) && z >= mapHeight / -2: //더 좋은 식 찾기
                TagChanging(TempGo);
                if (z == mapHeight / -2 | z == 0) TempGo.tag = transform.parent.tag;
                else Debug.Log("Floor5");
                break;
            case "Floor6" when x > 0 && z == 0:
                Debug.Log("Floor6");
                TagChanging(TempGo);
                break;
            default:
                TempGo.tag = transform.parent.tag;
                break;
        }
    }
    void TagChanging(GameObject TempGo)
    {
        TempGo.tag = "Floor7";
        count++;
    }
}
