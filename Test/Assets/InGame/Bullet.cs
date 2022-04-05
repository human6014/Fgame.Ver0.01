using System.Collections;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    private Rigidbody rigid;
    private AllTileMap allTileMap;
    [SerializeField] int damage;
    [SerializeField] int speed;
    public PhotonView view;
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * speed);
        allTileMap = FindObjectOfType<AllTileMap>();
        StartCoroutine(nameof(BallisticFall));
    }
    #region �Ѿ� ���� ����
    IEnumerator BallisticFall()
    {
        yield return new WaitForSeconds(0.3f);
        rigid.constraints = RigidbodyConstraints.None;
        yield return new WaitForSeconds(2.7f);
        Destroy(gameObject);
    }
    #endregion
    #region �Ѿ� �浹 �˻�
    private void OnCollisionEnter(Collision collision)
    {
        if (isCollison) return;
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !photonView.IsMine)
        {
            if(other.GetComponent<Player>().Hit(damage) && PhotonNetwork.LocalPlayer.IsLocal) allTileMap.SetKillCount();
            //ų ���� ���� ����� �ƴ� ���� ������� �ö� ����
            isCollison = true;
            Destroy(gameObject);
            //view.RPC(nameof(Destroy), RpcTarget.All);
        }
        else if (other.CompareTag("TPlayer"))
        {
            other.GetComponent<TestPlayer>().Hit(damage);
            isCollison = true;
            Destroy(gameObject);
            //view.RPC(nameof(Destroy), RpcTarget.All);
        }
        if (other.tag.Substring(0, 5) == "Floor")
        {
            isCollison = true;
            Destroy(gameObject);
            //view.RPC(nameof(Destroy), RpcTarget.All);
        }
        
    }
    #endregion
    [PunRPC]
    void Destroy() => Destroy(gameObject);
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}