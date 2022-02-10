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
        if ((other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !photonView.IsMine))
        {
            Debug.Log("Hit");
            isCollison = true;
            view.RPC(nameof(Effect), RpcTarget.AllViaServer);
            Raycasting();
        }
        else if (!view.IsMine && (other.tag.StartsWith("Floor") || other.name.StartsWith("Spawner")))
        {
            isCollison = true;
            view.RPC(nameof(Effect), RpcTarget.AllViaServer);
            Raycasting();
        }
    }
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
        Debug.Log("Raycasting");
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, 0.5f, Vector3.up, 0, 1 << LayerMask.NameToLayer("Destroyable"));
        
        foreach (RaycastHit hit in raycastHits)
        {
            /*
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<Player>().Hit(5);
            }
            */
            if(!hit.transform.tag.EndsWith(view.Owner.GetPlayerNumber().ToString()))
            {
                view.RPC(nameof(FloorDestroy),RpcTarget.AllViaServer, hit.transform.name,hit.transform.parent.name);
                Debug.Log(hit.transform.name);
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
        allTileMap.SetPlusHasTileNum(view.Owner.GetPlayerNumber() - 1);//문제있음
    }
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}
