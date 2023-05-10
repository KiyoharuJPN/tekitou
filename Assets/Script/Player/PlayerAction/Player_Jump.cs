using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class Player_Jump : MonoBehaviour
{
    PlayerController player;
    bool isGrounded = false; //�ڒn�t���O
    const float FALL_VELOCITY = 0.3f; //����������p�萔�icharacter��Vilocity��������傫���ꍇtrue�j

    float jumpTime = 0;
    bool isjump = false;
    bool canSecondJump = false;
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
        player.isFalling = player.rb.velocity.y < -FALL_VELOCITY;

        //�W�����v����
        if (player.isJumping && player.knockBackCounter <= 0)
        {
            if (jumpPos + jumpHight <= player.transform.position.y)
            {
                player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
                isjump = false;
            }
        }

        //�W�����v�L�[�擾
        JumpBottan();
    }

    private void FixedUpdate()
    {
        Jump();
        //�d��
        if (player.rb.velocity.y <= 0) 
        {
            Gravity();
        }
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
                jumpHight = player.jumpData.secondJumpHeight;
                jumpPos = this.transform.position.y;
                player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
                jumpTime = 0;
                isjump = true;
            }
        }

        //���ڃW�����v
        if (Input.GetButtonDown("Jump") && !player.isJumping && !player.isFalling && !canSecondJump)
        {
            player.isSquatting = true;
            player.animator.SetBool("IsSquatting", player.isSquatting);
            jumpHight = player.jumpData.jumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            isjump = true;
            canSecondJump = true;
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
        if (canSecondJump)
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpData.speed + jumpTime * Time.deltaTime);
        }
        else
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpData.speed + jumpTime * Time.deltaTime);
        }
    }

    //�d��
    void Gravity()
    {
        if (!player.isAttack)
        {
            player.rb.AddForce(new Vector2(0, -player.jumpData.gravity));
        }
    }


    //���n�̔���
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
        {
            if (player.isUpAttack)
            {
                return;
            }
            if (player.isFalling == true)
            {
                player.isLanding = true;
            }
            player.isSquatting = false;
            player.isJumping = false;
            
            player.rb.velocity = Vector2.zero;
            jumpTime = 0;
            canSecondJump = false;

            //�˂��h���U���I���
            if (player.isDropAttack)
            {
                shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength);
                Invoke(nameof(DropAttackOff), 0.5f);
            };
            
            Invoke("Landingoff", 0.1f);
        }
    }

    void Landingoff()
    {
        player.isLanding = false;
    }

    private void DropAttackOff()
    {
        player.isDropAttack = false;
        player.animator.SetBool("IsDropAttack", player.isDropAttack);
    }
}
