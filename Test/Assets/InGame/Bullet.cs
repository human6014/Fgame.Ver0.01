using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks,IPunObservable
{
    public int damage;
    public PhotonView pv;
    new Rigidbody rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        StartCoroutine("Flag");
    }
    IEnumerator Flag()
    {
        yield return new WaitForSeconds(0.2f);
        rigidbody.constraints = RigidbodyConstraints.None;
        yield return new WaitForSeconds(3);
        if (!gameObject) yield break;
        pv.RPC("Destroy", RpcTarget.AllBuffered);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!gameObject) return;
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PhotonView>().IsMine && !photonView.IsMine)
        {
            Debug.Log(collision.gameObject.tag);
            
            collision.gameObject.GetComponent<Player>().Hit(damage);
            pv.RPC("Destroy",RpcTarget.AllBuffered);
        }
        else if (collision.gameObject.CompareTag("TPlayer"))
        {
            collision.gameObject.GetComponent<TestPlayer>().Hit(damage);
            pv.RPC("Destroy", RpcTarget.AllBuffered);
        }
        if (collision.gameObject.tag.Substring(0, 5) == "Floor")
        {
            pv.RPC("Destroy", RpcTarget.AllBuffered);
        }
        if (collision.gameObject.CompareTag("GameController"))
        {
            pv.RPC("Destroy", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    void Destroy()
    {
        if (!gameObject) return;
        Destroy(gameObject);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { if (!gameObject) return; }
}
