using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Warhead : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    public GameObject particle;
    public MeshRenderer meshRenderer;
    public int damage;
    public PhotonView pv;
    Vector3 trajectory = Vector3.forward * 12;
    private void Start() => StartCoroutine("BallisticFall");
    void Update()
    {
        if (isCollison) return;
        transform.Translate(trajectory * Time.deltaTime);
    }
    #region ÃÑ¾Ë ±ËÀû ¼³Á¤
    IEnumerator BallisticFall()
    {
        yield return new WaitForSeconds(0.2f);
        while (true)
        {
            trajectory += Vector3.down * 0.8f + Vector3.back * 2f;
            yield return new WaitForSeconds(2.5f);
            Destroy(gameObject);
            yield break;
        }
    }
    #endregion
    #region ÃÑ¾Ë Ãæµ¹ °Ë»ç
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Substring(0, 5) == "Floor" )
        {
            if(other.tag.Substring(5) != "7") Destroy(other.gameObject);
            StartCoroutine(nameof(Effect));
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
