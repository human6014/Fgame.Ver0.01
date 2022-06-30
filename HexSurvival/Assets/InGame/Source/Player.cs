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
                 rotateTimer;
    private float xMove,
                  zMove,
                  attackDelay,
                  changeAttackDelay,
                  teleDelay,
                  chargingTime;
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
                 isCrash,
                 isCharging,
                 isChargingOff,
                 animFlag;
    KeyCode[] keyCodes = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3
    };
    Weapon.WeaponsType type;
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
    [SerializeField] Transform bulletPosTransform;
    [SerializeField] PlayerTrajectory playerTrajectory;

    private MeshRenderer[] meshRenderer;
    private GameObject[] weapons = new GameObject[3];
    private GameObject playersPool;
    private GeneralManager generalManager;
    private AllTileMap allTileMap;
    private Weapon equipWeapon;

    private void Awake() => playersPool = GameObject.Find("PlayersPool");
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
        transform.SetParent(playersPool.transform);
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
            rotateTimer++;
            if (rotateTimer % 5 == 0)
            {
                transform.LookAt(transform.position + moveVec);
                rotateTimer = 0;
            }

            attackDelay += Time.deltaTime;
            if (changeAttackDelay < 10) changeAttackDelay += Time.deltaTime;
            if (type == Weapon.WeaponsType.Throwing)
            {
                if (isCharging && Attackable())
                {
                    if (chargingTime <= 3)
                    {
                        anim.SetBool("isThrow", true);
                        if (!animFlag)
                        {
                            animFlag = true;
                            Invoke(nameof(StopAnim), 0.1f);
                        }
                        chargingTime += Time.deltaTime;
                    }
                }
                if (isChargingOff && chargingTime > 0) ThrowAttack();
                if (!isCharging || isDodge || isDying || isSwaping)
                {
                    playerTrajectory.RenderOff();
                    chargingTime = 0;
                }
            }
            else Attack();
            Jump();
            Dodge(moveVec);
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }
    private void FixedUpdate()
    {
        isCrash = Physics.Raycast(transform.position, transform.forward, 0.3f, LayerMask.GetMask("Destroyable"));
        if (isCharging && Attackable() && chargingTime != 0) playerTrajectory.DrawTrajectory(chargingTime);
    }
    
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
        isCharging = Input.GetKey(KeyCode.Z);
        isChargingOff = Input.GetKeyUp(KeyCode.Z);
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
                playerTrajectory.RenderOff();
                view.RPC(nameof(ChangeWeapon), RpcTarget.All, i);
                break;
            }
        }
    }
    #endregion
    #region 공격 
    private void StopAnim() => anim.speed = 0;
    bool Attackable() => (equipWeapon.GetRate() < attackDelay && !isDodging && !isDying && !isSwaping);
    private void Attack()
    {
        if (isAttack && Attackable())
        {
            if (type == Weapon.WeaponsType.Melee) anim.SetBool("isSwing", true);
            else if (type == Weapon.WeaponsType.Range) anim.SetBool("isShot", true);
            Invoke(nameof(SetAttackAnim), 0.1f);
            equipWeapon.UseWeapons(generalManager.GetMeleeIndex(), chargingTime);
            attackDelay = 0;
        }
    }
    private void ThrowAttack()
    {
        if (Attackable())
        {
            anim.speed = 1;
            Invoke(nameof(SetAttackAnim), 0.1f);
            equipWeapon.UseWeapons(generalManager.GetMeleeIndex(), chargingTime);
            attackDelay = 0;
            playerTrajectory.RenderOff();
        }
    }
    private void SetAttackAnim()
    {
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
            anim.SetTrigger("doJump");
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
        if (isDying) return false;
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
        if (isDying) return;
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
        view.RPC(nameof(StunUp), RpcTarget.All);
        yield return new WaitForSeconds(1f);
        view.RPC(nameof(StunDown), RpcTarget.All);
    }
    public void ChargingCancel()
    {
        if (type != Weapon.WeaponsType.Throwing) return;
        view.RPC(nameof(PunCharingCancel), RpcTarget.All);
    }
    [PunRPC]
    public void PunCharingCancel()
    {
        chargingTime = 0;
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
        type = equipWeapon.GetWeaponsType();
    }
    [PunRPC]
    void ChangeWeapon(int index)
    {
        anim.SetBool("isThrow", false);
        anim.speed = 1;
        anim.SetBool("isShot", false);
        anim.SetBool("isSwap", true);
        isSwaping = true;
        weapons[curEquip].SetActive(false);
        weapons[index].SetActive(true);
        equipWeapon = weapons[index].GetComponent<Weapon>();
        type = equipWeapon.GetWeaponsType();
        if (type == Weapon.WeaponsType.Melee && changeAttackDelay > equipWeapon.GetRate())
        {
            changeAttackDelay = 0;
            attackDelay = 5;
        }
        else attackDelay = 0;
        curEquip = index;
        chargingTime = 0;
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
    IEnumerator Respawn(bool _isFall)
    {
        if (isEnd || !photonView.IsMine || !allTileMap.GetSpawner(myIndex - 1)) yield break;
        anim.speed = 1;
        anim.SetBool("isThrow", false);
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