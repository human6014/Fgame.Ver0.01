using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    int curEquip,
        myIndex;
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
        KeyCode.Alpha3
    };
    [SerializeField] float speed;
    [SerializeField] GameObject[] allWeapons;
    [SerializeField] bool[] hasWeapons;
    private GameObject[] weapons = new GameObject[3]; //최적화 대기
    [SerializeField] GameObject child;
    [SerializeField] Image HP;
    [SerializeField] Image MP;
    [SerializeField] Text Name;
    [SerializeField] PhotonView view;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Animator anim;
    AllTileMap allTileMap;
    Weapon equipWeapon;

    int timer;
    private void Start()
    {
        if (photonView.IsMine)
        {
            Camera.main.GetComponent<MainCamera>().target = transform;
            myIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            Name.text = PhotonNetwork.NickName;
            allTileMap = FindObjectOfType<AllTileMap>();

            for (int i = 0; i < 3; i++)
            {
                Debug.Log(allWeapons[allTileMap.weapon[i]]);
                weapons[i] = allWeapons[allTileMap.weapon[i]];
                Debug.Log(weapons[i].transform.name);
            }
            curEquip = 0;
            weapons[0].SetActive(true);
            equipWeapon = weapons[0].GetComponent<Weapon>();
        } 
        else Name.text = view.Owner.NickName;
        
        /*
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
        */


    }
    Vector3 curPos;
    Vector3 moveVec;
    void Update()
    {
        if (photonView.IsMine && !isEnd)
        {
            if (myIndex != -1 && allTileMap.GetIsOutPlayer(myIndex - 1)) //버그 수정 대기
            {
                isEnd = true;
                PhotonNetwork.Destroy(gameObject);
            }
            if (isDying) return;
            KeyInput();
            if (xMove + zMove != 1 && xMove + zMove != -1) xMove /= 2; zMove *= 0.866f;
            moveVec = new Vector3(xMove, 0, zMove);
            if (moveVec != Vector3.zero) transform.Translate((isWalk ? 1f : 1.5f) * speed * Time.deltaTime * Vector3.forward); //변경 고민중

            if (!isJumping && !isDodging)
                if (isWalk || (moveVec == Vector3.zero)) MP.fillAmount += 0.3f * Time.deltaTime;
                else MP.fillAmount += 0.2f * Time.deltaTime;

            anim.SetBool("isRun", moveVec != Vector3.zero);
            anim.SetBool("isWalk", isWalk);
            timer++;
            if (timer % 8 == 0)
            {
                transform.LookAt(transform.position + moveVec);
                timer = 0;
            }
            
            Attack();
            Jump();
            Dodge(moveVec);
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }
    #region 키 입력
    void KeyInput()
    {
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");
        isWalk = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
        isDodge = Input.GetButtonDown("Dodge");
        isAttack = Input.GetButtonDown("Attack");

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                view.RPC(nameof(ChangeEquip), RpcTarget.All, i);
                break;
            }
        }
    }
    #endregion
    #region 공격 
    void Attack() //오브젝트 폴링 활용할 것
    {
        attackDelay += Time.deltaTime;
        if (isAttack && equipWeapon.rate < attackDelay && !isDodging && !isDying)
        {
            if (equipWeapon.type == Weapon.WeaponsType.Melee) anim.SetBool("isSwing", true);
            else if (equipWeapon.type == Weapon.WeaponsType.Range) anim.SetBool("isShot", true);
            else if (equipWeapon.type == Weapon.WeaponsType.Cannon) anim.SetBool("isShot", true);
            else if (equipWeapon.type == Weapon.WeaponsType.Throwing) anim.SetBool("isThrow", true);
            equipWeapon.UseWeapons();
            attackDelay = 0;
            Invoke(nameof(SetAttackAnim), 0.1f);
        }
    }
    void SetAttackAnim()
    {
        if (equipWeapon.type == Weapon.WeaponsType.Melee) anim.SetBool("isSwing", false);
        else if (equipWeapon.type == Weapon.WeaponsType.Range) anim.SetBool("isShot", false);
        else if (equipWeapon.type == Weapon.WeaponsType.Cannon) anim.SetBool("isShot", false);
        else if (equipWeapon.type == Weapon.WeaponsType.Throwing) anim.SetBool("isThrow", false);
    }
    #endregion
    #region 점프
    void Jump()
    {
        if (isJump && !isJumping && !isDodging && MP.fillAmount >= 0.2f && !isDying)
        {
            rigid.velocity = Vector3.up * 4.5f;
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
        }
    }
    #endregion
    #region 회피
    void Dodge(Vector3 moveVec)
    {
        if (isDodge && !isDodging  && moveVec != Vector3.zero && MP.fillAmount >= 0.3f && !isDying)
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
    public void Hit(int damage)
    {
        if (isDying) return;
        view.RPC(nameof(PunHit), RpcTarget.All, damage);
    }
    [PunRPC]
    public void PunHit(int damage)
    {
        HP.fillAmount -= damage / 100f;
        if (HP.fillAmount <= 0) StartCoroutine(nameof(Respawn));
    }
    #endregion
    #region 무기 교체
    [PunRPC]
    void ChangeEquip(int index) //버그있음
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
    [PunRPC]
    void UnRecovery()
    {
        HP.fillAmount = 0;
        MP.fillAmount = 0;
    }
    IEnumerator Respawn()//FallRespawn 통합 예정
    {
        if (isEnd || !photonView.IsMine) yield break;
        isDying = true;
        view.RPC(nameof(UnRecovery), RpcTarget.All);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        transform.eulerAngles = new Vector3(-90, 180, 0);

        yield return new WaitForSeconds(5f);
        transform.position = allTileMap.GetSpawner(myIndex - 1).position + Vector3.up;
        transform.eulerAngles = new Vector3(0, 180, 0);
        view.RPC(nameof(Recovery), RpcTarget.All);
        isDying = false;
    }
    #endregion
    #region 낙사
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GameController")) StartCoroutine(nameof(FallRespawn));
    }
    IEnumerator FallRespawn()
    {
        if (isEnd || !photonView.IsMine) yield break;
        isDying = true;
        view.RPC(nameof(UnRecovery), RpcTarget.All);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        transform.position = allTileMap.GetSpawner(myIndex - 1).position + Vector3.up;
        transform.eulerAngles = new Vector3(-90, 180, 0);

        yield return new WaitForSeconds(5f);
        transform.eulerAngles = new Vector3(0, 180, 0);
        view.RPC(nameof(Recovery), RpcTarget.All);
        isDying = false;
    }
    #endregion
    #region 포탈 이동
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Portal") && Input.GetKeyDown(KeyCode.Tab))
            other.gameObject.GetComponent<Portal>().PlayerEntry(gameObject);
    }
    #endregion
    #region 위치,MP 동기화
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