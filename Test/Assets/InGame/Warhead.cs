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
    private GeneralManager generalManager;
    public GameObject particle;
    public MeshRenderer meshRenderer;
    public PhotonView view;
    public int damage;
    private void Start()
    {
        allTileMap = FindObjectOfType<AllTileMap>();
        generalManager = FindObjectOfType<GeneralManager>();
        StartCoroutine("BallisticFall");
        Destroy(gameObject, 3);
    }
    void Update()
    {
        if (isCollison) return;
        transform.Translate(Vector3.back * 12 * Time.deltaTime);
    }
    #region 탄두 궤적 설정
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
    #region 탄두 충돌 검사
    private void OnTriggerEnter(Collider other)
    {
        if (isCollison || view.IsMine) return;
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            other.transform.GetComponent<Player>().Hit(damage);
            isCollison = true;
            Raycasting();
        }
        else if ((other.tag.StartsWith("Floor") || other.name.StartsWith("Spawner")))
        {
            isCollison = true; //이렇게 안하면 rpc 반응 속도때문에 여러번 호출될 수 있음
            Raycasting();
        }
    }
    [PunRPC]
    void Effect()
    {
        isCollison = true;
        meshRenderer.enabled = false;
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
                hit.transform.GetComponent<Player>().Hit(10);
                _onDamage = true;
            }
            if(!hit.transform.tag.EndsWith(view.Owner.GetPlayerNumber().ToString()) &&hit.transform.tag.StartsWith("Floor"))
            {
                view.RPC(nameof(FloorDestroy),RpcTarget.AllViaServer, hit.transform.name, hit.transform.parent.name);
            }
        }
    }
    [PunRPC]
    void FloorDestroy(string hitName,string hitParentName)
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
