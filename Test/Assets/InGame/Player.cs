using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    int weaponIndex = -1;
    float xMove,
          zMove,
          attackDelay,
          timer;
    bool walkMove,
         jumpMove,
         dodgeMove,
         attack,
         isAttackReady,
         isJump,
         isDodge,
         isDying;
    [SerializeField] float speed;
    [SerializeField] bool[] hasWeapons;
    [SerializeField] GameObject[] weapons;
    [SerializeField] Image HP;
    [SerializeField] Image MP;
    [SerializeField] Text Name;
    [SerializeField] PhotonView view;
    [SerializeField] Weapon equipWeapon; //이게 되나?
    GameObject child;
    Rigidbody rigid;
    Animator anim;
    Transform tr;
    Vector3 curPos;
    private void Start()
    {
        timer = 0.0f;
        isDying = false;
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>();
        child = transform.Find("GameObject").gameObject;
        if (photonView.IsMine) Camera.main.GetComponent<MainCamera>().target = tr;
        for (int i = 0; i < hasWeapons.Length; i++)
        {
            if (hasWeapons[i])
            {
                weapons[i].SetActive(true);
                weaponIndex = i;
                break;
            }
        }
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            xMove = Input.GetAxisRaw("Horizontal");
            zMove = Input.GetAxisRaw("Vertical");
            walkMove = Input.GetButton("Walk");
            jumpMove = Input.GetButtonDown("Jump");
            dodgeMove = Input.GetButtonDown("Dodge");
            attack = Input.GetButtonDown("MeleeAttack");
            Vector3 moveVec = new Vector3(xMove, 0, zMove).normalized;

            if(!isDying) transform.position += (walkMove ? 1f : 1.5f) * speed * Time.deltaTime * moveVec;
            if (!isJump && !isDodge)
                if (walkMove || moveVec == Vector3.zero) MP.fillAmount += Time.time * Time.deltaTime / 25f;
                else MP.fillAmount += Time.time * Time.deltaTime / 50f;

            anim.SetBool("IsRun", moveVec != Vector3.zero);
            anim.SetBool("IsWalk", walkMove);
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
            equipWeapon.UseWeapons();
            anim.SetTrigger("doSwing");
            attackDelay = 0;
        }
    }
    void Jump()
    {
        if (jumpMove && !isJump && !isDodge && MP.fillAmount >= 0.2f && !isDying)
        {
            rigid.AddForce(Vector3.up * 3.5f, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("DoJump");
            MP.fillAmount -= 0.15f;
            isJump = true;
        }
    }
    void Dodge(Vector3 moveVec)
    {
        if (dodgeMove && !isDodge && !isJump && moveVec != Vector3.zero && MP.fillAmount >= 0.3f && !isDying)
        {
            speed *= 2.5f;
            anim.SetTrigger("DoDodge");
            isDodge = true;
            MP.fillAmount -= 0.25f;
            Invoke(nameof(DodgeOut), 0.4f);
        }
    }
    void DodgeOut()
    {
        speed *= 0.4f;
        isDodge = false;
    }
    public void Hit(int damage)
    {
        if (isDying) return;
        view.RPC("PunHit", RpcTarget.All,damage);
    }
    [PunRPC]
    void PunHit(int damage)
    {
        HP.fillAmount -= damage / 100f;
        if (HP.fillAmount <= 0)
        {
            HP.fillAmount = 0;
            isDying = true;
            Die();
        }
    }
    void Die()
    {
        gameObject.transform.position = new Vector3(0, 1, 0); //Destroy 후 재생성 고민중
        child.transform.eulerAngles = new Vector3(0, 0, 180); //미완성
        HP.fillAmount = 1;
        MP.fillAmount = 1;
        Invoke("Respawn", 5f);
    }
    void Respawn()
    {
        isDying = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Substring(0, 5) == "Floor")
        {
            anim.SetBool("IsJump", false);
            isJump = false;
            //Destroy(collision.gameObject,10f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GameManager") view.RPC("PunHit", RpcTarget.All, 1000);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(MP.fillAmount);
            Debug.Log("HP : " + HP.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            MP.fillAmount = (float)stream.ReceiveNext();
            Debug.Log("HP : " + HP.fillAmount);
        }
    }
}
