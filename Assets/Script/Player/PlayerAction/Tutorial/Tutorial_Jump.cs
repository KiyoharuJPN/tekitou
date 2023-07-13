using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Jump : Player_Jump
{
    TutorialPlayer player;
    const float FALL_VELOCITY = 0.4f;   //落下中判定用定数（characterのVilocityがこれより大きい場合true）

    override protected void Start()
    {
        player = this.gameObject.GetComponent<TutorialPlayer>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

        //停止
        if (player.isExAttack || player.isWarpDoor) return;

        player.isFalling = player.rb.velocity.y < -FALL_VELOCITY;

        if (player.isUpAttack && !isSecondJump) canSecondJump = true;

        //ジャンプキー取得
        if (player.canMove && !player.isAttack && player.isTJump) JumpBottan();

        if (isjump && player.isAttack)
        {
            isjump = false;
            jumpTime = 0;
        };

    }

    private void FixedUpdate()
    {
        if (player.isExAttack) return;
        if (isUpAttack || !player.canDropAttack || player.isSideAttack)
        {
            isjump = false;
            return;
        }
        Jump();
        Gravity();
    }

    void JumpBottan()
    {

        //ジャンプ中の処理
        if (isjump)
        {
            if (!Input.GetButton("Jump") || jumpTime >= player.jumpData.maxJumpTime)
            {
                Debug.Log("ジャンプ終了");
                isjump = false;
                jumpTime = 0;
            }
            else if (Input.GetButton("Jump") && jumpTime <= player.jumpData.maxJumpTime)
            {
                jumpTime += Time.deltaTime;
            }
        }

        //ジャンプキー入力
        if (Input.GetButtonDown("Jump"))
        {
            JumpSet();
        }
    }

    //ジャンプ使用変数へのセット
    internal new void JumpSet()
    {
        //ジャンプ1段目
        if (FarstJump && !canSecondJump)
        {

            player.isSquatting = true;
            FarstJump = false;
            player.animator.SetBool("IsSquatting", player.isSquatting);
            jumpHight = player.jumpData.jumpHeight;
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            isjump = true;
            player.playerSE._JumpSE();
        }
        //ジャンプ2段目
        else if (canSecondJump && player.isTAirJump)
        {
            player.animator.SetTrigger("IsSecondJump");
            canSecondJump = false;
            isSecondJump = true;
            jumpHight = player.jumpData.secondJumpHeight;
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
            jumpTime = 0;
            isjump = true;
        }
    }

    //ジャンプ移動処理
    void Jump()
    {
        if (!isjump) return;

        player.isJumping = true;

        if (Input.GetButton("Jump"))
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, player.jumpData.speed) + jumpTime * Time.deltaTime);
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, player.jumpData.speed) + jumpTime * Time.deltaTime * 0.5f);
        }
    }

    //重力
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -player.jumpData.gravity * 2);
        player.rb.AddForce(myGravity);
    }
}
