using System.Collections;
using UnityEngine;
using Photon.Pun;
public class Bullet : MonoBehaviourPunCallbacks, IPunObservable
{
    private bool isCollison;
    private Rigidbody rigid;
    private AllTileMap allTileMap;
    [SerializeField] int damage;
    [SerializeField] int speed;
    public PhotonView view;
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
            if(other.GetComponent<Player>().Hit(damage)) allTileMap.SetKillCount();
            isCollison = true;
            Destroy(gameObject);
        }
        if (other.tag.Substring(0, 5) == "Floor")
        {
            isCollison = true;
            Destroy(gameObject);
        }
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}