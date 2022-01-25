using System.Collections;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    public int damage;
    public PhotonView pv;
    Vector3 trajectory = Vector3.forward * 15;
    private void Start() => StartCoroutine("BallisticFall");
    void Update() => transform.Translate(trajectory * Time.deltaTime);
    #region ÃÑ¾Ë ±ËÀû ¼³Á¤
    IEnumerator BallisticFall()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            trajectory += Vector3.down * 0.8f + Vector3.back * 0.4f;
            yield return new WaitForSeconds(2.5f);
            Destroy(gameObject);
            yield break;
        }
    }
    #endregion
    #region ÃÑ¾Ë Ãæµ¹ °Ë»ç
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !photonView.IsMine)
        {
            other.GetComponent<Player>().Hit(damage,0);
            pv.RPC("Destroy", RpcTarget.AllBuffered);
            //Destroy(gameObject);
        }
        else if (other.CompareTag("TPlayer"))
        {
            other.GetComponent<TestPlayer>().Hit(damage);
            pv.RPC("Destroy", RpcTarget.AllBuffered);
        }
        if (other.tag.Substring(0, 5) == "Floor")
        {
            pv.RPC("Destroy", RpcTarget.AllBuffered);
        }
    }
    #endregion
    [PunRPC]
    void Destroy() => Destroy(gameObject);
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}