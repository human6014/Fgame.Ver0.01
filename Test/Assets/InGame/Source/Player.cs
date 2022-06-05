using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    private int curEquip,
                 myIndex,
                 timer;
    private float xMove,
                  zMove,
                  attackDelay,
                  teleDelay;
    private bool isWalk,
                 isJump,
                 isDodge,
                 isAttack,
                 isAttackReady,
                 isJumping,
                 isDodging,
                 isDying,
                 isSwaping,
                 isEnd,
                 isStun,
                 isTab,
                 isTele,
                 isInPortal,
                 isCrash;
    KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3
    };

    Vector3 curPos;
    Vector3 moveVec;

    [SerializeField] float speed;
    [SerializeField] bool[] hasWeapons;
    [SerializeField] GameObject[] allWeapons;
    [SerializeField] Image HP;
    [SerializeField] Image MP;
    [SerializeField] Text Name;
    [SerializeField] PhotonView view;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource audioSource;
    [SerializeField] new Transform transform;

    private MeshRenderer[] meshRenderer;
    private GameObject[] weapons = new GameObject[3];
    private GameObject players;
    private GeneralManager generalManager;
    private AllTileMap allTileMap;
    private Weapon equipWeapon;

    private void Awake() => players = GameObject.Find("PlayersPool");
    private void Start()
    {
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
        if (photonView.IsMine)
        {
            Camera.main.GetComponent<MainCamera>().SetTarget(transform);
            myIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            Name.text = PhotonNetwork.NickName;
            allTileMap = FindObjectOfType<AllTileMap>();
            generalManager = FindObjectOfType<GeneralManager>();

            view.RPC(nameof(EquipWeapon), RpcTarget.All, generalManager.GetWeapon());
        }
        else
        {
            Name.text = view.Owner.NickName;
            gameObject.name = "Player" + view.Owner.GetPlayerNumber();
        }
        transform.SetParent(players.transform);
    }

    private void Update()
    {
        if (isEnd) return;
        if (photonView.IsMine)
        {
            if (myIndex != -1 && allTileMap.GetIsOutPlayer(myIndex - 1))
            {
                generalManager.SetRemainPlayerCount();
                isEnd = true;
                PhotonNetwork.Destroy(gameObject);
                return;
            }
            if (isDying) return;
            
            KeyInput();
            if (xMove + zMove != 1 && xMove + zMove != -1) xMove /= 2; zMove *= 0.866f;
            moveVec = new Vector3(xMove, 0, zMove);
            if (moveVec != Vector3.zero && !isCrash) transform.Translate((isWalk ? 0.8f : 1.2f) * speed * Time.deltaTime * Vector3.forward.normalized);

            if (!isJumping && !isDodging)
                if (isWalk || (moveVec == Vector3.zero)) MP.fillAmount += 0.3f * Time.deltaTime;
                else MP.fillAmount += 0.2f * Time.deltaTime;

            anim.SetBool("isWalk", isWalk && moveVec != Vector3.zero);
            anim.SetBool("isRun", moveVec != Vector3.zero);

            teleDelay += Time.deltaTime;
            timer++;
            if (timer % 5 == 0)
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
    private void FixedUpdate() => isCrash = Physics.Raycast(transform.position, transform.forward, 0.25f, LayerMask.GetMask("Destroyable"));
    #region 키 입력  
    private void KeyInput()
    {
        if (isStun || generalManager.GetIsChatOn())return;
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");
        isWalk = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
        isDodge = Input.GetButtonDown("Dodge");
        isAttack = Input.GetButtonDown("Attack");
        if (isInPortal)
        {
            isTab = Input.GetKeyDown(KeyCode.Tab);
            if (isTab && teleDelay >= 0.5f)
            {
                teleDelay = 0;
                isTele = true;
            }
        }

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                view.RPC(nameof(ChangeWeapon), RpcTarget.All, i);
                break;
            }
        }
    }
    #endregion
    #region 공격 
    private void Attack()
    {
        attackDelay += Time.deltaTime;
        if (isAttack && equipWeapon.GetRate() < attackDelay && !isDodging && !isDying && !isSwaping)
        {
            Weapon.WeaponsType type = equipWeapon.GetWeaponsType();
            if (type == Weapon.WeaponsType.Melee) anim.SetBool("isSwing", true);
            else if (type == Weapon.WeaponsType.Range) anim.SetBool("isShot", true);
            else if (type == Weapon.WeaponsType.Throwing) anim.SetBool("isThrow", true);
            equipWeapon.UseWeapons(generalManager.GetMeleeIndex());
            attackDelay = 0;
            Invoke(nameof(SetAttackAnim), 0.1f);
        }
    }
    private void SetAttackAnim()
    {
        Weapon.WeaponsType type = equipWeapon.GetWeaponsType();
        if (type == Weapon.WeaponsType.Melee) anim.SetBool("isSwing", false);
        else if (type == Weapon.WeaponsType.Range) anim.SetBool("isShot", false);
        else if (type == Weapon.WeaponsType.Throwing) anim.SetBool("isThrow", false);
    }
    #endregion
    #region 점프
    void Jump()
    {
        if (isJump && !isJumping && !isDodging && MP.fillAmount >= 0.2f && !isDying)
        {
            rigid.velocity = Vector3.up * 5;
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
    public bool Hit(int damage)
    {
        view.RPC(nameof(PunHit), RpcTarget.All, damage);
        if (HP.fillAmount <= 0)
        {
            isDying = true; //샷건 때문에 이렇게 함
            Invoke(nameof(SetIsDying), 5); //여기까지
            return true;
        }
        return false;
    }
    public void SetIsDying() => isDying = false;
    [PunRPC]
    private void PunHit(int damage)
    {
        if (isDying) return;

        HP.fillAmount -= damage / 100f;
        HP.fillAmount = (float)Math.Round(HP.fillAmount,2);
        StartCoroutine(nameof(HitDisplay));
        audioSource.Play();
        if (HP.fillAmount <= 0) StartCoroutine(nameof(Respawn), false);
    }
    IEnumerator HitDisplay()
    {
        foreach (MeshRenderer mesh in meshRenderer) mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        foreach (MeshRenderer mesh in meshRenderer) mesh.material.color = Color.white;
    }
    #endregion
    #region 근접무기 cc
    public void CrowdControl(int _ccIndex)
    {
        switch (_ccIndex)
        {
            case 0:
                StartCoroutine(nameof(Slow));
                break;
            case 2:
                StartCoroutine(nameof(Stun));
                break;
            default:
                Debug.LogError("Melee weapon index error");
                break;
        }
    }
    [PunRPC] private void SpeedDown() => speed -= 0.3f;
    [PunRPC] private void SpeedUp() => speed = 1;
    [PunRPC] private void KnockBackUp(float x, float z) => 
        rigid.AddForce(new Vector3((transform.localPosition.x - x) * 25, 10, (transform.localPosition.z - z) * 25), ForceMode.Impulse);
    [PunRPC] private void StunUp() => isStun = true;
    [PunRPC] private void StunDown() => isStun = false;

    IEnumerator Slow()//Knife ,1
    {
        if (isDying) yield break;
        view.RPC(nameof(SpeedDown), RpcTarget.All);
        yield return new WaitForSeconds(3);
        view.RPC(nameof(SpeedUp), RpcTarget.All);
    }
    public void KnockBack(float x, float z)
    {
        if (isDying) return;
        view.RPC(nameof(KnockBackUp), RpcTarget.All, x, z); //Bat ,2
    }
    
    IEnumerator Stun()//Hammer ,3
    {
        if (isDying) yield break;
        view.RPC(nameof(StunUp), RpcTarget.All);
        yield return new WaitForSeconds(0.5f);
        view.RPC(nameof(StunDown), RpcTarget.All);
    }
    #endregion
    #region 무기 장착,교체
    [PunRPC]
    void EquipWeapon(int[] _weapons)
    {
        for (int i = 0; i < 3; i++) weapons[i] = allWeapons[_weapons[i]];
        curEquip = 0;
        weapons[0].SetActive(true);
        equipWeapon = weapons[0].GetComponent<Weapon>();
    }
    [PunRPC]
    void ChangeWeapon(int index)
    {
        anim.SetBool("isSwap", true);
        isSwaping = true;
        weapons[curEquip].SetActive(false);
        weapons[index].SetActive(true);
        equipWeapon = weapons[index].GetComponent<Weapon>();
        curEquip = index;
        StartCoroutine(nameof(EndChangeAnim));
    }
    IEnumerator EndChangeAnim()
    {
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("isSwap", false);
        yield return new WaitForSeconds(0.2f);
        isSwaping = false;
    }
    #endregion
    //낙사 시 피격 판정 들어감 why??!!
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
    IEnumerator Respawn(bool _isFall)//FallRespawn 통합 예정
    {
        if (isEnd || !photonView.IsMine || !allTileMap.GetSpawner(myIndex - 1)) yield break;
        anim.SetBool("isJump", false);
        isJumping = false;
        isDying = true;
        anim.SetTrigger("isDie");
        allTileMap.SetDieCount();
        view.RPC(nameof(UnRecovery), RpcTarget.All);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        rigid.isKinematic = true;
        if(_isFall) transform.position = allTileMap.GetSpawner(myIndex - 1).position + Vector3.up;

        yield return new WaitForSeconds(5f);
        if(!_isFall)transform.position = allTileMap.GetSpawner(myIndex - 1).position + Vector3.up;
        view.RPC(nameof(Recovery), RpcTarget.All);
        rigid.isKinematic = false;
        isDying = false;
    }
    #endregion
    #region 낙사
    private void OnTriggerEnter(Collider other)
    {
        if (!isEnd && other.gameObject.CompareTag("GameController")) StartCoroutine(nameof(Respawn), true);
    }
    #endregion
    #region 포탈 이동
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Portal"))
        {
            isInPortal = true;
            if (isTele)
            {
                other.gameObject.GetComponent<Portal>().PlayerEntry(gameObject);
                isTele = false;
            }
        }   
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Portal")) isInPortal = false;
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