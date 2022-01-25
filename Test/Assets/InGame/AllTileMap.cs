using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class AllTileMap : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject personTileMap;
    [SerializeField] GeneralManager generalManager;
    [SerializeField] Text tileCount;

    private Transform[,] childPortal = new Transform[6, 2];
    private Transform[] childSpawner = new Transform[6];
    private GameObject PersonTile;
    private SphereCollider[] childSphereColliders= new SphereCollider[6];
    private int[] childTileCount = new int[6];

    private bool isCreateTile;
    public int i, j, bottom, top = 5;
    public int catleRotation = 90;

    public float[] myField = new float[6];
    public int playerNum;
    #region Getter + Setter
    public void SetPortal(Transform tr, int i, int j) => childPortal[i, j] = tr;
    public void SetSpawner(Transform tr, int i) => childSpawner[i] = tr;
    public void SetPersonTileRadius(int i, float radius) => childSphereColliders[i].radius = radius;
    public void SetChildTileCount(int i,int count) => childTileCount[i] = count;
    public Transform GetPortal(int i, int j) => childPortal[i, j];
    public Transform GetSpawner(int i) => childSpawner[i];
    public float GetPersonTileRadius(int i) => childSphereColliders[i].radius;
    public bool GetIsCreateTile() => isCreateTile;
    #endregion
    IEnumerator Start()
    {
        myField = new float[] { 8.5f,7.5f,6.5f,5.5f,4.5f,3.5f};//플레이어 타일 크기

        yield return new WaitUntil(()=> generalManager.isFull);
        CreatePersonTile();
        yield return new WaitForSeconds(3);
        generalManager.CreatePlayer();
        isCreateTile = true;
    }
    void CreatePersonTile()
    {
        float x = 0,
              z = 0;
        int tagNum = 0;
        float _radius = personTileMap.GetComponent<SphereCollider>().radius;
        for (int i = 0; i < 7; i++)
        {
            switch (i)
            {
                case 0:
                    x = 0;
                    z = 0;
                    break;
                case 1:
                    x = 1;
                    z = 0;
                    break;
                case 2:
                    x = 1.5f;
                    z = 0.865f;
                    break;
                case 3:
                    x = 1;
                    z = 1.73f;
                    break;
                case 4:
                    x = 0;
                    z = 1.73f;
                    break;
                case 5:
                    x = -0.5f;
                    z = 0.865f;
                    break;
                case 6:
                    x = 0.5f;
                    z = 0.865f;
                    break;
            }
            PersonTile = Instantiate(personTileMap, new Vector3
            (x * _radius * 3, 0, z * _radius * 3), Quaternion.identity);
            tagNum++;
            PersonTile.transform.parent = transform;
            PersonTile.name = "PerosnTileMap" + (i + 1);
            PersonTile.tag = "Floor" + tagNum;
            PersonTile.GetComponent<PersonTileMap>().CreateHexTileMap();
            if (i != 6) childSphereColliders[i] = PersonTile.GetComponent<SphereCollider>();
        }
        Destroy(personTileMap);
    }
    private void Update()
    {
        tileCount.text = "남은 타일\n";
        if (!generalManager.isFull) return;
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            tileCount.text += player.NickName + " : " + childTileCount[player.ActorNumber - 1] + "\n";
        }
    }
}