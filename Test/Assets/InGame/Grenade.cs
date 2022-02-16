using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using UnityEngine;
public class Grenade : MonoBehaviourPunCallbacks
{
    private bool isCollison;
    private AllTileMap allTileMap;

    public GameObject particle;
    public Rigidbody rigid;
    public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;
    public MeshRenderer childMeshRenderer;
    public PhotonView view;
    public int damage;
    public int speed;

    private void Start()
    {
        allTileMap = FindObjectOfType<AllTileMap>();
        rigid.AddForce(-transform.forward * speed + Vector3.up * 10);
        StartCoroutine("BallisticFall");
    }
    #region 총알 궤적 설정
    IEnumerator BallisticFall()
    {
        yield return new WaitForSeconds(1.5f);
        Raycasting();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
    #endregion
    #region 탄두 충돌 검사
    private void OnCollisionEnter(Collision collision)
    {
        /*
        if (isCollison || view.IsMine) return;
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            other.transform.GetComponent<Player>().Hit(damage);
            isCollison = true;
            Raycasting();
        }
        else if ((other.tag.StartsWith("Floor") || other.name.StartsWith("Spawner")))
        {
            isCollison = true; //이렇게 안하면 rpc 반응 속도때문에 Raycasting이 여러번 호출될 수 있음
            Raycasting();
        }
        */
    }
    [PunRPC]
    void Effect()
    {
        isCollison = true;
        rigid.isKinematic = true;
        meshRenderer.enabled = false;
        childMeshRenderer.enabled = false;
        meshCollider.isTrigger = true;
        particle.SetActive(true);
    }
    void Raycasting()
    {
        view.RPC(nameof(Effect), RpcTarget.All);
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.up, 0, 1 << LayerMask.NameToLayer("Destroyable"));
        bool _onDamage = false;
        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.transform.CompareTag("Player") && !_onDamage)
            {
                _onDamage = true;
                hit.transform.GetComponent<Player>().Hit(damage);
            }
            if (!hit.transform.tag.EndsWith(view.Owner.GetPlayerNumber().ToString()) && hit.transform.tag.StartsWith("Floor"))
            {
                view.RPC(nameof(FloorDestroy), RpcTarget.AllViaServer, hit.transform.name, hit.transform.parent.name);
            }
        }
    }
    [PunRPC]
    void FloorDestroy(string hitName, string hitParentName)
    {
        Transform parentObject = allTileMap.transform.Find(hitParentName);
        Transform hitObject = parentObject.Find(hitName);
        if (hitObject == null) return;

        Destroy(hitObject.gameObject);
        allTileMap.SetPlusHasTileNum(view.Owner.GetPlayerNumber() - 1);
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
