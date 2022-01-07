using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Weapon : MonoBehaviourPunCallbacks
{
    public enum weaponsType { Melee, Range };
    public weaponsType type;
    public int damage;
    public float rate, speed;
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
        else if (type == weaponsType.Range) Shot();
    }
    private void Shot()
    {
        GameObject intantBullet = PhotonNetwork.Instantiate("BulletHandGun", bulletPos.position, bulletPos.rotation);
        //Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        //bulletRigid.velocity = bulletPos.forward * speed;
        //bulletRigid.AddForce(bulletPos.forward * speed, ForceMode.Impulse); //speed minimum 3
        GameObject intantCase = PhotonNetwork.Instantiate("BulletCase", bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-0.02f, -0.01f) + Vector3.up * Random.Range(0.01f, 0.02f);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up, ForceMode.Impulse);
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
        if (other.CompareTag("Player")) other.GetComponent<Player>().Hit(damage);
        else if (other.CompareTag("TPlayer")) other.GetComponent<TestPlayer>().Hit(damage);
    }
}
