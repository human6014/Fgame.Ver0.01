using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileMap : MonoBehaviour
{
    public GameObject HexTilePrefab;
    public Transform Holder;

    [SerializeField] int mapWidth,
                         mapHeight;

    float tileXOffset = 1.00725f,
          tileZOffset = 0.87f,
          timespan = 0.0f;
    void Start()
    {
        CreateHexTileMap();
    }
    private void Update()
    {
        timespan += Time.deltaTime;
        if (timespan == 1)
        {
            Debug.Log(timespan);
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
                TempGo.tag = "Floor";
                Vector3 pos;

                if (z % 2 == 0)
                {
                    pos = new Vector3(x * tileXOffset, 0, z * tileZOffset);
                }
                else
                {
                    pos = new Vector3(x * tileXOffset + tileXOffset / 2, 0, z * tileZOffset);
                }

                StartCoroutine(SetTileInfo(TempGo, x, z, pos));
            }
        }
    }
    IEnumerator SetTileInfo(GameObject Go, float x, float z, Vector3 pos)
    {
        yield return new WaitForSeconds(0.00001f);
        Go.transform.parent = Holder;
        Go.name = x.ToString() + "," + z.ToString();
        Go.transform.position = pos;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Floor")
            Destroy(other.gameObject);
    }
}
