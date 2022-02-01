using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    int curEquip;
    float xMove,
          zMove,
          attackDelay;
    bool isWalk,
         isJump,
         isDodge,
         isAttack,
         isAttackReady,
         isJumping,
         isDodging,
         isDying,
         isEnd;
    KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4 };
    [SerializeField] float speed;
    [SerializeField] bool[] hasWeapons;
    [SerializeField] GameObject[] weapons;
    [SerializeField] GameObject child;
    [SerializeField] Image HP;
    [SerializeField] Image MP;
    [SerializeField] Text Name;
    [SerializeField] PhotonView view;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Animator anim;
    AllTileMap allTileMap;
    Weapon equipWeapon; //이게 되나?

    private void Start()
    {
        if (photonView.IsMine)
        {
            Camera.main.GetComponent<MainCamera>().target = transform;
            Name.text = PhotonNetwork.NickName;
            allTileMap = FindObjectOfType<AllTileMap>();
        }
        else Name.text = view.Owner.NickName;
        for (int i = 0; i < hasWeapons.Length; i++)
        {
            if (hasWeapons[i])
            {
                curEquip = i;
                weapons[i].SetActive(true);
                equipWeapon = weapons[i].GetComponent<Weapon>();
                break;
            }
        }
    }
    Vector3 curPos;
    Vector3 moveVec;
    void Update()
    {
        if (photonView.IsMine)
        {
            if (isDying) return;
            xMove = Input.GetAxisRaw("Horizontal");
            zMove = Input.GetAxisRaw("Vertical");
            isWalk = Input.GetButton("Walk");
            isJump = Input.GetButtonDown("Jump");
            isDodge = Input.GetButtonDown("Dodge");
            isAttack = Input.GetButtonDown("Attack");
            moveVec = new Vector3(xMove, 0, zMove).normalized;
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(keyCodes[i]))
                {
                    view.RPC("ChangeEquip", RpcTarget.All, i);
                    break;
                }
            }
            if (moveVec != Vector3.zero) transform.Translate((isWalk ? 1f : 1.5f) * speed * Time.deltaTime * Vector3.forward); //변경 고민중

            if (!isJumping && !isDodging)
                if (isWalk || (moveVec == Vector3.zero)) MP.fillAmount += Time.time * Time.deltaTime / 25f;
                else MP.fillAmount += Time.time * Time.deltaTime / 50f;

            anim.SetBool("isRun", moveVec != Vector3.zero);
            anim.SetBool("isWalk", isWalk);
            transform.LookAt(transform.position + moveVec);
            Attack();
            Jump();
            Dodge(moveVec);
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }

    #region 공격 
    void Attack() //오브젝트 폴링 활용할 것
    {
        attackDelay += Time.deltaTime;
        if (isAttack && equipWeapon.rate < attackDelay && !isDodging && !isDying)
        {
            if (equipWeapon.type == Weapon.weaponsType.Melee) anim.SetBool("isSwing", true);
            else if (equipWeapon.type == Weapon.weaponsType.Range) anim.SetBool("isShot", true);
            else if(equipWeapon.type == Weapon.weaponsType.Destroyer) anim.SetBool("isShot", true);
            equipWeapon.UseWeapons();
            attackDelay = 0;
            Invoke(nameof(SetAttackAnim), 0.1f);
        }
    }
    void SetAttackAnim()
    {
        if (equipWeapon.type == Weapon.weaponsType.Melee) anim.SetBool("isSwing", false);
        else if (equipWeapon.type == Weapon.weaponsType.Range) anim.SetBool("isShot", false);
        else anim.SetBool("isShot", false);
    }
    #endregion
    #region 점프
    void Jump()
    {
        if (isJump && !isJumping && !isDodging && MP.fillAmount >= 0.2f && !isDying)
        {
            rigid.AddForce(Vector3.up * 4f, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            MP.fillAmount -= 0.15f;
            isJumping = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Substring(0, 5) == "Floor")
        {
            anim.SetBool("isJump", false);
            isJumping = false;
            //Destroy(collision.gameObject,10f);
        }
    }
    #endregion
    #region 회피
    void Dodge(Vector3 moveVec)
    {
        if (isDodge && !isDodging && !isJumping && moveVec != Vector3.zero && MP.fillAmount >= 0.3f && !isDying)
        {
            speed *= 2.5f;
            anim.SetBool("isDodge", true);
            isDodging = true;
            MP.fillAmount -= 0.25f;
            Invoke(nameof(DodgeOut), 0.4f);
        }
    }
    public void DodgeOut()
    {
        speed *= 0.4f;
        anim.SetBool("isDodge", false);
        isDodging = false;
    }
    #endregion
    #region 피격
    public void Hit(int damage,int cause)//casuse == 0 피격, == 1 낙사 //최적화 대기중
    {
        if (isDying) return;
        view.RPC(nameof(PunHit), RpcTarget.All, damage,cause);
    }
    [PunRPC]
    public void PunHit(int damage, int cause) 
    {
        HP.fillAmount -= damage / 100f;
        if (HP.fillAmount <= 0) StartCoroutine(nameof(Respawn), cause);
    }
    #endregion
    #region 무기 교체
    [PunRPC]
    void ChangeEquip(int index)
    {
        weapons[curEquip].SetActive(false);
        weapons[index].SetActive(true);
        equipWeapon = weapons[index].GetComponent<Weapon>();
        curEquip = index;
    }
    #endregion
    #region 죽음,리스폰
    [PunRPC]
    void Recovery()
    {
        HP.fillAmount = 1;
        MP.fillAmount = 1;
    }
    IEnumerator Respawn(int cause)
    {
        isDying = true;
        if (cause == 1)
            gameObject.transform.position = allTileMap.GetSpawner(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1).position + Vector3.up;
            gameObject.transform.eulerAngles = new Vector3(-90, 180, 0); //미완성
        yield return new WaitForSeconds(5f);
        if (cause == 0)
            gameObject.transform.position = allTileMap.GetSpawner(PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1).position + Vector3.up;
            gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        view.RPC(nameof(Recovery), RpcTarget.All);
        isDying = false;
    }
    #endregion
    #region 낙사
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GameController")) view.RPC(nameof(PunHit), RpcTarget.All,1000,1);
    }
    #endregion
    #region 포탈 이동
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Portal") && Input.GetKeyDown(KeyCode.Tab))
            other.gameObject.GetComponent<Portal>().PlayerEntry(gameObject);
    }
    #endregion
    #region 위치,체력 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(MP.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            MP.fillAmount = (float)stream.ReceiveNext();
        }
    }
    #endregion
}