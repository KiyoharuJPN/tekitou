using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class Player_Walk : MonoBehaviour
{
    //�v���C���[�R���g���[���[�N���X
    PlayerController player;

    //�ϐ�
    private float xSpeed = 0.0f;

    private float dashSpeed;
    private float dashTime; //�_�b�V�����Ă��鎞��
    float moveInput; //�ړ��L�[����
    
    float timer;

    //�A�j���[�V�����p�ϐ�
    

    private void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        MoveKay();
        Dash();

        if (!player.isMoving)
        {
            dashTime = 0;
            dashSpeed = 0.0f;
            player.isRun = false;
        }
    }

    private void FixedUpdate()
    {
        if (player.isSideAttack || player.isDropAttack)
        {
            return;
        }

        if (player.canMovingCounter <= 0) 
        {
            //�v���C���[�̍��E�̈ړ�
            player.rb.velocity = new Vector2(xSpeed + dashSpeed, player.rb.velocity.y);
        }
    }

    //�L�[���͂��ꂽ��ړ�����
    private void MoveKay()
    {
        //�ړ��L�[�擾
        moveInput = Input.GetAxis("Horizontal");

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            xSpeed = JumpCheck();

        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            xSpeed = -JumpCheck();
        }
        else
        {
            xSpeed = 0.0f;
        }

        player.isMoving = moveInput != 0;
    }

    //�W�����v�����ǂ���
    float JumpCheck()
    {
        if(player.isJumping || player.isFalling)
        {
            return player.moveData.jumpFirstSpeed;
        }
        else { return player.moveData.firstSpeed; }
    }

    //�ړ����̉�����
    void Dash()
    {
        //�_�b�V������
        if (player.isMoving 
            && ((player.moveData.maxSpeed >= (xSpeed + dashSpeed)) 
            && (-player.moveData.maxSpeed <= (xSpeed + dashSpeed))))
        {
            dashTime += Time.deltaTime;
            
            if (dashTime > player.moveData.acceleTime)
            {
                Debug.Log("����");
                dashSpeed += DirectionChack();
                dashTime = 0;
                if ((xSpeed + dashSpeed) >= player.moveData.dashSpeed)
                {
                    player.isRun = true;
                }
            }
        }
    }

    float DirectionChack()
    {
        if (moveInput > 0)
        {
            if ((xSpeed + dashSpeed) >= player.moveData.dashSpeed)
            {
                player.isRun = true;
            }
            return player.moveData.accele;
        }
        else if (moveInput < 0)
        {
            if ((xSpeed + dashSpeed) <= -player.moveData.dashSpeed)
            {
                player.isRun = true;
            }
            return -player.moveData.accele;
        }
        else return 0.0f;
    }
}
