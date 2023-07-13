using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Jump : Player_Jump
{
    TutorialPlayer Tutroialplayer;
    const float FALL_VELOCITY = 0.4f;   //����������p�萔�icharacter��Vilocity��������傫���ꍇtrue�j

    override protected void Start()
    {
        Tutroialplayer = this.gameObject.GetComponent<TutorialPlayer>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

        //��~
        if (Tutroialplayer.isExAttack || Tutroialplayer.isWarpDoor) return;

        Tutroialplayer.isFalling = Tutroialplayer.rb.velocity.y < -FALL_VELOCITY;

        if (Tutroialplayer.isUpAttack && !isSecondJump) canSecondJump = true;

        //�W�����v�L�[�擾
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

        //�W�����v���̏���
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

        //�W�����v�L�[����
        if (Input.GetButtonDown("Jump"))
        {
            JumpSet();
        }
    }

    //�W�����v�g�p�ϐ��ւ̃Z�b�g
    internal new void JumpSet()
    {
        //�W�����v1�i��
        if (FarstJump && !canSecondJump)
        {

            Tutroialplayer.isSquatting = true;
            FarstJump = false;
            Tutroialplayer.animator.SetBool("IsSquatting", Tutroialplayer.isSquatting);
            jumpHight = Tutroialplayer.jumpData.jumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            isjump = true;
            Tutroialplayer.playerSE._JumpSE();
        }
        //�W�����v2�i��
        else if (canSecondJump && Tutroialplayer.isTAirJump)
        {
            Tutroialplayer.animator.SetTrigger("IsSecondJump");
            canSecondJump = false;
            isSecondJump = true;
            jumpHight = Tutroialplayer.jumpData.secondJumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            Tutroialplayer.rb.velocity = new Vector2(Tutroialplayer.rb.velocity.x, 0);
            jumpTime = 0;
            isjump = true;
        }
    }

    //�W�����v�ړ�����
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

    //�d��
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -Tutroialplayer.jumpData.gravity * 2);
        Tutroialplayer.rb.AddForce(myGravity);
    }
}
