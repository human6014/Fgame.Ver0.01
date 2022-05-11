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

    [SerializeField] MeshCollider meshCollider;
    [SerializeField] GameObject particle;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Rigidbody rigid;
    [SerializeField] PhotonView view;
    [SerializeField] int damage;
    [SerializeField] int speed;
    private void Awake() => transform.rotation *= Quaternion.Euler(0, 180, 0);
    private void Start()
    {
        allTileMap = FindObjectOfType<AllTileMap>();
        rigid.AddForce(-transform.forward * speed);
        StartCoroutine("BallisticFall");
        Destroy(gameObject, 3);
    }
    #region 탄두 궤적 설정
    private IEnumerator BallisticFall()
    {
        yield return new WaitForSeconds(0.1f);
        rigid.constraints = RigidbodyConstraints.None;
        rigid.AddTorque(-transform.right * 0.25f);
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }
    #endregion
    //탄두 충돌시 튀는 버그, 동기화 이상함
    #region 탄두 충돌 검사
    private void OnCollisionEnter(Collision collision)
    {
        if (isCollison) return;
        GameObject other = collision.gameObject;
        
        if (other.CompareTag("Player") && !other.GetComponent<PhotonView>().IsMine && photonView.IsMine)
        {
            isCollison = true;
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
    private void Raycasting()
    {
        view.RPC(nameof(Effect), RpcTarget.All);
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.up, 0, 1 << LayerMask.NameToLayer("Destroyable"));
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
    [PunRPC]
    private void Effect()
    {
        isCollison = true;
        rigid.isKinematic = true;
        meshRenderer.enabled = false;
        meshCollider.isTrigger = true;
        particle.SetActive(true);
    }
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
