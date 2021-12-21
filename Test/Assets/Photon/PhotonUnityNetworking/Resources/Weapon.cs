using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Weapon : MonoBehaviourPunCallbacks
{
    public enum weaponsType { Melee,Range };
    public weaponsType type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public PhotonView PV;
    public void UseWeapons()
    {
        Debug.Log("UseWeapons");
        if (type == weaponsType.Melee)
        {
            StartCoroutine("Swing");
        }
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
            Debug.Log("Weapon's Trigger");
        }
    }
}
