using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Jump : Player_Jump
{
    TutorialPlayer tutroialPlayer;
    const float FALL_VELOCITY = 0.4f;   //����������p�萔�icharacter��Vilocity��������傫���ꍇtrue�j

    override protected void Start()
    {
        tutroialPlayer = this.gameObject.GetComponent<TutorialPlayer>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //�ڒn��Ԃ𓾂�
        isGround = ground.IsGround();

        //��~
        if (tutroialPlayer.isExAttack || tutroialPlayer.isWarpDoor) return;

        tutroialPlayer.isFalling = tutroialPlayer.rb.velocity.y < -FALL_VELOCITY;

        if (tutroialPlayer.isUpAttack && !isSecondJump) canSecondJump = true;

        //�W�����v�L�[�擾
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

        //�W�����v���̏���
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

        //�W�����v�L�[����
        if (tutroialPlayer.jumpKay.WasPressedThisFrame())
        {
            JumpSet();
        }
    }

    //�W�����v�g�p�ϐ��ւ̃Z�b�g
    internal new void JumpSet()
    {
        //�W�����v1�i��
        if (FarstJump && !canSecondJump && isGround)
        {

            tutroialPlayer.isSquatting = true;
            FarstJump = false;
            tutroialPlayer.animator.SetBool("IsSquatting", tutroialPlayer.isSquatting);
            jumpHight = tutroialPlayer.jumpData.jumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            isjump = true;
            tutroialPlayer.playerSE._JumpSE();

            tutroialPlayer.animator.Play("Hero_anim_Jump_1");
        }
        //�W�����v2�i��
        else if (!FarstJump && canSecondJump && tutroialPlayer.canTAirJump)
        {
            tutroialPlayer.animator.SetTrigger("IsSecondJump");
            canSecondJump = false;
            isSecondJump = true;
            jumpHight = tutroialPlayer.jumpData.secondJumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            tutroialPlayer.rb.velocity = new Vector2(tutroialPlayer.rb.velocity.x, 0);
            jumpTime = 0;
            isjump = true;
            tutroialPlayer._JumpEffect();
        }
    }

    //�W�����v�ړ�����
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

    //�d��
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -tutroialPlayer.jumpData.gravity * 2);
        tutroialPlayer.rb.AddForce(myGravity);
    }
}
