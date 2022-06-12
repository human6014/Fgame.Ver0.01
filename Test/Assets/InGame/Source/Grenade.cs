using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using UnityEngine;
public class Grenade : MonoBehaviourPunCallbacks
{
    private bool isCollison;
    private AllTileMap allTileMap;

    [SerializeField] GameObject particle;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshCollider childMeshCollider;
    [SerializeField] MeshRenderer childMeshRenderer;
    [SerializeField] Rigidbody rigid;
    [SerializeField] PhotonView view;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float raycastingRange;
    [SerializeField] float livingTime;
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] bool isAttachable;

    float tempSpeed;
    private void SetPower(float power)
    {
        Debug.Log(power);
        tempSpeed = power * 5;
    }
    private IEnumerator Start()
    {
        allTileMap = FindObjectOfType<AllTileMap>();
        //rigid.AddForce(-transform.forward * speed + Vector3.up * (speed / 2));
        rigid.AddForce(-transform.forward * tempSpeed + Vector3.up * (tempSpeed / 2));
        yield return new WaitForSeconds(livingTime);
        Raycasting();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
    #region ÅºµÎ Ãæµ¹ °Ë»ç
    private void OnCollisionEnter(Collision collision)
    {
        if (isCollison) return;
        GameObject other = collision.gameObject;

        if (other.CompareTag("Player") && !other.GetComponent<PhotonView>().IsMine && photonView.IsMine)
        {
            isCollison = true;
            if (isAttachable)
            {
                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }
        }
        else if (other.tag.StartsWith("Floor") || other.name.StartsWith("Spawner"))
        {
            isCollison = true;
            if (isAttachable) rigid.velocity /= 2;
        }
    }
    #endregion
    #region ·¹ÀÌÄÉ½ºÆ®¿Í ÀÌÆåÆ®
    [PunRPC]
    void Effect()
    {
        isCollison = true;
        audioSource.Play();
        rigid.isKinematic = true;
        particle.SetActive(true);
        meshRenderer.enabled = false;
        meshCollider.isTrigger = true;
        if (childMeshCollider) childMeshCollider.isTrigger = false;
        if (childMeshRenderer) childMeshRenderer.enabled = false;
    }
    void Raycasting()
    {
        if (!view.IsMine) return;
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
            if (!hit.transform.tag.EndsWith(view.Owner.GetPlayerNumber().ToString()) && hit.transform.tag.StartsWith("Floor") && !hit.transform.name.StartsWith("Spawner"))
            {
                view.RPC(nameof(FloorDestroy), RpcTarget.AllViaServer, hit.transform.name, hit.transform.parent.name);
            }
        }
    }
    #endregion
    #region ¹Ù´Ú ÆÄ±«
    [PunRPC]
    void FloorDestroy(string hitName, string hitParentName)
    {
        Transform parentObject = allTileMap.transform.Find(hitParentName);
        if (!parentObject) return;
        Transform hitObject = parentObject.Find(hitName);
        if (hitObject == null) return;

        if (view.IsMine) allTileMap.SetDestroyCount();
        Destroy(hitObject.gameObject);
        allTileMap.SetPlusHasTileNum(view.Owner.GetPlayerNumber() - 1);
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}