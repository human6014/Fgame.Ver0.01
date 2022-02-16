using System.Collections;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    private Rigidbody rigid;
    public int damage;
    public int speed;
    public PhotonView view;
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * speed);
        StartCoroutine("BallisticFall");
    }
    #region ÃÑ¾Ë ±ËÀû ¼³Á¤
    IEnumerator BallisticFall()
    {
        yield return new WaitForSeconds(0.3f);
        rigid.constraints = RigidbodyConstraints.None;
        yield return new WaitForSeconds(2.7f);
        Destroy(gameObject);
    }
    #endregion
    #region ÃÑ¾Ë Ãæµ¹ °Ë»ç
    private void OnCollisionEnter(Collision collision)
    {
        
        if (isCollison) return;
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !photonView.IsMine)
        {
            other.GetComponent<Player>().Hit(damage);
            isCollison = true;
            view.RPC(nameof(Destroy), RpcTarget.All);
        }
        else if (other.CompareTag("TPlayer"))
        {
            other.GetComponent<TestPlayer>().Hit(damage);
            isCollison = true;
            view.RPC(nameof(Destroy), RpcTarget.All);
        }
        if (other.tag.Substring(0, 5) == "Floor")
        {
            isCollison = true;
            view.RPC(nameof(Destroy), RpcTarget.All);
        }
        
    }
    #endregion
    [PunRPC]
    void Destroy() => Destroy(gameObject);
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}