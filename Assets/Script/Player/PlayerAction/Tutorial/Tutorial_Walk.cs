using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (player.isSideAttack || !player.canDropAttack
            || player.isExAttack || player.isWarpDoor || player.isUpAttack)
        {
            return;
        }

        if (player.isTWalk)
        {
            MoveKay();
        }
    }

    private void FixedUpdate()
    {
        if (player.isSideAttack || !player.canDropAttack || player.isExAttack || player.isWarpDoor)
        {
            return;
        }

        if (player.canMovingCounter <= 0)
        {
            //プレイヤーの左右の移動
            player.rb.velocity = new Vector2(xSpeed, player.rb.velocity.y);
            if (player.parallaxBackground != null)
            {
                player.parallaxBackground.StartScroll(player.transform.position);
            }
        }
    }

    //キー入力されたら移動する
    private void MoveKay()
    {
        //移動キー取得
        if (player.canMove) moveInput = Input.GetAxis("Horizontal");
        if (!player.canMove)
        {
            moveInput = 0;
            player.rb.velocity = (new Vector2(0, player.rb.velocity.y));
        }

        if (moveInput == 0)
        {
            xSpeed = 0;
        }
        else if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            xSpeed = player.moveData.maxSpeed;
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            xSpeed = -player.moveData.maxSpeed;
        }

        player.isMoving = moveInput != 0;
    }
}
