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
    public Transform [,] childPortal = new Transform[6,2];
    public Transform [,] childSpawner = new Transform[1,6];
    public string [] playerName;
    public float[,] myField;
    public int[] childCount = new int[6];
    public int playerNum;
    public int i, j, bottom, top = 5;
    public int personTileCount;
    int playerCount;
    private void Start()
    {
        playerName = new string[]{ "없음","없음", "없음", "없음", "없음", "없음" };
        myField = new float[,] {{ 1,2,3,4,5,6 },//플레이어 넘버
                               { 8.5f,8.5f,8.5f,8.5f,8.5f,8.5f}};//플레이어 타일 크기
        float x = 1,
              z = 0;
        int tagNum = 1;
        for (int i = 1; i < 7; i++)
        {
            switch (i)
            {
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
            GameObject PersonTile = Instantiate(personTileMap_obj, new Vector3
                (x * personTileMap_script.sphere.radius * 3, 0, z * personTileMap_script.sphere.radius * 3), Quaternion.identity);
            tagNum++;
            PersonTile.transform.parent = transform;
            PersonTile.name = "PerosnTileMap" + (i + 1);
            PersonTile.tag = "Floor" + tagNum;
        }
    }
    private void Update()
    {
        tileCount.text = "남은 타일\n";
        for (int i = 0; i < 6; i++)
        {
            tileCount.text += playerName[i].ToString() + " : " + childCount[i] + "\n";
        }
    }
    public void SetPlayer(string name) => photonView.RPC(nameof(PlayerIn), RpcTarget.AllBuffered, name);
    public void setPlayer() => photonView.RPC(nameof(PlayerOut), RpcTarget.AllBuffered);
    [PunRPC]
    private void PlayerIn(string name)
    {
        playerName[playerCount] = name;
        playerCount++;
    }
    [PunRPC]
    private void PlayerOut()
    {
        playerName[playerCount] = "없음";
        playerCount--;
    }
}
