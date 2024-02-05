using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class Tutorial_Walk : MonoBehaviour
{
    //プレイヤーコントローラークラス
    TutorialPlayer player;

    //変数
    private float xSpeed = 0.0f;

    private float dashSpeed;
    private float dashTime; //ダッシュしている時間
    internal float moveInput; //移動キー入力

    float timer;

    bool isDash = false;

    //アニメーション用変数


    private void Start()
    {
        player = this.gameObject.GetComponent<TutorialPlayer>();
    }

    private void Update()
    {
        if ((player.playerState == PlayerController.PlayerState.Idle ||
            player.playerState == PlayerController.PlayerState.NomalAttack) &&
             Time.timeScale != 0)
        {
            if (player.canTWalk)
            {
                MoveKay();
            }
        }
    }

    private void FixedUpdate()
    {
        if (player.playerState == PlayerController.PlayerState.Idle ||
            player.playerState == PlayerController.PlayerState.NomalAttack)
        {
            if (player.canMovingCounter <= 0)
            {
                //プレイヤーの左右の移動
                player.rb.velocity = new Vector2(xSpeed, player.rb.velocity.y);
            }
        }
    }

    //キー入力されたら移動する
    private void MoveKay()
    {
        //移動キー取得
        if (player.canMove) moveInput = player.move.ReadValue<Vector2>().x;
        if (!player.canMove)
        {
            moveInput = 0;
            player.rb.velocity = (new Vector2(0, player.rb.velocity.y));
        }

        if (moveInput <= 0.3 && moveInput >= -0.3)
        {
            xSpeed = 0;
        }
        else if (moveInput > 0.3)
        {
            transform.localScale = new Vector3(1, 1, 1);
            xSpeed = player.moveData.maxSpeed;
        }
        else if (moveInput < -0.3)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            xSpeed = -player.moveData.maxSpeed;
        }

        player.isMoving = (moveInput <= -0.3) || (moveInput >= 0.3);
    }
}
