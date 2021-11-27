using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum weaponsType { Melee,Range };
    public weaponsType type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

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
}
