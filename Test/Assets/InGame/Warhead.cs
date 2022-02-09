using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

using UnityEngine.UI;

public class Warhead : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    private AllTileMap allTileMap;
    public GameObject particle;
    public MeshRenderer meshRenderer;
    public PhotonView pv;
    public int damage;
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
    #region 총알 궤적 설정
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
    #region 총알 충돌 검사
    private void OnTriggerEnter(Collider other)
    {
        if (isCollison) return;
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !photonView.IsMine)
        {
            //StartCoroutine(nameof(Effect));
            pv.RPC(nameof(Effect), RpcTarget.AllViaServer);
            Raycasting();
        }
        else if (other.tag.StartsWith("Floor") || other.name.StartsWith("Spawner"))
        {
            pv.RPC(nameof(Effect), RpcTarget.AllViaServer);
            Raycasting();
            //StartCoroutine(nameof(Effect));
        }
    }
    /*
    [PunRPC]
    void Effect() //RaycastHit rpc와 분리 -> 실행시킨 클라에서 RaycastHit 계산 -> 파괴되는 개체 rpc로 전달
    {
        bool _onDamage = false;
        isCollison = true;
        meshRenderer.enabled = false;
        particle.SetActive(true);
        
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.up, 0);
        foreach (RaycastHit hit in raycastHits)
        {
            if (!hit.transform.name.StartsWith("PerosnTileMap") && hit.transform.tag.StartsWith("Floor") &&
                !hit.transform.tag.EndsWith(pv.Owner.GetPlayerNumber().ToString()) && !hit.transform.tag.EndsWith("7") &&
                !hit.transform.name.StartsWith("Spawner"))
            {
                //Destroy(hit.transform.gameObject);
                //pv.RPC(nameof(FloorDestroy),RpcTarget.All, hit.transform.name,hit.transform.parent.name);
                FloorDestroy(hit.transform.name, hit.transform.parent.name);
                allTileMap.SetPlusHasTileNum(pv.Owner.GetPlayerNumber() - 1);
            }
            if (hit.transform.CompareTag("Player") && !_onDamage)
            {
                _onDamage = true;
                hit.transform.GetComponent<Player>().Hit(damage);
            }
        }
    }
    */
    [PunRPC]
    void Effect()
    {
        bool _onDamage = false;
        isCollison = true;
        meshRenderer.enabled = false;
        particle.SetActive(true);
    }
    void Raycasting()
    {
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.up, 0, 1 << LayerMask.NameToLayer("Destroyable"));
        foreach (RaycastHit hit in raycastHits)
        {
            if(!hit.transform.tag.EndsWith(pv.Owner.GetPlayerNumber().ToString()))
            {
                //Destroy(hit.transform.gameObject);
                pv.RPC(nameof(FloorDestroy),RpcTarget.AllViaServer, hit.transform.name,hit.transform.parent.name);
                
            }
        }
    }
    [PunRPC]
    void FloorDestroy(string hitName,string hitParentName)
    {
        Debug.Log(hitName + " " + hitParentName);
        Transform parentObject = GameObject.Find(hitParentName).transform;
        Transform hitObject = parentObject.Find(hitName);
        if (hitObject == null) return;
        Debug.Log(hitObject.name + " " + parentObject.name);
        Destroy(hitObject.gameObject);
        allTileMap.SetPlusHasTileNum(pv.Owner.GetPlayerNumber() - 1);//문제있음
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
