using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Weapon : MonoBehaviourPunCallbacks
{
    public enum weaponsType { Melee,Range };
    public weaponsType type;
    public int damage;
    public float rate,speed;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public GameObject bullet;
    public GameObject bulletCase;
    public Transform bulletPos;
    public Transform bulletCasePos;
    public new Rigidbody rigidbody;
    public void UseWeapons()
    {
        if (type == weaponsType.Melee) StartCoroutine("Swing");
        else if (type == weaponsType.Range) StartCoroutine("Shot");
    }
    IEnumerator Shot()
    {
        rigidbody.isKinematic = true;
        GameObject intantBullet = PhotonNetwork.Instantiate("BulletHandGun", bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * speed;

        GameObject intantCase = PhotonNetwork.Instantiate("BulletCase", bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-1, -0.5f) + Vector3.up * Random.Range(0.5f, 1);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 2,ForceMode.Impulse);

        yield return new WaitForSeconds(0.05f);
        rigidbody.isKinematic = false;
    }
    IEnumerator Swing()
    {
        Debug.Log("Swing");
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = false;
        trailEffect.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Hit(damage);
        }
        else if (other.CompareTag("TPlayer"))
        {
            other.GetComponent<TestPlayer>().Hit(damage);
        }
    }
}
