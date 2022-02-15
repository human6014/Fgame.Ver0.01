using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Weapon : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum WeaponsType { Melee, Range, Cannon, Throwing};
    public WeaponsType type;
    public int damage;
    public float rate, speed;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public GameObject bullet;
    public GameObject bulletCase;
    public Transform bulletPos;
    public Transform bulletCasePos;
    #region 사용 무기 판별
    public void UseWeapons()
    {
        switch (type)
        {
            case WeaponsType.Melee:
                StartCoroutine("Swing");
                break;
            case WeaponsType.Range:
                Shot();
                break;
            case WeaponsType.Cannon:
                Launch();
                break;
            case WeaponsType.Throwing:
                Invoke(nameof(Throw), 0.2f);
                break;
            default:
                Debug.LogError("NonUseWeapons");
                break;
        }
    }
    #endregion
    #region 원거리 무기 사용
    private void Shot()
    {
        PhotonNetwork.Instantiate("BulletHandGun", bulletPos.position, bulletPos.rotation);
        GameObject intantCase = PhotonNetwork.Instantiate("BulletCase", bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-0.02f, -0.01f) + Vector3.up * Random.Range(0.01f, 0.02f);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up, ForceMode.Impulse);
    }
    private void Launch()
    {
        StartCoroutine(nameof(Reload));
        PhotonNetwork.Instantiate("WarHead", bulletPos.position, bulletPos.rotation * Quaternion.Euler(new Vector3(0, 180, 0)));
    }
    private void Throw()
    {
        PhotonNetwork.Instantiate("Grenade", bulletPos.position, bulletPos.rotation * Quaternion.Euler(new Vector3(0, 180, 0)));
        GameObject instantCase = PhotonNetwork.Instantiate("Pin", bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-0.02f, -0.01f) + Vector3.up * Random.Range(0.01f, 0.02f);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up, ForceMode.Impulse);
    }
    IEnumerator Reload()
    {
        bullet.SetActive(false);
        yield return new WaitForSeconds(2);
        bullet.SetActive(true);
    }
    #endregion
    #region 근접 무기
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
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
            meleeArea.enabled = false;
        }
    }
    #endregion 
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}