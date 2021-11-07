using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    bool [] existence;
    float xMove,
          zMove;
    bool walkMove,
         jumpMove,
         dodgeMove,
         isJump,
         isDodge;
    [SerializeField] float speed;
    [SerializeField] Image HP;
    [SerializeField] Image MP;
    [SerializeField] Text Name;
    [SerializeField] PhotonView view;
    [SerializeField] NetworkManager net;
    int playerC;
    Rigidbody rigid;
    Animator anim;
    Transform tr;
    private void Awake()
    {
        existence = new bool[net.roomSize];
    }
    private void Start()
    {
        
        Debug.Log(net.roomSize);
        //existence = true;
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        tr = GetComponent<Transform>();
        if (photonView.IsMine)
            Camera.main.GetComponent<MainCamera>().target = tr;
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            Name.text = playerC.ToString();

            xMove = Input.GetAxisRaw("Horizontal");
            zMove = Input.GetAxisRaw("Vertical");
            walkMove = Input.GetButton("Walk");
            jumpMove = Input.GetButtonDown("Jump");
            dodgeMove = Input.GetButtonDown("Dodge");

            Vector3 moveVec = new Vector3(xMove, 0, zMove).normalized;

            transform.position += moveVec * speed * (walkMove ? 1f : 1.5f) * Time.deltaTime;

            if (!isJump && !isDodge)
            {
                if (walkMove)
                    MP.fillAmount+= Time.time * Time.deltaTime / 30f;
                else 
                    MP.fillAmount += Time.time * Time.deltaTime / 50f;
            }

            anim.SetBool("IsRun", moveVec != Vector3.zero);
            anim.SetBool("IsWalk", walkMove);

            transform.LookAt(transform.position + moveVec);
            Jump(jumpMove);
            Dodge(dodgeMove, moveVec);
        }
    }
    void Jump(bool jump)
    {
        if (jump && !isJump && !isDodge && MP.fillAmount >= 0.2f)
        {
            rigid.AddForce(Vector3.up * 3.5f, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("DoJump");
            MP.fillAmount -= 0.2f;
            isJump = true;
        }
    }
    void Dodge(bool dodge, Vector3 moveVec)
    {
        if (dodge && !isDodge && !isJump && moveVec != Vector3.zero && MP.fillAmount >= 0.3f)
        {
            speed *= 2.5f;
            anim.SetTrigger("DoDodge");
            isDodge = true;
            MP.fillAmount -= 0.3f;
            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        speed *= 0.4f;
        isDodge = false;
    }
    void Hit()
    {
        HP.fillAmount -= 0.1f;
        if (HP.fillAmount <= 0)
        {
            //respawn;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Substring(0, 5) == "Floor")
            anim.SetBool("IsJump", false);
            isJump = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GameManager")
            gameObject.transform.position = new Vector3(0, 1, 0);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(MP.fillAmount);
            stream.SendNext(net.playerCount);
        }
        else
        {
            MP.fillAmount = (float)stream.ReceiveNext();
            playerC += (int)stream.ReceiveNext()+1;
        }
    }
}
