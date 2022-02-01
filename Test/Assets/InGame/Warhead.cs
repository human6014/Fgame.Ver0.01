using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Warhead : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    public GameObject particle;
    public MeshRenderer meshRenderer;
    public int damage;
    public PhotonView pv;
    Vector3 trajectory = Vector3.forward * 12;
    private void Start()
    {
        StartCoroutine("BallisticFall");
        Destroy(gameObject, 3);
    }
    void Update()
    {
        if (isCollison) return;
        transform.Translate(trajectory * Time.deltaTime);
        //transform.LookAt(trajectory);
    }
    #region ÃÑ¾Ë ±ËÀû ¼³Á¤
    IEnumerator BallisticFall()
    {
        yield return new WaitForSeconds(0.1f);
        while (!isCollison && trajectory.z >= 0)
        {
            trajectory += Vector3.down * 0.08f + Vector3.back * 0.12f;
            yield return null;
        }
    }
    #endregion
    #region ÃÑ¾Ë Ãæµ¹ °Ë»ç
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !photonView.IsMine) StartCoroutine(nameof(Effect));
        else if (other.tag.StartsWith("Floor"))
        {
            if (other.name.StartsWith("Spawner") || pv.Owner.GetPlayerNumber().ToString() == other.tag.Substring(other.tag.Length - 1))
                StartCoroutine(nameof(Effect));
            else
            {
                if (other.tag.Substring(5) != "7") Destroy(other.gameObject);
                StartCoroutine(nameof(Effect));
            }
        }
    }
    IEnumerator Effect()
    {
        isCollison = true;
        meshRenderer.enabled = false;
        particle.SetActive(true);

        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
