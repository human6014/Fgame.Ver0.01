using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks
{
    public int damage;
    private void Start()
    {
        Destroy(gameObject, 3f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !this.gameObject)
        {
            collision.gameObject.GetComponent<Player>().Hit(damage);
        }
        else if (collision.gameObject.CompareTag("TPlayer"))
        {
            collision.gameObject.GetComponent<TestPlayer>().Hit(damage);
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
        else
        {
            //Destroy(gameObject, 3);
        }
    }
    /*
    private void Update()
    {
        Vector3 dir = Vector3.forward;
        transform.position += dir * 5 * Time.deltaTime;
    }
    */
}
