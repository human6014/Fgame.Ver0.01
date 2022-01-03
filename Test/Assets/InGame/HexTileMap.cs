using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileMap : MonoBehaviour
{
    public GameObject HexTilePrefab;
    public GameObject PortalPrefab;
    public Transform PersonTileMap_transform;
    public Transform AllPortal;
    public AllTileMap AllTileMap;
    [SerializeField]
    int mapWidth,  //22 
        mapHeight; //22 (0,0)제외 10타일 길이의 길 생성

    const float tileXOffset = 1.00725f,
                        tileZOffset = 0.87f;
    int count = 1;
    int i, j;
    void Start() => CreateHexTileMap();
    void CreateHexTileMap()
    {
        int mapXMin = -mapWidth / 2;
        int mapXMax =  mapWidth / 2;

        int mapZMin = -mapHeight / 2;
        int mapZMax =  mapHeight / 2;

        Vector3 pos;
        GameObject TempGo;
        for (float x = mapXMin; x < mapXMax; x++)
        {
            for (float z = mapZMin; z < mapZMax; z++)
            {
                TempGo = Instantiate(HexTilePrefab, PersonTileMap_transform);

                if (z % 2 == 0) pos = new Vector3(PersonTileMap_transform.position.x + x * tileXOffset, 0,
                                                  PersonTileMap_transform.position.z + z * tileZOffset);
                else            pos = new Vector3(PersonTileMap_transform.position.x + x * tileXOffset + tileXOffset / 2, 0,
                                                  PersonTileMap_transform.position.z + z * tileZOffset);
                StartCoroutine(SetTileInfo(TempGo, x, z, pos));
            }
        }
    }
    IEnumerator SetTileInfo(GameObject TempGo, float x, float z, Vector3 pos)
    {
        Debug.Log("HexTileMap SetTileInfo 시작");

        TempGo.transform.parent = PersonTileMap_transform;
        TempGo.name = x.ToString() + "," + z.ToString();
        TagChecking(TempGo, x, z);

        yield return new WaitForSeconds(0.000001f);
        TempGo.transform.position = pos;
        Debug.Log("HexTileMap SetTileInfo 끝");
    }
    void TagChecking(GameObject TempGo, float x, float z)
    {
        switch (transform.parent.tag)
        {
            case "Floor1" when x == (int)z / 2 && z == count:
                TagChanging(TempGo, x, z);
                break;
            case "Floor2" when x == ((int)z + 1) / -2 && z == mapHeight / 2 - count + (count % 2 == 0 ? 1 : -1) && z > 0: //입력 값에따라 안될 경우 있음
                TagChanging(TempGo, x, z + 1);
                break;
            case "Floor3" when x < 0 && z == 0 && x > mapWidth / -2:
                TagChanging(TempGo, x, z);
                break;
            case "Floor4" when x == ((int)z - 1) / 2 && z == mapHeight / -2 + count && x < 0:
                TagChanging(TempGo, x, z);
                break;
            case "Floor5" when x == (int)z / -2 && z == -count + (count % 2 == 0 ? 2 : 0) && z >= mapHeight / -2: //더 좋은 식 찾기
                TagChanging(TempGo, x + 1, z - 2);
                if (z == mapHeight / -2 || z == 0) TempGo.tag = transform.parent.tag;
                break;
            case "Floor6" when x > 0 && z == 0:
                TagChanging(TempGo, x, z);
                break;
            default:
                TempGo.tag = transform.parent.tag;
                if (transform.parent.CompareTag("Floor7")) CreatePortal(x,z);
                break;
        }
    }
    void TagChanging(GameObject TempGo, float x, float z)
    {
        TempGo.tag = "Floor7";
        count++;
        if (x >= 0 && count == mapHeight / 2)CreatePortal(x,z);
        else if (x < 0 && count == 2)        CreatePortal(x,z);
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
            Debug.Log(count);
        }
        Debug.Log("Portal Create");
        GameObject Portal = Instantiate(PortalPrefab, new Vector3(x * tileXOffset + PersonTileMap_transform.position.x, 0.5f,
                                                                  z * tileZOffset + PersonTileMap_transform.position.z), Quaternion.identity);
        Portal.transform.parent = AllPortal;
        Portal.name = "Portal " + (int)x + "," + z;
        //AllTileMap.childPortal[i,j] = Portal.transform;
        //i++;
    }
}
