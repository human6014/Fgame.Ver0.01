using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileMap : MonoBehaviour
{
    public GameObject HexTilePrefab;
    public GameObject PortalPrefab;
    public GameObject CastlePrefab;
    public Transform PersonTileMap_transform;
    public AllTileMap allTileMap;
    [SerializeField]
    int mapWidth,  //22 
        mapHeight; //22 (0,0)제외 10타일 길이의 길 생성

    private const float tileXOffset = 1.00725f,
                        tileZOffset = 0.87f;
    int count = 1;
    public void CreateHexTileMap()
    {
        int mapXMin = -mapWidth / 2;
        int mapXMax = mapWidth / 2;

        int mapZMin = -mapHeight / 2;
        int mapZMax = mapHeight / 2;

        Vector3 pos;
        GameObject TempGo;
        for (float x = mapXMin; x < mapXMax; x++)
        {
            for (float z = mapZMin; z < mapZMax; z++)
            {
                TempGo = Instantiate(HexTilePrefab, PersonTileMap_transform);
                if (z % 2 == 0) pos = new Vector3(PersonTileMap_transform.position.x + x * tileXOffset, 0,
                                                  PersonTileMap_transform.position.z + z * tileZOffset);
                else            pos =  new Vector3(PersonTileMap_transform.position.x + x * tileXOffset + tileXOffset / 2, 0,
                                                  PersonTileMap_transform.position.z + z * tileZOffset);
                StartCoroutine(SetTileInfo(TempGo, x, z, pos));
            }
        }
    }
    IEnumerator SetTileInfo(GameObject TempGo, float x, float z, Vector3 pos)
    {
        TempGo.transform.parent = PersonTileMap_transform;
        TempGo.name = x.ToString() + "," + z.ToString();
        if (x == 0 && z == 0 && !transform.parent.CompareTag("Floor7"))
        {
            GameObject castle = Instantiate(CastlePrefab, new Vector3(x * tileXOffset + PersonTileMap_transform.position.x, 0,
                                                                      z * tileZOffset + PersonTileMap_transform.position.z), Quaternion.identity);
            castle.name = "Spawner" + transform.parent.tag.Substring(5, 1);
            castle.transform.parent = PersonTileMap_transform;
            castle.tag = transform.parent.tag;
            allTileMap.childSpawner[0,int.Parse(transform.parent.tag.Substring(5,1)) - 1] = castle.transform;
        }
        TagChecking(TempGo, x, z);

        yield return null;
        TempGo.transform.position = pos;
    }
    void TagChecking(GameObject TempGo, float x, float z)
    {
        switch (transform.parent.tag)
        {
            case "Floor1" when x == (int)z / 2 && z == count:
                Debug.Log("Floor1");
                TagChanging(TempGo, x, z);
                break;
            case "Floor2" when x == ((int)z + 1) / -2 && z == mapHeight / 2 - count + (count % 2 == 0 ? 1 : -1) && z > 0: //입력 값에따라 안될 경우 있음
                Debug.Log("Floor2");
                TagChanging(TempGo, x, z + 1);
                break;
            case "Floor3" when x < 0 && z == 0 && x > mapWidth / -2:
                Debug.Log("Floor3");
                TagChanging(TempGo, x, z);
                break;
            case "Floor4" when x == ((int)z - 1) / 2 && z == mapHeight / -2 + count && x < 0:
                Debug.Log("Floor4");
                TagChanging(TempGo, x, z);
                break;
            case "Floor5" when x == (int)z / -2 && z == -count + (count % 2 == 0 ? 2 : 0) && z >= mapHeight / -2: //더 좋은 식 찾기
                TagChanging(TempGo, x + 1, z - 2);
                if (z == mapHeight / -2 | z == 0) TempGo.tag = transform.parent.tag;
                else Debug.Log("Floor5");
                break;
            case "Floor6" when x > 0 && z == 0:
                Debug.Log("Floor6");
                TagChanging(TempGo, x, z);
                break;
            default:
                TempGo.tag = transform.parent.tag;
                if (transform.parent.CompareTag("Floor7")) CreatePortal(x, z);
                break;
        }
    }
    void TagChanging(GameObject TempGo, float x, float z)
    {
        TempGo.tag = "Floor7";
        count++;
        if (x >= 0 && count == mapHeight / 2) CreatePortal(x, z);
        else if (x < 0 && count == 2) CreatePortal(x, z);
    }
    void CreatePortal(float x, float z)
    {
        if (transform.parent.CompareTag("Floor7"))
        {
            switch (count)
            {
                case 1 when x == -5 && z == 0:
                    break;
                case 2 when x == -3 && z == -5:
                    x += tileXOffset / 2;
                    break;
                case 3 when x == -3 && z == 5:
                    x += tileXOffset / 2;
                    break;
                case 4 when x == 2 && z == -5:
                    x += tileXOffset / 2;
                    break;
                case 5 when x == 2 && z == 5:
                    x += tileXOffset / 2;
                    break;
                case 6 when x == 5 && z == 0:
                    break;
                default:
                    return;
            }
            count++;
        }
        GameObject Portal = Instantiate(PortalPrefab, new Vector3(x * tileXOffset + PersonTileMap_transform.position.x, 0.5f,
                                                                  z * tileZOffset + PersonTileMap_transform.position.z), Quaternion.identity);
        Portal.transform.parent = PersonTileMap_transform.parent;
        if (allTileMap.j > 0)
        {
            if (allTileMap.i % 2 == 0)
            {
                Portal.name = allTileMap.top + "" + allTileMap.j;
                allTileMap.childPortal[allTileMap.top--, allTileMap.j] = Portal.transform;
            }
            else
            {
                Portal.name = allTileMap.bottom + "" + allTileMap.j;
                allTileMap.childPortal[allTileMap.bottom++, allTileMap.j] = Portal.transform;
            }
        }
        else
        {
            Portal.name = allTileMap.i + "" + allTileMap.j;
            allTileMap.childPortal[allTileMap.i, allTileMap.j] = Portal.transform;
            if (allTileMap.i == 5) allTileMap.j++;
        }
        allTileMap.i++;
    }
}
