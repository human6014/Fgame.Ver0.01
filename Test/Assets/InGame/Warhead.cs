using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class Warhead : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    private int destroyBlockCount;
    public GameObject target;
    public GameObject particle;
    public MeshRenderer meshRenderer;
    public PhotonView pv;
    public int damage;
    private void Start()
    {
        StartCoroutine("BallisticFall");
        Destroy(gameObject, 3);
    }
    void Update()
    {
        if (isCollison) return;
        transform.Translate(Vector3.back / 8);
    }
    #region �Ѿ� ���� ����
    IEnumerator BallisticFall()
    {

        yield return new WaitForSeconds(0.1f);
        while (!isCollison)
        {
            if (transform.eulerAngles.x == 270) yield break;
            transform.Rotate(Vector3.left);
            yield return null;
        }
    }
    #endregion
    #region �Ѿ� �浹 �˻�
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !photonView.IsMine)
        {
            pv.RPC(nameof(Effect), RpcTarget.All);
            Debug.Log("player hit");
        }
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
    [PunRPC]
    IEnumerator Effect()
    {
        isCollison = true;
        meshRenderer.enabled = false;
        particle.SetActive(true);

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.up, 0);
        foreach (RaycastHit hit in raycastHits)
        {
            if (!hit.transform.name.StartsWith("PerosnTileMap") && hit.transform.tag.StartsWith("Floor") &&
                !hit.transform.tag.EndsWith(pv.Owner.GetPlayerNumber().ToString()) && !hit.transform.tag.EndsWith("7"))
            {
                Destroy(hit.transform.gameObject);
                destroyBlockCount++;
                Debug.Log(hit.transform.name);
                Debug.Log(destroyBlockCount);
            }
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<Player>().Hit(10, 0);
                Debug.Log(hit.transform.name);
            }
        }

        yield return new WaitForSeconds(3);
        pv.RPC(nameof(Destroy), RpcTarget.All);
    }
    [PunRPC]
    void Destroy() => Destroy(gameObject);
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
