using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class AllTileMap : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject beforeStartPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject personTileMap;
    [SerializeField] GeneralManager generalManager;
    [SerializeField] Text tileCount;
    [SerializeField] Text startTimer;
    [SerializeField] Text gameOverText;
    [SerializeField] Text informationText;
    [SerializeField] Text explainText;
    [SerializeField] Text hasTileCount;

    private Transform[,] childPortal = new Transform[6, 2];
    private Transform[] childSpawner = new Transform[6];
    private SphereCollider[] childSphereColliders= new SphereCollider[6];

    private int[] childTileCount = new int[6];
    private int[] hasTileNum = new int[6];
    private bool[] isOutPlayer = new bool[6];
    
    private bool isLose,
                 isGameEnd;

    private const int startTime = 10;
    private int dieCount,
                killCount,
                destroyCount;

    private float suviveTime;

    public int i, j, tempTop = 5, tempBottom = 0;
    public int spawnerRotation = 90;

    #region Getter + Setter
    public void SetPortal(Transform tr, int i, int j) => childPortal[i, j] = tr;
    public void SetSpawner(Transform tr, int i) => childSpawner[i] = tr;
    public void SetPersonTileRadius(int i, float _radius) => childSphereColliders[i].radius = _radius;
    public void SetChildTileCount(int i,int _count) => childTileCount[i] = _count;
    public void SetPlusHasTileNum(int i) => hasTileNum[i] += 1000;
    public void SetMinusHasTileNum(int i) => hasTileNum[i] -= 10;
    public void SetIsOutPlayer(bool _isOutPlayer, int i) => isOutPlayer[i] = _isOutPlayer;
    public void SetKillCount() => killCount++;
    public void SetDieCount() => dieCount++;
    public void SetDestroyCount() => destroyCount++;
    public Transform GetPortal(int i, int j) => childPortal[i, j];
    public Transform GetSpawner(int i) => childSpawner[i];
    public float GetPersonTileRadius(int i) => childSphereColliders[i].radius;
    public int GetHasTileNum(int i) => hasTileNum[i];
    public bool GetIsOutPlayer(int i)
    {
        if (i < 0 || i > 6) return false;
        return isOutPlayer[i];
    }
    #endregion

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => generalManager.GetIsRoomFull());
        CreatePersonTile();
        generalManager.SetIsCreateTile(true);
        for (int i = startTime; i >= 1; i--)
        {
            startTimer.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        startTimer.text = "! ½ÃÀÛ !";
        generalManager.CreatePlayer();
        generalManager.SetIsCreatePlayer(true);
        yield return new WaitForSeconds(0.5f);
        Camera.main.GetComponent<MainCamera>().SetPlayers();
        beforeStartPanel.SetActive(false);
    }
    private void CreatePersonTile()
    {
        GameObject _personTile;
        float _x = 0,
              _z = 0;
        int _tagNum = 0;
        float _radius = personTileMap.GetComponent<SphereCollider>().radius;

        for (int i = 0; i <= PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            switch (i)
            {
                case 0: //Áß¾Ó
                    _x = 0.5f;
                    _z = 0.865f;
                    break;
                case 1: //ÁÂÇÏ
                    _x = 0;
                    _z = 0;
                    break;
                case 2: //¿ìÇÏ
                    _x = 1;
                    _z = 0;
                    break;
                case 3: //¿ì
                    _x = 1.5f;
                    _z = 0.865f;
                    break;
                case 4: //¿ì»ó
                    _x = 1;
                    _z = 1.73f;
                    break;
                case 5: //ÁÂ»ó
                    _x = 0;
                    _z = 1.73f;
                    break;
                case 6: //ÁÂ
                    _x = -0.5f;
                    _z = 0.865f;
                    break;
            }
            _personTile = Instantiate(personTileMap, new Vector3
            (_x * _radius * 3, 0, _z * _radius * 3), Quaternion.identity);
            _personTile.transform.parent = transform;
            _personTile.name = "PerosnTileMap" + i;
            _personTile.tag = "Floor" + _tagNum++;
            _personTile.GetComponent<PersonTileMap>().CreateHexTileMap();
            if (i != 0) childSphereColliders[i - 1] = _personTile.GetComponent<SphereCollider>();
        }
        Destroy(personTileMap);
    }
    private void FixedUpdate()
    {
        if (!PhotonNetwork.InRoom) return;
        if (!generalManager.GetIsCreatePlayer() || isGameEnd) return;
        tileCount.text = "³²Àº ¶¥\n";
        if (generalManager.GetIsGameEnd())
        {
            isGameEnd = true;
            SetGameOverPanel();
            return;
        }
        if (!isLose) suviveTime += Time.deltaTime;
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetPlayerNumber() == -1) return;
            tileCount.text += player.NickName + " : ";
            tileCount.text += (isOutPlayer[player.GetPlayerNumber() - 1] ? "OUT" : childTileCount[player.GetPlayerNumber() - 1].ToString())
                + (player.IsLocal ? "<color=red> ME</color>" : "") + "\n";

            if (!isLose && isOutPlayer[PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1])
            {
                isLose = true;
                SetGameOverPanel();
            }
        }
        hasTileCount.text = "X "+ Math.Ceiling(((float)hasTileNum[PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1])/1000) + "\n";
    }
    private void SetGameOverPanel()
    {
        informationText.text = "Å³ : " + killCount + " Á×À½ : " + dieCount + " ÆÄ±«ÇÑ ¶¥ : " + destroyCount + " »ýÁ¸½Ã°£ : " + Math.Truncate(suviveTime * 100)/100;
        if (isGameEnd)
        {
            int winnerIndex = generalManager.GetWinnerPlayerIndex();
            if (!PhotonNetwork.InRoom) return;
            if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == winnerIndex) gameOverText.text = "Winner Winner";
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
                gameOverText.text = "Winner Winner";
                explainText.text = PhotonNetwork.LocalPlayer.NickName + " ´ÔÀÌ ½Â¸®ÇÏ¿´½À´Ï´Ù";
            }
            else explainText.text = generalManager.GetPlayerName(winnerIndex - 1) + " ´ÔÀÌ ½Â¸®ÇÏ¿´½À´Ï´Ù";
        }
        gameOverPanel.SetActive(true);
    }
}