using System.Collections;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    public int damage;
    public int speed;
    public PhotonView pv;
    Vector3 trajectory = Vector3.forward * 15;
    private void Start()
    {
        StartCoroutine("BallisticFall");
        Destroy(gameObject, 3);
    }
    void Update()
    {
        if (isCollison) return;
        transform.Translate(trajectory * Time.deltaTime);
    }
    #region ÃÑ¾Ë ±ËÀû ¼³Á¤
    IEnumerator BallisticFall()
    {
        yield return new WaitForSeconds(0.4f);
        while (true)
        {
            trajectory += Vector3.down * 0.08f + Vector3.back * 0.04f;
            yield return null;
        }
    }
    #endregion
    #region ÃÑ¾Ë Ãæµ¹ °Ë»ç
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !photonView.IsMine)
        {
            other.GetComponent<Player>().Hit(damage,0);
            isCollison = true;
            pv.RPC(nameof(Destroy), RpcTarget.All);
        }
        else if (other.CompareTag("TPlayer"))
        {
            other.GetComponent<TestPlayer>().Hit(damage);
            isCollison = true;
            pv.RPC(nameof(Destroy), RpcTarget.All);
        }
        if (other.tag.Substring(0, 5) == "Floor")
        {
            isCollison = true;
            pv.RPC(nameof(Destroy), RpcTarget.All);
        }
    }
    #endregion
    [PunRPC]
    void Destroy() => Destroy(gameObject);
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}