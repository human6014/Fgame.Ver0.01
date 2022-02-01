using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PersonTileMap : MonoBehaviour
{
    public GeneralManager generalManager;
    public AllTileMap allTileMap;
    private SphereCollider sphereCollider;
    private MeshCollider meshCollider;
    private new Rigidbody rigidbody;
    private new Renderer renderer;
    private float initRadius;
    private bool tryOnce;
    [SerializeField] GameObject HexTilePrefab;
    [SerializeField] GameObject PortalPrefab;
    [SerializeField] GameObject CastlePrefab;
    [SerializeField] int mapWidth,  //22 
                         mapHeight;
    private const float tileXOffset = 1.00725f,
                        tileZOffset = 0.87f,
                        tileRadius  = 8.5f;
    int myIndex;
    int count = 1;
    void Start()
    {
        if (gameObject.name == "Sample") return;
        sphereCollider = GetComponent<SphereCollider>();
        if (gameObject.CompareTag("Floor7")) sphereCollider.radius = 5;
        //else sphereCollider.radius = tileRadius;
        else
        {
            sphereCollider.radius = allTileMap.myField[allTileMap.playerNum++];
            myIndex = int.Parse(transform.tag.Substring(5));
        }
        //Debug.Log(myIndex);
        initRadius = sphereCollider.radius - 0.05f;
    }
    private void Update()
    {
        if (gameObject.CompareTag("Floor7") || gameObject.name == "Sample") return;
        allTileMap.SetChildTileCount(int.Parse(name.Substring(13, 1)) - 1, transform.childCount); //위치 수정 보류
        if (sphereCollider.radius >= 0)
        {
            if (generalManager.GetIsCreatePlayer())
            {
                //if (allTileMap.GetPersonTileRadius(myIndex) > 0)
                {

                }
                //else
                {

                }
                sphereCollider.radius -= Time.deltaTime * Time.time / 1000;
            }
        }
        else if (!tryOnce)
        {
            Transform[] child = GetComponentsInChildren<Transform>();

            foreach (Transform iter in child)
            {
                if (iter != transform) StartCoroutine(FallWaiting(iter.gameObject));
            }
            for (int i = 0; i < 2; i++) Destroy(allTileMap.GetPortal(int.Parse(transform.name.Substring(13, 1)) - 1, i).gameObject);
            tryOnce = true;
        }
    }
    #region 블럭 파괴
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tag))
        {
            if (sphereCollider.radius > initRadius) Destroy(other.gameObject);
            else StartCoroutine(FallWaiting(other.gameObject));
        }
    }
    #endregion
    #region 블럭 떨어짐
    IEnumerator FallWaiting(GameObject other)
    {
        renderer = other.GetComponent<Renderer>();
        renderer.material.color = new Color(255 / 255f, 25 / 255f, 25 / 255f);
        yield return new WaitForSeconds(3f); //밑에 순서 중요함
        if (!other) yield break;
        meshCollider = other.GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        rigidbody = other.GetComponent<Rigidbody>();
        rigidbody.mass = 0.5f;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }
    #endregion
    #region 블럭 생성
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
    #endregion
    #region 블럭 설정 + 스포너 생성
    IEnumerator SetTileInfo(GameObject TempGo, float x, float z, Vector3 pos)
    {
        TempGo.transform.parent = transform;
        TempGo.name = x.ToString() + "," + z.ToString();
        if (x == 0 && z == 0 && !transform.CompareTag("Floor7"))
        {
            GameObject castle = Instantiate(CastlePrefab, new Vector3(x * tileXOffset + transform.position.x, 0,
                                                                      z * tileZOffset + transform.position.z), Quaternion.identity);
            allTileMap.spawnerRotation -= 60;
            castle.transform.rotation = Quaternion.Euler(0, allTileMap.spawnerRotation, 0);
            castle.name = "Spawner" + transform.tag.Substring(5, 1);
            castle.transform.parent = transform;
            castle.tag = transform.tag;
            allTileMap.SetSpawner(castle.transform, int.Parse(transform.tag.Substring(5, 1)) - 1);
        }
        TagChecking(TempGo, x, z);

        yield return new WaitForSeconds(0.000001f);
        TempGo.transform.position = pos;
    }
    #endregion
    #region 포탈방향 블럭 설정
    void TagChecking(GameObject TempGo, float x, float z)
    {
        switch (transform.tag)
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
                if (z == mapHeight / -2 | z == 0) TempGo.tag = transform.tag;
                else Debug.Log("Floor5");
                break;
            case "Floor6" when x > 0 && z == 0:
                Debug.Log("Floor6");
                TagChanging(TempGo, x, z);
                break;
            default:
                TempGo.tag = transform.tag;
                if (transform.CompareTag("Floor7")) CreatePortal(x, z);
                break;
        }
    }
    #endregion
    #region 블럭 태그 설정+포탈
    void TagChanging(GameObject TempGo, float x, float z)
    {
        TempGo.tag = "Floor7";
        count++;
        if (x >= 0 && count == mapHeight / 2) CreatePortal(x, z);
        else if (x < 0 && count == 2) CreatePortal(x, z);
    }
    #endregion
    #region 포탈 생성
    void CreatePortal(float x, float z)
    {
        if (transform.CompareTag("Floor7"))
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
        GameObject Portal = Instantiate(PortalPrefab, new Vector3(x * tileXOffset + transform.position.x, 0.5f,
                                                                  z * tileZOffset + transform.position.z), Quaternion.identity);
        Portal.transform.parent = transform.parent;
        if (allTileMap.j > 0)
        {
            if (allTileMap.i % 2 == 0)
            {
                Portal.name = allTileMap.top + "" + allTileMap.j;
                allTileMap.SetPortal(Portal.transform, allTileMap.top--, allTileMap.j);
            }
            else
            {
                Portal.name = allTileMap.bottom + "" + allTileMap.j;
                allTileMap.SetPortal(Portal.transform, allTileMap.bottom++, allTileMap.j);
            }
        }
        else
        {
            Portal.name = allTileMap.i + "" + allTileMap.j;
            allTileMap.SetPortal(Portal.transform, allTileMap.i, allTileMap.j);
            if (allTileMap.i == 5) allTileMap.j++;
        }
        allTileMap.i++;
    }
    #endregion
}