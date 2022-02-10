using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class Grenade : MonoBehaviourPunCallbacks
{
    private AllTileMap allTileMap;
    public GameObject particle;
    public MeshRenderer meshRenderer;
    public PhotonView view;
    public int damage;
    private bool isCollison;
    
    private void Start()
    {
        allTileMap = FindObjectOfType<AllTileMap>();
        StartCoroutine("BallisticFall");
        Destroy(gameObject, 3);
    }
    void Update()
    {
        if (isCollison) return;
        transform.Translate(Vector3.back * 12 * Time.deltaTime);
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
        if (isCollison) return;
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !photonView.IsMine)
        {
            view.RPC(nameof(Effect), RpcTarget.All);
        }
        else if (other.tag.StartsWith("Floor") || other.name.StartsWith("Spawner"))
        {
            StartCoroutine(nameof(Effect));
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
                !hit.transform.tag.EndsWith(view.Owner.GetPlayerNumber().ToString()) && !hit.transform.tag.EndsWith("7") &&
                !hit.transform.name.StartsWith("Spawner"))
            {
                Destroy(hit.transform.gameObject);
                allTileMap.SetPlusHasTileNum(view.Owner.GetPlayerNumber() - 1);
            }
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<Player>().Hit(10);
                Debug.Log(hit.transform.name);
            }
        }

        yield return new WaitForSeconds(3);
        view.RPC(nameof(Destroy), RpcTarget.All);
    }
    [PunRPC]
    void Destroy() => Destroy(gameObject);
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
