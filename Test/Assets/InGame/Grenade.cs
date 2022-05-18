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
    [SerializeField] MeshRenderer childMeshRenderer;
    [SerializeField] Rigidbody rigid;
    [SerializeField] PhotonView view;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float raycastingRange;
    [SerializeField] float livingTime;
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] bool isAttachable;
    
    private IEnumerator Start()
    {
        allTileMap = FindObjectOfType<AllTileMap>();
        rigid.AddForce(-transform.forward * speed + Vector3.up * 10);
        yield return new WaitForSeconds(livingTime);
        Raycasting();
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
    #region ź�� �浹 �˻�
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
        else if ((other.tag.StartsWith("Floor") || other.name.StartsWith("Spawner")))
        {
            isCollison = true; //�̷��� ���ϸ� rpc ���� �ӵ������� Raycasting�� ������ ȣ��� �� ����
            if (isAttachable) rigid.velocity /= 2;
        }
    }
    #endregion
    #region �����ɽ�Ʈ�� ����Ʈ
    [PunRPC]
    void Effect()
    {
        isCollison = true;
        audioSource.Play();
        rigid.isKinematic = true;
        meshRenderer.enabled = false;
        childMeshRenderer.enabled = false;
        meshCollider.isTrigger = true;
        particle.SetActive(true);
    }
    void Raycasting()
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
            if (!hit.transform.tag.EndsWith(view.Owner.GetPlayerNumber().ToString()) && hit.transform.tag.StartsWith("Floor"))
            {
                view.RPC(nameof(FloorDestroy), RpcTarget.AllViaServer, hit.transform.name, hit.transform.parent.name);
            }
        }
    }
    #endregion

    //���� �߰�
    #region �ٴ� �ı�
    [PunRPC]
    void FloorDestroy(string hitName, string hitParentName)
    {
        Transform parentObject = allTileMap.transform.Find(hitParentName);
        Transform hitObject = parentObject.Find(hitName);
        if (hitObject == null) return;

        if (view.IsMine) allTileMap.SetDestroyCount();
        Destroy(hitObject.gameObject);
        allTileMap.SetPlusHasTileNum(view.Owner.GetPlayerNumber() - 1);
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
