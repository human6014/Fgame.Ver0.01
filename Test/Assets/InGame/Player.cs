using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityEngine.UI;
using Photon.Pun;
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    int curEquip,
        myIndex;
    float xMove,
          zMove,
          attackDelay;
    bool walkMove,
         jumpMove,
         dodgeMove,
         attack,
         isAttackReady,
         isJump,
         isDodge,
         isDying,
         start;
    KeyCode[] keyCodes = { 
        KeyCode.Alpha1, 
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4 };
    [SerializeField] float speed;
    [SerializeField] bool[] hasWeapons;
    [SerializeField] NetworkManager networkManager;
    [SerializeField] GameObject[] weapons;
    [SerializeField] GameObject child;
    [SerializeField] Image HP;
    [SerializeField] Image MP;
    [SerializeField] Text Name;
    [SerializeField] PhotonView view;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Animator anim;
    Weapon equipWeapon; //�̰� �ǳ�?
    Vector3 curPos;
    Vector3 moveVec;
    private void Start()
    {
        isDying = false;
        if (photonView.IsMine)
        {
            Camera.main.GetComponent<MainCamera>().target = transform;
            Name.text = PhotonNetwork.NickName;
        }
        else
        {
            Name.text = view.Owner.NickName;
        }
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
    void Update()
    {
        if (!start && PhotonNetwork.CurrentRoom.MaxPlayers != PhotonNetwork.PlayerList.Length) return;
        if (photonView.IsMine)
        {
            start = true;
            if (isDying) return;
            xMove = Input.GetAxisRaw("Horizontal");
            zMove = Input.GetAxisRaw("Vertical");
            walkMove = Input.GetButton("Walk");
            jumpMove = Input.GetButtonDown("Jump");
            dodgeMove = Input.GetButtonDown("Dodge");
            attack = Input.GetButtonDown("Attack");
            moveVec = new Vector3(xMove, 0, zMove).normalized;
            for(int i = 0; i< keyCodes.Length;i++)
            {
                if (Input.GetKeyDown(keyCodes[i]))
                {
                    view.RPC("ChangeEquip",RpcTarget.All,i);
                    break;
                }
            }
            if(moveVec!=Vector3.zero)
                transform.Translate ( (walkMove ? 1f : 1.5f) * speed * Time.deltaTime * Vector3.forward); //���� �����
            
            if (!isJump && !isDodge)
                if (walkMove || moveVec == Vector3.zero) MP.fillAmount += Time.time * Time.deltaTime / 25f;
                else MP.fillAmount += Time.time * Time.deltaTime / 50f;

            anim.SetBool("isRun", moveVec != Vector3.zero);
            anim.SetBool("isWalk", walkMove);
            transform.LookAt(transform.position + moveVec);
            Attack();
            Jump();
            Dodge(moveVec);
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }
    void Attack()
    {
        attackDelay += Time.deltaTime;
        if (attack && equipWeapon.rate < attackDelay && !isDodge && !isDying)
        {
            if (equipWeapon.type == Weapon.weaponsType.Melee) anim.SetBool("isSwing", true);
            else anim.SetBool("isShot", true);
            equipWeapon.UseWeapons();
            attackDelay = 0;
            Invoke(nameof(SetAttackAnim), 0.1f);
        }
    }
    void SetAttackAnim()
    {
        if (equipWeapon.type == Weapon.weaponsType.Melee) anim.SetBool("isSwing", false);
        else anim.SetBool("isShot", false);
    }
    void Jump()
    {
        if (jumpMove && !isJump && !isDodge && MP.fillAmount >= 0.2f && !isDying)
        {
            rigid.AddForce(Vector3.up * 3.5f, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            MP.fillAmount -= 0.15f;
            isJump = true;
        }
    }
    void Dodge(Vector3 moveVec)
    {
        if (dodgeMove && !isDodge && !isJump && moveVec != Vector3.zero && MP.fillAmount >= 0.3f && !isDying)
        {
            speed *= 2.5f;
            anim.SetBool("isDodge",true);
            isDodge = true;
            MP.fillAmount -= 0.25f;
            Invoke(nameof(DodgeOut), 0.4f);
        }
    }
    public void DodgeOut()
    {
        speed *= 0.4f;
        anim.SetBool("isDodge", false);
        isDodge = false;
    }
    public void Hit(int damage)
    {
        if (isDying) return;
        view.RPC("PunHit", RpcTarget.AllBuffered, damage);
    }
    [PunRPC]
    void PunHit(int damage)
    {
        HP.fillAmount -= damage / 100f;
        if (HP.fillAmount <= 0) Die();
    }
    [PunRPC]
    void ChangeEquip(int index)
    {
        weapons[curEquip].SetActive(false);
        weapons[index].SetActive(true);
        equipWeapon = weapons[index].GetComponent<Weapon>();
        curEquip = index;
    }
    void Die() //���� �� �ٷ� �̵� -> ���� ��ġ 5�� ��� �� ����
    {
        isDying = true;
        gameObject.transform.position = new Vector3(0, 1.05f, -0.5f);
        gameObject.transform.eulerAngles = new Vector3(-90, 180, 0); //�̿ϼ�
        HP.fillAmount = 1;
        MP.fillAmount = 1;
        StartCoroutine("Respawn");
    }
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);
        gameObject.transform.position = new Vector3(0, 1.05f, 0);//������ ��ġ�� �����ؾߵ�
        gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        isDying = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Substring(0, 5) == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
            //Destroy(collision.gameObject,10f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GameManager") view.RPC("PunHit", RpcTarget.All, 1000);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Portal") && Input.GetKeyDown(KeyCode.Tab)) other.gameObject.GetComponent<Portal>().PlayerEntry(gameObject);
    }
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
}