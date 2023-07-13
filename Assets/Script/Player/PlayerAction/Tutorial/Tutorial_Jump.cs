using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Jump : Player_Jump
{
    TutorialPlayer player;
    const float FALL_VELOCITY = 0.4f;   //����������p�萔�icharacter��Vilocity��������傫���ꍇtrue�j

    override protected void Start()
    {
        player = this.gameObject.GetComponent<TutorialPlayer>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

        //��~
        if (player.isExAttack || player.isWarpDoor) return;

        player.isFalling = player.rb.velocity.y < -FALL_VELOCITY;

        if (player.isUpAttack && !isSecondJump) canSecondJump = true;

        //�W�����v�L�[�擾
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

        //�W�����v���̏���
        if (isjump)
        {
            if (!Input.GetButton("Jump") || jumpTime >= player.jumpData.maxJumpTime)
            {
                Debug.Log("�W�����v�I��");
                isjump = false;
                jumpTime = 0;
            }
            else if (Input.GetButton("Jump") && jumpTime <= player.jumpData.maxJumpTime)
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

            player.isSquatting = true;
            FarstJump = false;
            player.animator.SetBool("IsSquatting", player.isSquatting);
            jumpHight = player.jumpData.jumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            isjump = true;
            player.playerSE._JumpSE();
        }
        //�W�����v2�i��
        else if (canSecondJump && player.isTAirJump)
        {
            player.animator.SetTrigger("IsSecondJump");
            canSecondJump = false;
            isSecondJump = true;
            jumpHight = player.jumpData.secondJumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
            jumpTime = 0;
            isjump = true;
        }
    }

    //�W�����v�ړ�����
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

    //�d��
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -player.jumpData.gravity * 2);
        player.rb.AddForce(myGravity);
    }
}
