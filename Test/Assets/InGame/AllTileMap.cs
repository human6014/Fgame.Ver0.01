using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class AllTileMap : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject beforeStartCanvas;
    [SerializeField] GameObject personTileMap;
    [SerializeField] GeneralManager generalManager;
    [SerializeField] Text tileCount;
    [SerializeField] Text startTimer;
    private GameObject PersonTile;
    private Transform[,] childPortal = new Transform[6, 2];
    private Transform[] childSpawner = new Transform[6];
    private SphereCollider[] childSphereColliders= new SphereCollider[6];
    private int[] childTileCount = new int[6];
    private int[] hasTileNum = new int[6];
    private bool[] isOutPlayer = new bool[6];

    public int i, j, bottom, top = 5;
    public int spawnerRotation = 90;

    public float[] myField = new float[6]; //임시용
    public int playerNum;                  //임시용
    #region Getter + Setter
    public void SetPortal(Transform tr, int i, int j) => childPortal[i, j] = tr;
    public void SetSpawner(Transform tr, int i) => childSpawner[i] = tr;
    public void SetPersonTileRadius(int i, float radius) => childSphereColliders[i].radius = radius;
    public void SetChildTileCount(int i,int count) => childTileCount[i] = count;
    public void SetHasTileNum(int i, int j) => hasTileNum[i] = j;
    public void SetIsOutPlayer(bool _isOutPlayer, int i) => isOutPlayer[i] = _isOutPlayer;
    public Transform GetPortal(int i, int j) => childPortal[i, j];
    public Transform GetSpawner(int i) => childSpawner[i];
    public float GetPersonTileRadius(int i) => childSphereColliders[i].radius;
    public int GetHasTileNum(int i) => hasTileNum[i];
    public bool GetIsOutPlayer(int i) => isOutPlayer[i];
    
    #endregion
    IEnumerator Start()
    {
        myField = new float[] { 8.5f,7.5f,6.5f,5.5f,4.5f,3.5f};//플레이어 타일 크기

        yield return new WaitUntil(()=> generalManager.GetIsRoomFull());
        CreatePersonTile();
        generalManager.SetIsCreateTile(true);
        //generalManager.SetIsCreateTile(true);
        for (int i = 3; i >= 1; i--)
        {
            startTimer.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        startTimer.text = "! 시작 !";
        generalManager.CreatePlayer();
        generalManager.SetIsCreatePlayer(true);
        yield return new WaitForSeconds(0.5f);
        beforeStartCanvas.SetActive(false);
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
        if (!generalManager.GetIsCreatePlayer()) return;
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            tileCount.text += player.NickName + " : " + childTileCount[player.GetPlayerNumber() - 1] + "\n";
        }
    }
}