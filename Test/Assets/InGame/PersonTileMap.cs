using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PersonTileMap : MonoBehaviour
{
    private AllTileMap allTileMap;
    //public HexTileMap hexTileMap;
    public SphereCollider sphere;
    private MeshCollider meshCollider;
    private new Rigidbody rigidbody;
    private new Renderer renderer;
    float initRadius;
    bool outPlayer;


    [SerializeField]
    int mapWidth,  //22 
        mapHeight; //22 (0,0)제외 10타일 길이의 길 생성

    private const float tileXOffset = 1.00725f,
                        tileZOffset = 0.87f;
    int count = 1;
    public GameObject HexTilePrefab;
    public GameObject PortalPrefab;
    public GameObject CastlePrefab;
    private void Start()
    {
        allTileMap = transform.parent.GetComponent<AllTileMap>();
        sphere = GetComponent<SphereCollider>();
        if (gameObject.CompareTag("Floor7")) sphere.radius = 5;
        else sphere.radius = 8.5f;//allTileMap.myField[1, allTileMap.playerNum];
        allTileMap.playerNum++;
        initRadius = sphere.radius - 0.1f;
        Debug.Log("PersonTileMap Start");
        StartCoroutine(nameof(Comp));
        
    }
    IEnumerator Comp()
    {
        yield return new WaitUntil(() => allTileMap.comp);
        CreateHexTileMap();
    }
    private void Update()
    {
        if (gameObject.CompareTag("Floor7")) return;
        allTileMap.childCount[int.Parse(transform.name.Substring(13, 1)) - 1] = transform.childCount - 1; //위치 수정 보류
        if (sphere.radius >= 0)
        {
             sphere.radius -= Time.deltaTime * Time.time / 1000;
        }
        else if (!outPlayer)
        {
            Transform[] child = GetComponentsInChildren<Transform>();

            foreach (Transform iter in child)
            {
                if (iter.name != "HexTileMap" && iter != transform)
                    StartCoroutine(FallWaiting(iter.gameObject));
            }
            for (int i = 0; i < 2; i++) Destroy(allTileMap.childPortal[int.Parse(transform.name.Substring(13, 1)) - 1, i].gameObject);
            outPlayer = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tag))
        {
            Debug.Log("TriggerExit");
            if (sphere.radius > initRadius) Destroy(other.gameObject);
            else
            {
                StartCoroutine(FallWaiting(other.gameObject));
            }
        }
    }
    IEnumerator FallWaiting(GameObject other)
    {
        renderer = other.GetComponent<Renderer>();
        renderer.material.color = new Color(255 / 255f, 25 / 255f, 25 / 255f);
        yield return new WaitForSeconds(3f); //밑에 순서 중요함, 문제 발견
        if (!other) yield break;
        meshCollider = other.GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        rigidbody = other.GetComponent<Rigidbody>();
        rigidbody.mass = 0.5f;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }


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
                TempGo = Instantiate(HexTilePrefab, transform);
                if (z % 2 == 0) pos = new Vector3(transform.position.x + x * tileXOffset, 0,
                                                  transform.position.z + z * tileZOffset);
                else pos = new Vector3(transform.position.x + x * tileXOffset + tileXOffset / 2, 0,
                                       transform.position.z + z * tileZOffset);
                StartCoroutine(SetTileInfo(TempGo, x, z, pos));
            }
        }
    }
    IEnumerator SetTileInfo(GameObject TempGo, float x, float z, Vector3 pos)
    {
        TempGo.transform.parent = transform;
        TempGo.name = x.ToString() + "," + z.ToString();
        if (x == 0 && z == 0 && !transform.CompareTag("Floor7"))
        {
            GameObject castle = Instantiate(CastlePrefab, new Vector3(x * tileXOffset + transform.position.x, 0,
                                                                      z * tileZOffset + transform.position.z), Quaternion.identity);
            castle.name = "Spawner" + transform.tag.Substring(5, 1);
            castle.transform.parent = transform;
            castle.tag = transform.tag;
            allTileMap.childSpawner[0, int.Parse(transform.tag.Substring(5, 1)) - 1] = castle.transform;
        }
        TagChecking(TempGo, x, z);

        yield return null;
        TempGo.transform.position = pos;
    }
    void TagChecking(GameObject TempGo, float x, float z)
    {
        switch (transform.tag)
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
                if (z == mapHeight / -2 | z == 0) TempGo.tag = transform.parent.tag;
                break;
            case "Floor6" when x > 0 && z == 0:
                TagChanging(TempGo, x, z);
                break;
            default:
                TempGo.tag = transform.tag;
                if (transform.CompareTag("Floor7")) CreatePortal(x, z);
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
        if (transform.CompareTag("Floor7"))
        {
            switch (count)
            {
                case 1 when x == -5 && z == 0:
                    Debug.Log("1");
                    break;
                case 2 when x == -3 && z == -5:
                    x += tileXOffset / 2;
                    Debug.Log("2");
                    break;
                case 3 when x == -3 && z == 5:
                    x += tileXOffset / 2;
                    Debug.Log("3");
                    break;
                case 4 when x == 2 && z == -5:
                    x += tileXOffset / 2;
                    Debug.Log("4");
                    break;
                case 5 when x == 2 && z == 5:
                    x += tileXOffset / 2;
                    Debug.Log("5");
                    break;
                case 6 when x == 5 && z == 0:
                    Debug.Log("6");
                    break;
                default:
                    return;
            }
            count++;
        }
        GameObject Portal = Instantiate(PortalPrefab, new Vector3(x * tileXOffset + transform.position.x, 0.5f,
                                                                  z * tileZOffset + transform.position.z), Quaternion.identity);
        Portal.transform.parent = transform;
        if (allTileMap.j > 0)
        {
            if (allTileMap.i % 2 == 0)
            {
                Debug.Log(allTileMap.i);
                Debug.Log(allTileMap.j);
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
