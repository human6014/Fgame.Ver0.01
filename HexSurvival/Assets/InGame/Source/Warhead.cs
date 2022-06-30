using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Warhead : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    private AllTileMap allTileMap;

    [SerializeField] GameObject particle;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] Rigidbody rigid;
    [SerializeField] PhotonView view;
    [SerializeField] AudioSource audioSource;
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
        yield return new WaitForSeconds(2.9f);
        Destroy(gameObject);
    }
    private void Update()
    {
        if(!isCollison) transform.forward = -rigid.velocity;
    }
    #region 탄두 충돌 검사
    private void OnCollisionEnter(Collision collision)
    {
        if (isCollison || !view.IsMine) return;
        GameObject other = collision.gameObject;
        
        if (other.CompareTag("Player") && !other.GetComponent<PhotonView>().IsMine)
        {
            isCollison = true;
            rigid.isKinematic = true;
            if (other.GetComponent<Player>().Hit(crashDamage)) allTileMap.SetKillCount();
            Raycasting();
        }
        else if (other.tag.StartsWith("Floor") || other.name.StartsWith("Spawner"))
        {
            isCollison = true;
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
        audioSource.Play();
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
            if(!hit.transform.tag.EndsWith(view.Owner.GetPlayerNumber().ToString()) && hit.transform.tag.StartsWith("Floor") && !hit.transform.name.StartsWith("Spawner"))
            {
                view.RPC(nameof(FloorDestroy),RpcTarget.AllViaServer, hit.transform.name, hit.transform.parent.name);
            }
        }
    }
    #endregion
    #region 바닥 파괴
    [PunRPC]
    private void FloorDestroy(string hitName,string hitParentName)
    {
        Transform parentObject = allTileMap.transform.Find(hitParentName);
        if(!parentObject) return;
        Transform hitObject = parentObject.Find(hitName);
        if (!hitObject) return;
        
        Destroy(hitObject.gameObject);
        if(view.IsMine) allTileMap.SetDestroyCount();
        allTileMap.SetPlusHasTileNum(view.Owner.GetPlayerNumber() - 1);
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}