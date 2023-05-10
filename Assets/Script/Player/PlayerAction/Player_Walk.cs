using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class Player_Walk : MonoBehaviour
{
    //プレイヤーコントローラークラス
    PlayerController player;

    //変数
    private float xSpeed = 0.0f;

    private float dashSpeed;
    private float dashTime; //ダッシュしている時間
    float moveInput; //移動キー入力
    
    float timer;

    //アニメーション用変数
    

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
            //プレイヤーの左右の移動
            player.rb.velocity = new Vector2(xSpeed + dashSpeed, player.rb.velocity.y);
        }
    }

    //キー入力されたら移動する
    private void MoveKay()
    {
        //移動キー取得
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

    //ジャンプ中かどうか
    float JumpCheck()
    {
        if(player.isJumping || player.isFalling)
        {
            return player.moveData.jumpFirstSpeed;
        }
        else { return player.moveData.firstSpeed; }
    }

    //移動中の加速等
    void Dash()
    {
        //ダッシュ加速
        if (player.isMoving 
            && ((player.moveData.maxSpeed >= (xSpeed + dashSpeed)) 
            && (-player.moveData.maxSpeed <= (xSpeed + dashSpeed))))
        {
            dashTime += Time.deltaTime;
            
            if (dashTime > player.moveData.acceleTime)
            {
                Debug.Log("加速");
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
