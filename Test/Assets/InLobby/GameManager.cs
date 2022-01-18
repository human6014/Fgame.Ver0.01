using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GameManager : MonoBehaviourPunCallbacks
{

    #region ½Ì±ÛÅæ
    static GameManager _instance = null;
    public static GameManager Instance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }else Destroy(gameObject);
    }
    #endregion
    public string roomCode = string.Empty;
    public string playerName = string.Empty;
    public int stateIndex = -1;
    public bool isFull = false;
    [SerializeField] GameObject delayCancelButton;
    [SerializeField] Text roomCountDisplay;
    [SerializeField] Text timerToStartDisplay;
    AllTileMap allTileMap;
    GameObject AllTileMap;
    GameObject[] list;
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2 };

    #region Set Get
    public bool master() => PhotonNetwork.LocalPlayer.IsMasterClient;

    public int actorNum(Photon.Realtime.Player player = null)
    {
        if (player == null) player = PhotonNetwork.LocalPlayer;
        return player.ActorNumber;
    }

    public void destroy(List<GameObject> GO)
    {
        for (int i = 0; i < GO.Count; i++) PhotonNetwork.Destroy(GO[i]);
    }

    public void SetPos(Transform Tr, Vector3 target)
    {
        Tr.position = target;
    }

    public void SetTag(string key, object value, Photon.Realtime.Player player = null)
    {
        if (player == null) player = PhotonNetwork.LocalPlayer;
        player.SetCustomProperties(new Hashtable { { key, value } });
    }

    public object GetTag(Photon.Realtime.Player player, string key)
    {
        if (player.CustomProperties[key] == null) return null;
        return player.CustomProperties[key].ToString();
    }

    public bool AllhasTag(string key)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i].CustomProperties[key] == null) return false;
        return true;
    }
    #endregion

    private void Start()
    {
        
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 20;
        PhotonNetwork.GameVersion = "1.0";
        
        Debug.Log("NetworkManager Start");
    }
    public void Connect() => PhotonNetwork.ConnectUsingSettings();
    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    public override void OnJoinRandomFailed(short returnCode, string message) => PhotonNetwork.CreateRoom(null, roomOptions);
    public override void OnJoinRoomFailed(short returnCode, string message)
    {

    }
    public override void OnJoinedRoom()
    {
    }
    /*
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        PhotonNetwork.LoadLevel(0);
        //allTileMap.MinusPlayer();
    }
    */
    [PunRPC]
    void PunUpdate()
    {
        //roomCountDisplay.text = cPlayerCount + " / " + cMaxPlayer;

        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {  
            isFull = true;
        }
    }
    public void ResetInfo()
    {
        stateIndex = -1;
        playerName = "";
        roomCode = "";
    }
    public void SceneMove(int stateIndex, string playerName, string roomCode)
    {
        this.stateIndex = stateIndex;
        this.playerName = playerName;
        this.roomCode = roomCode;
        PhotonNetwork.NickName = playerName;
 
        switch (this.stateIndex)
        {
            case 0:
                PhotonNetwork.JoinRandomRoom();
                break;
            case 1:
                PhotonNetwork.CreateRoom(roomCode, roomOptions);
                //PhotonNetwork.CurrentRoom.IsOpen = false;
                break;
            case 2:
                PhotonNetwork.JoinRoom(roomCode);
                break;
            default:
                break;
        }
        PhotonNetwork.LoadLevel(1);
    }

    public void SetAllTileMap(GameObject obj)
    {
        /*
        if (!isFull) return;
        allTileMap = obj.GetComponent<AllTileMap>();
        PhotonNetwork.Instantiate("Player", allTileMap.childSpawner[0, cPlayerCount - 1].position + Vector3.up, Quaternion.identity);
        allTileMap.PlusPlayer(PhotonNetwork.NickName);
        */
    }

}
