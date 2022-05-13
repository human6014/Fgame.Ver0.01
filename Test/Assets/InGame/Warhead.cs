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

    [SerializeField] GameObject particle;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] Rigidbody rigid;
    [SerializeField] PhotonView view;
    [SerializeField] float raycastingRange;
    [SerializeField] int damage;
    [SerializeField] int crashDamage;
    [SerializeField] int speed;
    private void Awake() => transform.rotation *= Quaternion.Euler(0, 180, 0);
    private IEnumerator Start()
    {
        allTileMap = FindObjectOfType<AllTileMap>();
        rigid.AddForce(-transform.forward * speed);
        yield return new WaitForSeconds(0.1f);
        rigid.constraints = RigidbodyConstraints.None;
        rigid.AddTorque(-transform.right * 0.2f);
        yield return new WaitForSeconds(2.9f);
        Destroy(gameObject);
    }
    #region 탄두 충돌 검사
    private void OnCollisionEnter(Collision collision)
    {
        if (isCollison) return;
        GameObject other = collision.gameObject;
        
        if (other.CompareTag("Player") && !other.GetComponent<PhotonView>().IsMine && photonView.IsMine)
        {
            isCollison = true;
            if (other.GetComponent<Player>().Hit(crashDamage)) allTileMap.SetKillCount();
            rigid.isKinematic = true;
            Raycasting();
        }
        
        else if (other.tag.StartsWith("Floor") || other.name.StartsWith("Spawner"))
        {
            isCollison = true; //이렇게 안하면 rpc 반응 속도때문에(아마) Raycasting이 여러번 호출될 수 있음
            rigid.isKinematic = true;
            Raycasting();
        }
    }
    #endregion
    #region 레이케스트와 이펙트
    [PunRPC]
    private void Effect()
    {
        isCollison = true;
        rigid.isKinematic = true;
        meshRenderer.enabled = false;
        meshCollider.isTrigger = true;
        particle.SetActive(true);
    }
    private void Raycasting()
    {
        view.RPC(nameof(Effect), RpcTarget.All);
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, raycastingRange, Vector3.up, 0, 1 << LayerMask.NameToLayer("Destroyable"));
        bool _onDamage = false;
        foreach (RaycastHit hit in raycastHits)
        {
            if (!_onDamage && hit.transform.CompareTag("Player") && photonView.IsMine)
            {
                _onDamage = true;
                if(hit.transform.GetComponent<Player>().Hit(damage)) allTileMap.SetKillCount();
            }
            if(!hit.transform.tag.EndsWith(view.Owner.GetPlayerNumber().ToString()) && hit.transform.tag.StartsWith("Floor"))
            {
                view.RPC(nameof(FloorDestroy),RpcTarget.AllViaServer, hit.transform.name, hit.transform.parent.name);
            }
        }
    }
    #endregion

    //문제 발견
    #region 바닥 파괴
    [PunRPC]
    private void FloorDestroy(string hitName,string hitParentName)
    {
        Transform parentObject = allTileMap.transform.Find(hitParentName);
        Transform hitObject = parentObject.Find(hitName);
        if (!hitObject) return;
        
        Destroy(hitObject.gameObject);
        if(view.IsMine) allTileMap.SetDestroyCount();
        allTileMap.SetPlusHasTileNum(view.Owner.GetPlayerNumber() - 1);
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
