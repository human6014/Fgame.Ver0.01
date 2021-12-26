using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    public int damage;
    bool start;
    Vector3 curPos;
    new Rigidbody rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>(); 
        StartCoroutine("Flag");
        Destroy(gameObject, 3f);
    }
    IEnumerator Flag()
    {
        yield return new WaitForSeconds(0.1f);
        start = true;
        rigidbody.constraints = RigidbodyConstraints.None;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !gameObject)
        {
            collision.gameObject.GetComponent<Player>().Hit(damage);
        }
        else if (collision.gameObject.CompareTag("TPlayer"))
        {
            collision.gameObject.GetComponent<TestPlayer>().Hit(damage);
        }
        if (collision.gameObject.tag.Substring(0, 5) == "Floor")
        {
            Destroy(gameObject);
        }
        else
        {
            //Destroy(gameObject, 3);
        }
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            /*
            if (rigidbody.velocity.sqrMagnitude < new Vector3(1, 0, 1).sqrMagnitude && start)
            {
                Destroy(gameObject);
            }
            */
            
        }
        //Vector3 dir = Vector3.forward;
        //transform.position += dir * 5 * Time.deltaTime;
        else
        {
            if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
            else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
