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
    [SerializeField] Text hasTileCount;
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
    public void SetPersonTileRadius(int i, float _radius) => childSphereColliders[i].radius = _radius;
    public void SetChildTileCount(int i,int _count) => childTileCount[i] = _count;
    public void SetHasTileNum(int i, int j) => hasTileNum[i] = j * 1000;
    public void SetPlusHasTileNum(int i) => hasTileNum[i] += 1000;
    public void SetMinusHasTileNum(int i) => hasTileNum[i] -= 10;
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
        GameObject _personTile;
        float _x = 0,
              _z = 0;
        int _tagNum = 0;
        float _radius = personTileMap.GetComponent<SphereCollider>().radius;
        for (int i = 0; i < 7; i++)
        {
            switch (i)
            {
                case 0:
                    _x = 0;
                    _z = 0;
                    break;
                case 1:
                    _x = 1;
                    _z = 0;
                    break;
                case 2:
                    _x = 1.5f;
                    _z = 0.865f;
                    break;
                case 3:
                    _x = 1;
                    _z = 1.73f;
                    break;
                case 4:
                    _x = 0;
                    _z = 1.73f;
                    break;
                case 5:
                    _x = -0.5f;
                    _z = 0.865f;
                    break;
                case 6:
                    _x = 0.5f;
                    _z = 0.865f;
                    break;
            }
            _personTile = Instantiate(personTileMap, new Vector3
            (_x * _radius * 3, 0, _z * _radius * 3), Quaternion.identity);
            _tagNum++;
            _personTile.transform.parent = transform;
            _personTile.name = "PerosnTileMap" + (i + 1);
            _personTile.tag = "Floor" + _tagNum;
            _personTile.GetComponent<PersonTileMap>().CreateHexTileMap();
            if (i != 6) childSphereColliders[i] = _personTile.GetComponent<SphereCollider>();
        }
        Destroy(personTileMap);
    }
    private void Update()
    {
        if (!generalManager.GetIsCreatePlayer()) return;
        tileCount.text = "남은 타일\n";
        hasTileCount.text = "가진 타일\n";
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetPlayerNumber() == -1) return;
            tileCount.text += (player.IsLocal?"<color=red>": "<color=black>")+player.NickName + "</color> : " + childTileCount[player.GetPlayerNumber() - 1] + "\n";
            hasTileCount.text += player.NickName + " : " + hasTileNum[player.GetPlayerNumber() - 1] + "\n";
        }
    }
}