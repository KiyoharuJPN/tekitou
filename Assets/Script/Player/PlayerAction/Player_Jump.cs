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

    //アニメーション用

    

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
        //ジャンプ
        if (isjump && player.knockBackCounter <= 0)
        {
            Jump();
        }

        //重力
        Gravity();
    }

    void JumpBottan()
    {
        // キー入力取得
        if (Input.GetButton("Jump") && !player.isJumping && !player.isFalling)
        {
            player.isSquatting = true;
            isjump = true;
            canSecondJump = true;
        }

        //二段ジャンプ
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

        //ジャンプ中の処理
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

    //重力
    void Gravity()
    {
        if (player.knockBackCounter <= 0 || !player.isSideAttack)
        {
            player.rb.AddForce(new Vector2(0, -player.jumpData.gravity));
        }
    }


    //着地の判定
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

            //突き刺し攻撃終わり
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
