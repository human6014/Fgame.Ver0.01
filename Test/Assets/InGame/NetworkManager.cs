using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
public class NetworkManager : MonoBehaviourPunCallbacks//,IPunObservable
{
    public PhotonView PV;
    public List<PlayerInfo> playerInfos;
    public bool isStart, isEnd;


    void MasterInitPlayerInfo()
    {
        // 게임시작시 초기화
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Photon.Realtime.Player player = PhotonNetwork.PlayerList[i];
            playerInfos.Add(new PlayerInfo(player.NickName, player.ActorNumber, 0, 0, 0, PhotonNetwork.Time + 3.0));
        }
        MasterSendPlayerInfo(0);
    }

    void MasterRemovePlayerInfo(int actorNum)
    {
        // OnPlayerLeftRoom으로 방을 나갈경우 플레이어 제거
        PlayerInfo playerInfo = playerInfos.Find(x => x.actorNum == actorNum);
        playerInfos.Remove(playerInfo);
        MasterSendPlayerInfo(1);
    }

    [PunRPC]
    public void MasterReceiveRPC(byte code, int actorNum, int colActorNum)
    {
        // 시간과 죽음여부
        PlayerInfo playerInfo = playerInfos.Find(x => x.actorNum == actorNum);
        double lifeTime = PhotonNetwork.Time - playerInfo.lifeTime;
        lifeTime = System.Math.Truncate(lifeTime * 100) * 0.01;
        playerInfo.lifeTime = lifeTime;


        // 자기가 아닌 라인이나 플레이어 충돌시 킬데스 증가
        /*
        if (code == DIE)
        {
            playerInfo = null;
            playerInfo = playerInfos.Find(x => x.actorNum == colActorNum);
            ++playerInfo.killDeath;
        }
        */
        MasterSendPlayerInfo(code);
    }



    void MasterSendPlayerInfo(byte code)
    {
        // 방장은 PlayerInfo 정렬 후 보내기
        playerInfos.Sort((p1, p2) => p2.lifeTime.CompareTo(p1.lifeTime));

        string jdata = JsonUtility.ToJson(new Serialization<PlayerInfo>(playerInfos));
        PV.RPC(nameof(OtherReceivePlayerInfoRPC), RpcTarget.Others, code, jdata);
    }

    [PunRPC]
    void OtherReceivePlayerInfoRPC(byte code, string jdata)
    {
        // 다른 사람은 PlayerInfo 받기
        playerInfos = JsonUtility.FromJson<Serialization<PlayerInfo>>(jdata).target;
    }

    [PunRPC]
    void StartSyncRPC()
    {
        isStart = true;
    }
    IEnumerator Loading()
    {
        GameManager.Instance().SetTag("loadScene", true);
        while (!GameManager.Instance().AllhasTag("loadScene")) yield return null;

        // 모두 씬에 있어야 생성할 수 있음, 에디터와 클라는 에디터가 마스터
        yield return new WaitForSeconds(1);
        PhotonNetwork.Instantiate("Player", new Vector3(0, 1, 0), Quaternion.identity);

        while (!GameManager.Instance().AllhasTag("loadPlayer")) yield return null;
    }

    IEnumerator Start()
    {
        yield return Loading();

        if (GameManager.Instance().master())
        {
            MasterInitPlayerInfo();
            yield return new WaitForSeconds(3);
            PV.RPC(nameof(StartSyncRPC), RpcTarget.AllViaServer);
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // 마스터가 나가면 바뀐 마스터가 호출되서 성공
        if (GameManager.Instance().master())
        {
            MasterRemovePlayerInfo(otherPlayer.ActorNumber);
        }
        PhotonNetwork.LoadLevel(0);
    }
}
