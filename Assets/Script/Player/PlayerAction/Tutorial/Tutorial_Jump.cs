using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Jump : Player_Jump
{
    TutorialPlayer Tutroialplayer;
    const float FALL_VELOCITY = 0.4f;   //落下中判定用定数（characterのVilocityがこれより大きい場合true）

    override protected void Start()
    {
        Tutroialplayer = this.gameObject.GetComponent<TutorialPlayer>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

        //停止
        if (Tutroialplayer.isExAttack || Tutroialplayer.isWarpDoor) return;

        Tutroialplayer.isFalling = Tutroialplayer.rb.velocity.y < -FALL_VELOCITY;

        if (Tutroialplayer.isUpAttack && !isSecondJump) canSecondJump = true;

        //ジャンプキー取得
        if (Tutroialplayer.canMove && !Tutroialplayer.isAttack && Tutroialplayer.isTJump) JumpBottan();

        if (isjump && Tutroialplayer.isAttack)
        {
            isjump = false;
            jumpTime = 0;
        };

    }

    private void FixedUpdate()
    {
        if (Tutroialplayer.isExAttack) return;
        if (isUpAttack || !Tutroialplayer.canDropAttack || Tutroialplayer.isSideAttack)
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
            if (!Input.GetButton("Jump") || jumpTime >= Tutroialplayer.jumpData.maxJumpTime)
            {
                isjump = false;
                jumpTime = 0;
            }
            else if (Input.GetButton("Jump") && jumpTime <= Tutroialplayer.jumpData.maxJumpTime)
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

            Tutroialplayer.isSquatting = true;
            FarstJump = false;
            Tutroialplayer.animator.SetBool("IsSquatting", Tutroialplayer.isSquatting);
            jumpHight = Tutroialplayer.jumpData.jumpHeight;
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            isjump = true;
            Tutroialplayer.playerSE._JumpSE();
        }
        //ジャンプ2段目
        else if (canSecondJump && Tutroialplayer.isTAirJump)
        {
            Tutroialplayer.animator.SetTrigger("IsSecondJump");
            canSecondJump = false;
            isSecondJump = true;
            jumpHight = Tutroialplayer.jumpData.secondJumpHeight;
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            Tutroialplayer.rb.velocity = new Vector2(Tutroialplayer.rb.velocity.x, 0);
            jumpTime = 0;
            isjump = true;
        }
    }

    //ジャンプ移動処理
    void Jump()
    {
        if (!isjump) return;

        Tutroialplayer.isJumping = true;

        if (Input.GetButton("Jump"))
        {
            Tutroialplayer.rb.velocity = new Vector2(Tutroialplayer.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, Tutroialplayer.jumpData.speed) + jumpTime * Time.deltaTime);
        }

        if (Input.GetButtonUp("Jump"))
        {
            Tutroialplayer.rb.velocity = new Vector2(Tutroialplayer.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, Tutroialplayer.jumpData.speed) + jumpTime * Time.deltaTime * 0.5f);
        }
    }

    //重力
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -Tutroialplayer.jumpData.gravity * 2);
        Tutroialplayer.rb.AddForce(myGravity);
    }
}
