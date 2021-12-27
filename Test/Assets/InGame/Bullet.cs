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
        yield return new WaitForSeconds(0.25f);
        rigidbody.constraints = RigidbodyConstraints.None;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PhotonView>().IsMine && !photonView.IsMine)//&& !this.gameObject
        {
            Debug.Log("Bullett's CollisionEnter");
            collision.gameObject.GetComponent<Player>().Hit(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("TPlayer"))
        {
            collision.gameObject.GetComponent<TestPlayer>().Hit(damage);
        }
        if (collision.gameObject.tag.Substring(0, 5) == "Floor")
        {
            Destroy(gameObject);
        }
    }
}
