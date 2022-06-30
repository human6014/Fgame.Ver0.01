using System.Collections;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    
    private AllTileMap allTileMap;
    [SerializeField] Rigidbody rigid;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] int damage;
    [SerializeField] int speed;
    private IEnumerator Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * speed);
        allTileMap = FindObjectOfType<AllTileMap>();
        yield return new WaitForSeconds(0.3f);
        rigid.constraints = RigidbodyConstraints.None;
        yield return new WaitForSeconds(2.7f);
        Destroy(gameObject);
    }
    #region 총알 충돌 검사
    private void OnCollisionEnter(Collision collision)
    {
        if (isCollison) return;
        GameObject other = collision.gameObject;

        if (other.CompareTag("Player") && !other.GetComponent<PhotonView>().IsMine && photonView.IsMine)
        {
            Destroy(gameObject);
            isCollison = true;
            photonView.RPC(nameof(OnCollisionBullet),RpcTarget.All);
            if (other.GetComponent<Player>().Hit(damage)) allTileMap.SetKillCount();
        }
        else if (other.tag.Substring(0, 5) == "Floor")
        {
            Destroy(gameObject);
            isCollison = true;
            rigid.isKinematic = true;
            photonView.RPC(nameof(OnCollisionBullet), RpcTarget.All);
        }
    }
    [PunRPC]
    private void OnCollisionBullet()
    {
        trailRenderer.enabled = false;
        meshRenderer.enabled = false;
        rigid.isKinematic = true;
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}