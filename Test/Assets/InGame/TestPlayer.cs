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
    void Update()
    {
        if (isDying)
        {
            timer += Time.deltaTime;
            if (timer >= 5f)
            {
                timer = 0;
                isDying = false;
            }
        }
    }
    void Hit()
    {
        HP.fillAmount -= 0.1f;
        if (HP.fillAmount <= 0)
        {
            isDying = true;
            Die();
        }
    }
    void Die()
    {
        gameObject.transform.position = new Vector3(0, 1, 0);
        child.gameObject.transform.eulerAngles = new Vector3(0, 0, 180); //¹Ì¿Ï¼º
        MP.fillAmount = 1;
        HP.fillAmount = 1;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GameManager")
        {
            isDying = true;
            Die();
        }
        if (other.gameObject.tag == "Melee")
        {
            HP.fillAmount -= 0.2f;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(HP.fillAmount);
            stream.SendNext(MP.fillAmount);
        }
        else
        {
            HP.fillAmount = (float)stream.ReceiveNext();
            MP.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
