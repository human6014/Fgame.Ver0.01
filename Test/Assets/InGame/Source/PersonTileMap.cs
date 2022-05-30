using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class PersonTileMap : MonoBehaviour
{
    [SerializeField] GeneralManager generalManager;
    [SerializeField] AllTileMap allTileMap;
    [SerializeField]
    GameObject HexTilePrefab,
               PortalPrefab,
               CastlePrefab;
    [SerializeField]
    int mapWidth,  //22 
        mapHeight;
    private SphereCollider sphereCollider;
    private MeshCollider meshCollider;
    private new Rigidbody rigidbody;
    private new Renderer renderer;
    private bool tryOnce;
    private bool isClosedEnd;
    private const float tileXOffset = 1.00725f,
                        tileZOffset = 0.87f,
                        tileRadius = 8.5f,
                        tileSpeed = 0.0005f; //0.0005f == 분당 radius 1.5줄음
    private int myIndex;
    private int count = 1;
    private void Start()
    {
        if (gameObject.name == "Sample") return;
        sphereCollider = GetComponent<SphereCollider>();
        if (gameObject.CompareTag("Floor0")) sphereCollider.radius = 5;
        else
        {
            myIndex = int.Parse(transform.tag.Substring(5));
            sphereCollider.radius = tileRadius;
        }
    }
    private void FixedUpdate()
    {
        if (generalManager.GetIsGameEnd()) return;
        if (gameObject.CompareTag("Floor0") || gameObject.name == "Sample") return;

        allTileMap.SetChildTileCount(int.Parse(name.Substring(13, 1)) - 1, transform.childCount); //위치 수정 보류
        if (sphereCollider.radius >= 0)
        {
            if (generalManager.GetIsCreatePlayer())
            {
                if (allTileMap.GetHasTileNum(myIndex - 1) > 0) allTileMap.SetMinusHasTileNum(myIndex - 1);
                //else sphereCollider.radius -= 0.003f;
                else sphereCollider.radius -= tileSpeed;
            }
            if (!isClosedEnd && sphereCollider.radius <= 4)
            {
                isClosedEnd = true;
                generalManager.SetIsClosedEnd(); //버그 있음
            }
        }
        else if (!tryOnce)
        {
            Transform[] _child = GetComponentsInChildren<Transform>();

            foreach (Transform iter in _child)
            {
                if (iter != transform) StartCoroutine(FallWaiting(iter.gameObject));
            }
            for (int i = 0; i < 2; i++) Destroy(allTileMap.GetPortal(int.Parse(transform.name.Substring(13, 1)) - 1, i).gameObject);
            tryOnce = true;
        }
        if (transform.childCount == 0) allTileMap.SetIsOutPlayer(true, myIndex - 1);
    }
    #region 블럭 파괴
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tag))
        {
            if (!generalManager.GetIsCreatePlayer()) Destroy(other.gameObject);
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

        Vector3 _pos;
        GameObject TempGo;
        for (float x = mapXMin; x < mapXMax; x++)
        {
            for (float z = mapZMin; z < mapZMax; z++)
            {
                TempGo = Instantiate(HexTilePrefab, transform);
                if (z % 2 == 0) _pos = new Vector3(transform.position.x + x * tileXOffset, 0,
                                                   transform.position.z + z * tileZOffset);
                else _pos = new Vector3(transform.position.x + x * tileXOffset + tileXOffset / 2, 0,
                                        transform.position.z + z * tileZOffset);
                StartCoroutine(SetTileInfo(TempGo, x, z, _pos));
            }
        }
    }
    #endregion
    #region 블럭 설정 + 스포너 생성
    private IEnumerator SetTileInfo(GameObject TempGo, float x, float z, Vector3 pos)
    {
        TempGo.transform.parent = transform;
        TempGo.name = x.ToString() + "," + z.ToString();
        if (x == 0 && z == 0 && !transform.CompareTag("Floor0"))
        {
            GameObject castle = Instantiate(CastlePrefab, new Vector3(x * tileXOffset + transform.position.x, 0,
                                                                      z * tileZOffset + transform.position.z), Quaternion.identity);
            allTileMap.spawnerRotation -= 60;
            castle.transform.rotation = Quaternion.Euler(0, allTileMap.spawnerRotation, 0);
            castle.name = "Spawner" + transform.tag.Substring(5, 1);
            castle.transform.parent = transform;
            castle.tag = transform.tag;
            allTileMap.SetSpawner(castle.transform, int.Parse(transform.tag.Substring(5)) - 1);
        }
        TagChecking(TempGo, x, z);
        yield return new WaitForFixedUpdate();
        TempGo.transform.position = pos;
    }
    #endregion
    #region 포탈방향 블럭 설정
    private void TagChecking(GameObject TempGo, float x, float z)
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
            case "Floor5" when x == (int)z / -2 && z == -count + (count % 2 == 0 ? 2 : 0) && z >= mapHeight / -2: //동작 원리 혼자만 특이함
                TagChanging(TempGo, x + 1, z - 2);
                if (z == mapHeight / -2) TempGo.tag = transform.tag;
                break;
            case "Floor6" when x > 0 && z == 0:
                TagChanging(TempGo, x, z);
                break;
            default:
                if (x == 0 && z == 0)
                {
                    TempGo.tag = "Floor0";
                    TempGo.layer = 7;
                }
                else TempGo.tag = transform.tag;
                if (transform.CompareTag("Floor0"))
                {
                    TempGo.layer = 7;
                    CreatePortal(x, z);
                }
                break;
        }
    }
    #endregion
    #region 블럭 태그 설정+포탈
    private void TagChanging(GameObject TempGo, float x, float z)
    {
        TempGo.tag = "Floor0";
        TempGo.layer = 7;
        count++;
        if (x >= 0 && count == mapHeight / 2) CreatePortal(x, z);
        else if (x < 0 && count == 2) CreatePortal(x, z);
    }
    #endregion
    #region 포탈 생성
    private void CreatePortal(float x, float z)
    {
        if (transform.CompareTag("Floor0"))
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
            Portal.name = allTileMap.i - 6 + "" + allTileMap.j;
            allTileMap.SetPortal(Portal.transform, allTileMap.i - 6, allTileMap.j);
        }
        else
        {
            int myPosition;
            if (allTileMap.i % 2 == 0)
            {
                myPosition = allTileMap.tempTop;
                allTileMap.tempTop--;
            }
            else
            {
                myPosition = allTileMap.tempBottom;
                allTileMap.tempBottom++;
            }
            if (myPosition > PhotonNetwork.CurrentRoom.MaxPlayers - 1)
            {
                Destroy(Portal);
            }
            else
            {
                Portal.name = myPosition + "" + allTileMap.j;
                allTileMap.SetPortal(Portal.transform, myPosition, allTileMap.j);
            }
            if (allTileMap.i == 5) allTileMap.j++;
        }
        allTileMap.i++;
    }
    #endregion
}