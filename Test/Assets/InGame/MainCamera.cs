using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;

public class MainCamera : MonoBehaviour
{
    [SerializeField] Transform playersPool;
    Transform [] players = new Transform[6];
    AllTileMap allTileMap;
    Transform target;
    Vector3 offset = new Vector3(0,4,-2);

    private bool start;
    private bool isEnd;
    private int myNum;
    private int maxPlayer;
    KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6
    };
    public void SetTarget(Transform _target) => target = _target;
    private void Start()
    {
        allTileMap = FindObjectOfType<AllTileMap>();
        maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        myNum = PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1;
    }
    public void SetPlayers()
    {
        foreach (Transform child in playersPool)
            players[int.Parse(child.name.Substring(child.name.Length - 1)) - 1] = child;
        start = true;
    }
    //아웃된 플레이어가 다른 플레이어 보는 동안 그 플레이어 아웃 시 에러 발생
    private void Update()
    {
        if (!start) return;
        if (allTileMap.GetIsOutPlayer(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1))
        {
            for (int i = 0; i < maxPlayer; i++)
            {
                if (Input.GetKeyDown(keyCodes[i]))
                {
                    if (!players[i]) break;
                    target = players[i].transform;
                    break;
                }
            }
        }
        if (!target) return;
        transform.position = target.position + offset;
    }
}