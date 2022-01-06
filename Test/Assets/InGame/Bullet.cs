using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks
{
    public int damage;
    new Rigidbody rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        StartCoroutine("Flag");
        Destroy(gameObject, 3f);
    }
    IEnumerator Flag()
    {
        yield return new WaitForSeconds(0.2f);
        rigidbody.constraints = RigidbodyConstraints.None;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PhotonView>().IsMine && !photonView.IsMine)
        {
            Debug.Log(collision.gameObject.tag);
            
            collision.gameObject.GetComponent<Player>().Hit(damage);
            Destroy(gameObject,0.1f);
        }
        else if (collision.gameObject.CompareTag("TPlayer"))
        {
            collision.gameObject.GetComponent<TestPlayer>().Hit(damage);
            Destroy(gameObject, 0.1f);
        }
        if (collision.gameObject.tag.Substring(0, 5) == "Floor")
        {
            Destroy(gameObject, 0.1f);
        }
    }
}
