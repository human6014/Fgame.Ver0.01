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
        // ���ӽ��۽� �ʱ�ȭ
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Photon.Realtime.Player player = PhotonNetwork.PlayerList[i];
            playerInfos.Add(new PlayerInfo(player.NickName, player.ActorNumber, 0, 0, 0, PhotonNetwork.Time + 3.0));
        }
        MasterSendPlayerInfo(0);
    }

    void MasterRemovePlayerInfo(int actorNum)
    {
        // OnPlayerLeftRoom���� ���� ������� �÷��̾� ����
        PlayerInfo playerInfo = playerInfos.Find(x => x.actorNum == actorNum);
        playerInfos.Remove(playerInfo);
        MasterSendPlayerInfo(1);
    }

    [PunRPC]
    public void MasterReceiveRPC(byte code, int actorNum, int colActorNum)
    {
        // �ð��� ��������
        PlayerInfo playerInfo = playerInfos.Find(x => x.actorNum == actorNum);
        double lifeTime = PhotonNetwork.Time - playerInfo.lifeTime;
        lifeTime = System.Math.Truncate(lifeTime * 100) * 0.01;
        playerInfo.lifeTime = lifeTime;


        // �ڱⰡ �ƴ� �����̳� �÷��̾� �浹�� ų���� ����
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
        // ������ PlayerInfo ���� �� ������
        playerInfos.Sort((p1, p2) => p2.lifeTime.CompareTo(p1.lifeTime));

        string jdata = JsonUtility.ToJson(new Serialization<PlayerInfo>(playerInfos));
        PV.RPC(nameof(OtherReceivePlayerInfoRPC), RpcTarget.Others, code, jdata);
    }

    [PunRPC]
    void OtherReceivePlayerInfoRPC(byte code, string jdata)
    {
        // �ٸ� ����� PlayerInfo �ޱ�
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

        // ��� ���� �־�� ������ �� ����, �����Ϳ� Ŭ��� �����Ͱ� ������
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
        // �����Ͱ� ������ �ٲ� �����Ͱ� ȣ��Ǽ� ����
        if (GameManager.Instance().master())
        {
            MasterRemovePlayerInfo(otherPlayer.ActorNumber);
        }
        PhotonNetwork.LoadLevel(0);
    }
}
