using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float xMove,
          zMove;
    bool walkMove,
         jumpMove,
         dodgeMove,
         isJump,
         isDodge;
    [SerializeField] float speed;
    Rigidbody rigid;
    Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {

    }

    void Update()
    {
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");
        walkMove = Input.GetButton("Walk");
        jumpMove = Input.GetButtonDown("Jump");
        dodgeMove = Input.GetButtonDown("Dodge");

        Vector3 moveVec = new Vector3(xMove, 0, zMove).normalized;

        transform.position += moveVec * speed * (walkMove ? 1f : 1.5f) * Time.deltaTime;

        anim.SetBool("IsRun", moveVec != Vector3.zero);
        anim.SetBool("IsWalk", walkMove);

        transform.LookAt(transform.position + moveVec);
        Jump(jumpMove);
        Dodge(dodgeMove,moveVec);
    }
    void Jump(bool jump)
    {
        if (jump && !isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * 4f, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("DoJump");
            isJump = true;
        }
    }
    void Dodge(bool dodge,Vector3 moveVec)
    {
        if (dodge && !isDodge && !isJump && moveVec !=Vector3.zero)
        {
            speed *= 2.5f;
            anim.SetTrigger("DoDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        speed *= 0.4f;
        isDodge = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Substring(0,5) == "Floor" )
        {
            anim.SetBool("IsJump", false);
            isJump = false;
        }
    }
}
