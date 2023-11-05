using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Jump : Player_Jump
{
    TutorialPlayer tutroialPlayer;
    const float FALL_VELOCITY = 0.4f;   //落下中判定用定数（characterのVilocityがこれより大きい場合true）

    override protected void Start()
    {
        tutroialPlayer = this.gameObject.GetComponent<TutorialPlayer>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //接地状態を得る
        isGround = ground.IsGround();

        //停止
        if (tutroialPlayer.isExAttack || tutroialPlayer.isWarpDoor) return;

        tutroialPlayer.isFalling = tutroialPlayer.rb.velocity.y < -FALL_VELOCITY;

        if (tutroialPlayer.isUpAttack && !isSecondJump) canSecondJump = true;

        //ジャンプキー取得
        if (tutroialPlayer.canMove && !tutroialPlayer.isAttack && tutroialPlayer.canTJump) JumpBottan();


    }

    private void FixedUpdate()
    {
        if (tutroialPlayer.isExAttack) return;
        if (isUpAttack || !tutroialPlayer.canDropAttack || tutroialPlayer.isSideAttack)
        {
            isjump = false;
            return;
        }
        Jump();
        Gravity();

        if (isjump && player.isAttack)
        {
            isjump = false;
            jumpTime = 0;
        };
    }

    void JumpBottan()
    {

        //ジャンプ中の処理
        if (isjump)
        {
            if (!tutroialPlayer.jumpKay.IsPressed() || jumpTime >= tutroialPlayer.jumpData.maxJumpTime)
            {
                isjump = false;
                jumpTime = 0;
            }
            else if (tutroialPlayer.jumpKay.IsPressed() && jumpTime <= tutroialPlayer.jumpData.maxJumpTime)
            {
                jumpTime += Time.deltaTime;
            }
        }

        //ジャンプキー入力
        if (tutroialPlayer.jumpKay.WasPressedThisFrame())
        {
            JumpSet();
        }
    }

    //ジャンプ使用変数へのセット
    internal new void JumpSet()
    {
        //ジャンプ1段目
        if (FarstJump && !canSecondJump && isGround)
        {

            tutroialPlayer.isSquatting = true;
            FarstJump = false;
            tutroialPlayer.animator.SetBool("IsSquatting", tutroialPlayer.isSquatting);
            jumpHight = tutroialPlayer.jumpData.jumpHeight;
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            isjump = true;
            tutroialPlayer.playerSE._JumpSE();

            tutroialPlayer.animator.Play("Hero_anim_Jump_1");
        }
        //ジャンプ2段目
        else if (!FarstJump && canSecondJump && tutroialPlayer.canTAirJump)
        {
            tutroialPlayer.animator.SetTrigger("IsSecondJump");
            canSecondJump = false;
            isSecondJump = true;
            jumpHight = tutroialPlayer.jumpData.secondJumpHeight;
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            tutroialPlayer.rb.velocity = new Vector2(tutroialPlayer.rb.velocity.x, 0);
            jumpTime = 0;
            isjump = true;
            tutroialPlayer._JumpEffect();
        }
    }

    //ジャンプ移動処理
    void Jump()
    {
        if (!isjump) return;

        tutroialPlayer.isJumping = true;

        if (tutroialPlayer.jumpKay.IsPressed())
        {
            tutroialPlayer.rb.velocity = new Vector2(tutroialPlayer.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, tutroialPlayer.jumpData.speed) + jumpTime * Time.deltaTime);
        }

        if (tutroialPlayer.jumpKay.WasReleasedThisFrame())
        {
            tutroialPlayer.rb.velocity = new Vector2(tutroialPlayer.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, tutroialPlayer.jumpData.speed) + jumpTime * Time.deltaTime * 0.5f);
        }
    }

    //重力
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -tutroialPlayer.jumpData.gravity * 2);
        tutroialPlayer.rb.AddForce(myGravity);
    }
}
