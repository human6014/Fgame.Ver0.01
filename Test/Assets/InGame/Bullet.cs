using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().Hit(damage);
        }
        else if (collision.gameObject.CompareTag("TPlayer"))
        {
            collision.gameObject.GetComponent<TestPlayer>().Hit(damage);
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject, 3);
        }
        else
        {
            //Destroy(gameObject, 3);
        }
    }
    private void OnTriggerEnter(Collider other)
    {

    }
}
