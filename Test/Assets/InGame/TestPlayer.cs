using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class TestPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    float timer;
    bool isDying;
    [SerializeField] bool[] hasWeapons;
    [SerializeField] GameObject[] weapons;
    [SerializeField] Image HP;
    [SerializeField] Image MP;
    [SerializeField] Text Name;
    [SerializeField] PhotonView view;
    GameObject child;
    Rigidbody rigid;
    Transform tr;

    private void Start()
    {
        timer = 0.0f;
        isDying = false;

        rigid = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        child = transform.Find("GameObject").gameObject;
        for (int i = 0; i < hasWeapons.Length; i++)
        {
            if (hasWeapons[i]) weapons[i].SetActive(true);
        }
    }
    public void Hit(int damage)
    {
        if (isDying) return;
        view.RPC("PunHit", RpcTarget.All, damage);
    }
    [PunRPC]
    void PunHit(int damage)
    {
        HP.fillAmount -= damage / 100f;
        if (HP.fillAmount <= 0)
        {
            HP.fillAmount = 0;
            isDying = true;
            Die();
        }
    }//30,25.95
    void Die()
    {
        gameObject.transform.position = new Vector3(0, 2, -0.5f);
        gameObject.transform.eulerAngles = new Vector3(-90, 180, 0); //¹Ì¿Ï¼º
        HP.fillAmount = 1;
        MP.fillAmount = 1;
        StartCoroutine("Respawn");
    }
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);
        gameObject.transform.position = new Vector3(0, 1.05f, 0);
        gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        isDying = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GameManager")
        {
            isDying = true;
            Die();
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(MP.fillAmount);
        }
        else
        {
            MP.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
