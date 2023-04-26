using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class Player_Jump : MonoBehaviour
{
    PlayerController player;

    float jumpTime = 0;
    bool isjump = false;
    bool canSecondJump = false;

    //�A�j���[�V�����p

    

    void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        JumpBottan();
    }

    private void FixedUpdate()
    {
        //�W�����v
        if (isjump && player.knockBackCounter <= 0)
        {
            Jump();
        }

        //�d��
        Gravity();
    }

    void JumpBottan()
    {
        // �L�[���͎擾
        if (Input.GetButton("Jump") && !player.isJumping && !player.isFalling)
        {
            player.isSquatting = true;
            isjump = true;
            canSecondJump = true;
        }

        //��i�W�����v
        if (canSecondJump)
        {
            if (Input.GetButtonDown("Jump"))
            {
                canSecondJump = false;
                player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
                jumpTime = 0;
                isjump = true;
            }
        }

        //�W�����v���̏���
        if (isjump)
        {
            if (!Input.GetButton("Jump") || jumpTime >= player.jumpData.maxJumpTime)
            {
                isjump = false;
            }
            else if (Input.GetButton("Jump"))
            {
                jumpTime += Time.deltaTime;
            }
        }
    }

    void Jump()
    {
        player.isJumping = true;
        player.isSquatting = false;
        if (canSecondJump)
        {
            player.rb.AddForce(transform.up * player.jumpData.firstSpeed, ForceMode2D.Impulse);
        }
        else
        {
            player.rb.AddForce(transform.up * ((player.jumpData.firstSpeed / 5) * 4), ForceMode2D.Impulse);
        }
    }

    //�d��
    void Gravity()
    {
        if (player.knockBackCounter <= 0 || !player.isSideAttack)
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
            player.isJumping = false;
            
            player.rb.velocity = Vector2.zero;
            jumpTime = 0;
            canSecondJump = false;

            //�˂��h���U���I���
            player.isDropAttack = false;
            player.animator.SetBool("IsDropAttack", player.isDropAttack);
            Invoke("Landingoff", 0.01f);
        }
    }

    void Landingoff()
    {
        player.isLanding = false;
    }
}
