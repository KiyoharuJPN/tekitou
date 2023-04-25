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
    private float speed;�@�@//���݂̃X�s�[�h
    private float dashTime; //�_�b�V�����Ă��鎞��
    float moveInput; //�ړ��L�[����
    
    float timer;

    //�A�j���[�V�����p�ϐ�
    

    private void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
        speed = player.moveData.firstSpeed;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        //�ړ��L�[�擾
        moveInput = Input.GetAxis("Horizontal");

        player.isMoving = moveInput != 0;

        if (moveInput > 0 && moveInput < 0)
        {
            dashTime += Time.deltaTime;
        }

        //�摜�̔��]
        //�ړ������ɍ��킹�ĉ摜�̔��]
        if (player.isMoving)
        {
            Vector3 scale = gameObject.transform.localScale;

            if (moveInput < 0 && scale.x > 0 || moveInput > 0 && scale.x < 0)
            {
                scale.x *= -1;
            }

            gameObject.transform.localScale = scale;

            timer += Time.deltaTime;
        }
        else
        {
            timer = player.moveData.firstSpeed;
        }

        Dash();

        if (!player.isMoving)
        {
            dashTime = 0;
            speed = player.moveData.firstSpeed;
        }

        
    }

    private void FixedUpdate()
    {
        if (player.isSideAttack)
        {
            return;
        }

        if(player.canMovingCounter <= 0) 
        {
            //�v���C���[�̍��E�̈ړ�
            player.rb.velocity = new Vector2(moveInput * speed * timer, player.rb.velocity.y);

        }
    }

    //�ړ����̉�����
    void Dash()
    {
        //�_�b�V������
        if (player.isMoving && player.moveData.maxSpeed > speed)
        {
            dashTime += Time.deltaTime;

            if (dashTime > player.moveData.acceleTime)
            {
                speed += player.moveData.accele;
                dashTime = 0;
            }
        }
    }
}
