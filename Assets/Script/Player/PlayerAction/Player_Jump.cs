using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class Player_Jump : MonoBehaviour
{
    PlayerController player;
    bool isGrounded = false;            //�ڒn�t���O
    const float FALL_VELOCITY = 0.4f;   //����������p�萔�icharacter��Vilocity��������傫���ꍇtrue�j

    [Header("���蔲�������肷�邩")]
    public bool checkPlatformGroud = true;
    private string platformTag = "GroundPlatform";

    internal float jumpTime = 0;
    bool isjump = false;
    internal bool FarstJump;
    internal bool canSecondJump = false;
    internal bool isSecondJump = false;
    float jumpHight;

    //�W�����v�����ۂ̈ʒu
    float jumpPos;

    //�J�����h��i�˂��h���I�����Ɏg�p�j
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("�h�ꎞ��")]
        public float Duration;
        [Tooltip("�h��̋���")]
        public float Strength;
    }

    [SerializeField]
    [Header("��ʗh��Ɋւ���")]
    public ShakeInfo _shakeInfo;
    public CameraShake shake;

    void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //��~
        if (player.isExAttack || player.isWarpDoor) return;

        player.isFalling = player.rb.velocity.y < -FALL_VELOCITY;
        if (player.isFalling)
        {
            FarstJump = false;
        }

        if(player.isUpAttack && !isSecondJump) canSecondJump = true;

        //�W�����v�L�[�擾
        if (player.canMove && !player.isAttack) JumpBottan();
    }

    private void FixedUpdate()
    {
        if (player.isExAttack) return;

        Jump();
        Gravity();
    }

    void JumpBottan()
    {
        //��i�W�����v
        if (canSecondJump)
        {

            if (Input.GetButtonDown("Jump"))
            {
                player.animator.SetTrigger("IsSecondJump");
                canSecondJump = false;
                isSecondJump = true;
                jumpHight = player.jumpData.secondJumpHeight;
                jumpPos = this.transform.position.y;
                player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
                jumpTime = 0;
                isjump = true;
            }
        }

        //���ڃW�����v
        if (Input.GetButtonDown("Jump") && FarstJump && !canSecondJump)
        {
            player.isSquatting = true;
            FarstJump = false;
            player.animator.SetBool("IsSquatting", player.isSquatting);
            jumpHight = player.jumpData.jumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            isjump = true;
        }

        //�W�����v���̏���
        if (isjump)
        {
            if (!Input.GetButton("Jump") || jumpTime >= player.jumpData.maxJumpTime)
            {
                isjump = false;
                jumpTime = 0;
            }
            else if (Input.GetButton("Jump") && jumpTime <= player.jumpData.maxJumpTime)
            {
                jumpTime += Time.deltaTime;
            }
        }
    }

    void Jump()
    {
        if (!isjump) return;

        player.isJumping = true;

        //�W�����v������������
        if (player.isJumping && player.knockBackCounter <= 0 && !player.isUpAttack)
        {
            if (jumpPos + jumpHight <= player.transform.position.y)
            {
                player.isJumping = false;
                player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
                isjump = false;
            }
        }
        
        if (Input.GetButton("Jump"))
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpData.speed + jumpTime * Time.deltaTime);
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpData.speed + jumpTime * Time.deltaTime * 0.5f);
        }
    }

    //�d��
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -player.jumpData.gravity * 2);
        player.rb.AddForce(myGravity);
    }
}
