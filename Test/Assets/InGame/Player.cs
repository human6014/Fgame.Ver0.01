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
         isEnd,
         isStun;
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
    GameObject players;
    AllTileMap allTileMap;
    GeneralManager generalManager;
    Weapon equipWeapon;
    int timer;

    private void Awake() => players = GameObject.Find("PlayersPool");
    private void Start()
    {
        if (photonView.IsMine)
        {
            Camera.main.GetComponent<MainCamera>().SetTarget(transform);
            myIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();
            Name.text = PhotonNetwork.NickName;
            allTileMap = FindObjectOfType<AllTileMap>();
            generalManager = FindObjectOfType<GeneralManager>();

            view.RPC(nameof(EquipWeapon), RpcTarget.All, allTileMap.GetWeapon());
        }
        else
        {
            Name.text = view.Owner.NickName;
            gameObject.name = "Player" + view.Owner.GetPlayerNumber();
        }
        transform.SetParent(players.transform);
    }
    Vector3 curPos;
    Vector3 moveVec;
    private void Update()
    {
        if (photonView.IsMine && !isEnd)
        {
            if (myIndex != -1 && allTileMap.GetIsOutPlayer(myIndex - 1))
            {
                allTileMap.SetIsOutPlayer(true, myIndex - 1);
                isEnd = true;
                PhotonNetwork.Destroy(gameObject);
                return;
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
    private void KeyInput()
    {
        if (isStun||generalManager.GetIsChatOn())return;
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
                view.RPC(nameof(ChangeWeapon), RpcTarget.All, i);
                break;
            }
        }
        
    }
    #endregion
    #region 공격 
    private void Attack() //오브젝트 폴링 활용할 것
    {
        attackDelay += Time.deltaTime;
        if (isAttack && equipWeapon.rate < attackDelay && !isDodging && !isDying)
        {
            if (equipWeapon.type == Weapon.WeaponsType.Melee) anim.SetBool("isSwing", true);
            else if (equipWeapon.type == Weapon.WeaponsType.Range) anim.SetBool("isShot", true);
            else if (equipWeapon.type == Weapon.WeaponsType.Throwing) anim.SetBool("isThrow", true);
            equipWeapon.UseWeapons(allTileMap.GetMeleeIndex());
            attackDelay = 0;
            Invoke(nameof(SetAttackAnim), 0.1f);
        }
    }
    private void SetAttackAnim()
    {
        if (equipWeapon.type == Weapon.WeaponsType.Melee) anim.SetBool("isSwing", false);
        else if (equipWeapon.type == Weapon.WeaponsType.Range) anim.SetBool("isShot", false);
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
    public bool Hit(int damage)
    {
        if (isDying) return false;
        
        view.RPC(nameof(PunHit), RpcTarget.All, damage);
        Debug.Log(HP.fillAmount);
        if (HP.fillAmount <= 0) return true;
        return false;
    }
    [PunRPC]
    private void PunHit(int damage)
    {
        HP.fillAmount -= damage / 100f;
        if (HP.fillAmount <= 0)
        {
            Debug.Log("Die");
            StartCoroutine(nameof(Respawn));
        }
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
    [PunRPC]
    private void KnockBackUp(float x, float z)
    {
        Debug.Log("때린넘 : x : " + x + " z : " + z + "\n맞은넘 : x : " + transform.localPosition.x + " z : "+ transform.localPosition.z);
        rigid.AddForce(new Vector3((transform.localPosition.x - x) * 7, 2.5f, (transform.localPosition.z - z) * 7), ForceMode.Impulse);
    }
    [PunRPC] private void StunUp() => isStun = true;
    [PunRPC] private void StunDown() => isStun = false;

    IEnumerator Slow()//Knife ,1
    {
        view.RPC(nameof(SpeedDown), RpcTarget.All);
        yield return new WaitForSeconds(3);
        view.RPC(nameof(SpeedUp), RpcTarget.All);
    }
    public void KnockBack(float x,float z) => view.RPC(nameof(KnockBackUp), RpcTarget.All, x, z); //Bat ,2
    
    IEnumerator Stun()//Hammer ,3
    {
        view.RPC(nameof(StunUp), RpcTarget.All);
        yield return new WaitForSeconds(0.5f);
        view.RPC(nameof(StunDown), RpcTarget.All);
    }
    #endregion
    #region 무기 장착,교체
    [PunRPC]
    void EquipWeapon(int[] _weapons)
    {
        for (int i = 0; i < 3; i++)
        {
            weapons[i] = allWeapons[_weapons[i]];
        }
        curEquip = 0;
        weapons[0].SetActive(true);
        equipWeapon = weapons[0].GetComponent<Weapon>();
    }
    [PunRPC]
    void ChangeWeapon(int index)
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
        allTileMap.SetDieCount();
        isDying = true;
        view.RPC(nameof(UnRecovery), RpcTarget.All);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        transform.eulerAngles = new Vector3(-90, 180, 0);

        Debug.Log("Respawn중");

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
        if (!isEnd && other.gameObject.CompareTag("GameController")) StartCoroutine(nameof(FallRespawn));
    }
    IEnumerator FallRespawn()
    {
        if (!photonView.IsMine || !allTileMap.GetSpawner(myIndex - 1)) yield break;
        allTileMap.SetDieCount();
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