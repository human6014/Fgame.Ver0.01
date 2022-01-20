using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class AllTileMap : MonoBehaviourPunCallbacks
{
    public GameObject personTileMap_obj;
    public NetworkManager networkManager;
    public PersonTileMap personTileMap_script;
    public Text tileCount;
    private Transform[,] childPortal = new Transform[6, 2];
    private Transform[,] childSpawner = new Transform[1, 6];
    public string[] playerName = new string[6];
    public float[,] myField;
    public int[] childCount = new int[6];
    public int playerNum;
    public int i, j, bottom, top = 5;
    public int personTileCount;
    #region Getter + Setter
    public void SetPortal(Transform tr, int i, int j) => childPortal[i, j] = tr;
    public void SetSpawner(Transform tr, int i, int j) => childSpawner[i, j] = tr;
    public Transform GetPortal(int i, int j)
    {
        return childPortal[i, j];
    }
    public Transform GetSpawner(int i, int j)
    {
        return childSpawner[i, j];
    }
    #endregion
    void Start()
    {
        myField = new float[,] {{ 1,2,3,4,5,6 },//플레이어 넘버
                               { 8.5f,8.5f,8.5f,8.5f,8.5f,8.5f}};//플레이어 타일 크기
        CreatePersonTile();
    }
    void CreatePersonTile()
    {
        GameObject[] PersonTile = new GameObject[7];
        float x = 0,
              z = 0;
        int tagNum = 0;
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
            PersonTile[i] = Instantiate(personTileMap_obj, new Vector3
            (x * personTileMap_script.sphere.radius * 3, 0, z * personTileMap_script.sphere.radius * 3), Quaternion.identity);
            tagNum++;
            PersonTile[i].transform.parent = transform;
            PersonTile[i].name = "PerosnTileMap" + (i + 1);
            PersonTile[i].tag = "Floor" + tagNum;
        }
        for(int i = 0; i < 7; i++)
        {
            PersonTile[i].GetComponent<PersonTileMap>().CreateHexTileMap();
        }
    }
    private void Update()
    {
        tileCount.text = "남은 타일\n";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            tileCount.text += playerName[i].ToString() + " : " + childCount[i] + "\n";
        }
    }
    /*
    public void PlusPlayer(string name) => photonView.RPC(nameof(PlayerIn), RpcTarget.AllBuffered, name);
    //public void MinusPlayer() => photonView.RPC(nameof(PlayerOut), RpcTarget.AllBuffered);
    [PunRPC]
    private void PlayerIn(string name)
    {
        playerName[playerCount] = name;
        playerCount++;
        Debug.Log("playerCount : " + playerCount);
        Debug.Log("Debug.Log(PhotonNetwork.CountOfPlayers) : " + PhotonNetwork.CountOfPlayers);
        Debug.Log("PhotonNetwork.PlayerList.Length : "+PhotonNetwork.PlayerList.Length);
        for(int i=0;i<PhotonNetwork.PlayerList.Length;i++)
            Debug.Log("\nPhotonNetwork.PlayerList : "+PhotonNetwork.PlayerList[i]);
    }
    [PunRPC]
    private void PlayerOut()
    {
        playerName[PhotonNetwork.PlayerList.Length-1] = "없음";
        playerCount--;
        Debug.Log("playerCount : "+playerCount);
        Debug.Log("Debug.Log(PhotonNetwork.CountOfPlayers) : "+PhotonNetwork.CountOfPlayers);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            Debug.Log("\nPhotonNetwork.PlayerList : " + PhotonNetwork.PlayerList[i]);
    }
    */
}