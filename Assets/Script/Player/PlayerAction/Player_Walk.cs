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
    private float speed;　　//現在のスピード
    private float dashTime; //ダッシュしている時間
    float moveInput; //移動キー入力
    
    float timer;

    //アニメーション用変数
    

    private void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
        speed = player.moveData.firstSpeed;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        //移動キー取得
        moveInput = Input.GetAxis("Horizontal");

        player.isMoving = moveInput != 0;

        if (moveInput > 0 && moveInput < 0)
        {
            dashTime += Time.deltaTime;
        }

        //画像の反転
        //移動方向に合わせて画像の反転
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
            //プレイヤーの左右の移動
            player.rb.velocity = new Vector2(moveInput * speed * timer, player.rb.velocity.y);

        }
    }

    //移動中の加速等
    void Dash()
    {
        //ダッシュ加速
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
