using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileMap : MonoBehaviour
{
    public GameObject HexTilePrefab;
    public GameObject PersonTileMap_object;
    public PersonTileMap PersonTileMap_script;
    public Transform PersonTileMap_transform;
    public SphereCollider Sphere;
    

    public int mapWidth,
               mapHeight;
    float tileXOffset = 1.00725f,
          tileZOffset = 0.87f;
    float timeSpan;
    void Start()
    {
        Debug.Log("HexTileMap Start 시작");

        Sphere = GetComponent<SphereCollider>();
        CreateHexTileMap();
        timeSpan = 0.0f;
        PersonTileMap_script.CopyTag();
        
        Debug.Log("HexTileMap Start 끝");
    }
    private void FixedUpdate()
    {
        Debug.Log("FixedUpdate 중");

        timeSpan += Time.deltaTime;      
        if (Sphere.radius >0)
        {
            Sphere.radius -= timeSpan / 3000;
        }
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
                GameObject TempGo = Instantiate(HexTilePrefab);
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
        TempGo.transform.position = pos;
        yield return new WaitForSeconds(0.00001f);

        Debug.Log("HexTileMap SetTileInfo 끝");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("HexTileMap OnTriggerExit 시작");

        if (other.tag == this.tag)
            Destroy(other.gameObject);

        Debug.Log("HexTileMap OnTriggerExit 끝");
    }
}
