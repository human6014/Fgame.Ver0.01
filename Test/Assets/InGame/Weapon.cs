using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Weapon : MonoBehaviourPunCallbacks, IPunObservable
{
    public enum WeaponsType { Melee, Range, Throwing };
    public WeaponsType type;
    public int damage;
    public float rate, speed;
    [SerializeField] BoxCollider meleeArea;
    [SerializeField] TrailRenderer trailEffect;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bulletCase;
    [SerializeField] Transform bulletPos;
    [SerializeField] Transform bulletCasePos;
    private int curEquip;
    #region 사용 무기 판별
    public void UseWeapons(int _curEquip)
    {
        switch (type)
        {
            case WeaponsType.Melee:
                curEquip = _curEquip;
                StartCoroutine(nameof(Swing));
                break;
            case WeaponsType.Range:
                Shot();
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
        GameObject _bullet;
        Rigidbody _bulletRigid;
        for (int i = 0; i < 12; i++)
        {
            _bullet = PhotonNetwork.Instantiate(bullet.name, bulletPos.position, bulletPos.rotation);
            if (bullet.name != "BulletShotgun") break;
            _bulletRigid = _bullet.GetComponent<Rigidbody>();
            _bulletRigid.AddForce(bulletPos.right * Random.Range(-0.35f, 0.35f) + bulletPos.up * Random.Range(0.05f, 0.20f), ForceMode.Impulse);
        }
        CreateCase();
    }
    private void Throw()
    {
        PhotonNetwork.Instantiate(bullet.name, bulletPos.position, bulletPos.rotation * Quaternion.Euler(new Vector3(0, 180, 0)));
        CreateCase();
    }
    private void CreateCase()
    {
        if (bulletCase == null) return;
        GameObject instantCase = PhotonNetwork.Instantiate(bulletCase.name, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-0.02f, -0.01f) + Vector3.up * Random.Range(0.01f, 0.02f);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up, ForceMode.Impulse);
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
            Player player = other.GetComponent<Player>();
            //cc기 설계 변경 보류
            if (curEquip == 1) player.KnockBack(transform.root.position.x, transform.root.position.z);
            else player.CrowdControl(curEquip);
            player.Hit(damage);
            meleeArea.enabled = false;
        }
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}