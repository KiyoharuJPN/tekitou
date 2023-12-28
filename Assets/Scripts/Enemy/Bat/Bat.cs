using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    [Tooltip("移動範囲")]
    public float MoveDistance = 10;
    [Tooltip("移動速度")]
    public float MoveSpeed = 5;

    Vector2 BatStartPos, BatEndPos, LeftSpeed, RightSpeed;

    //移動方向確認用関数 0はnull、１は左、２は右
    int Direction = 0;
    //最初の移動方向を記録する関数
    enum FirstDir
    {
        none,
        Left,
        Right,
    }
    FirstDir firstDir = FirstDir.none;

    protected override void Start()
    {
        base.Start();
        //初期位置と最終の位置を記録して、移動範囲を決める
        BatStartPos = transform.position;
        BatEndPos = new Vector2(BatStartPos.x + MoveDistance, BatStartPos.y);
        //移動速度を代入する
        LeftSpeed = new Vector2(-Mathf.Abs(MoveSpeed), 0f);
        RightSpeed = new Vector2(Mathf.Abs(MoveSpeed), 0f);
        //もし最初から右へ行こうとしたら右に向かせる
        if (BatEndPos.x > transform.position.x) transform.localScale = new Vector2(-1, 1);
    }

    protected override void Update()
    {
        base.Update();
        
        // アニメーターの設定
        animator.SetBool("IsBlowing", isDestroy);
    }


    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;

        //飛ばされていない限り
        if (!isDestroy)
        {
            //画面内にいるときは動かせる
            if (OnCamera)
            {
                Movement();
            }
            else
            {
                //画面外にいるときは止まらせる
                enemyRb.velocity = Vector2.zero;
                //Debug.Log(3);
            }
        }

        //吹っ飛び中の煙エフェクト
        if (isDestroy)
        {
            if (effectTime > effectInterval)
            {
                BlowAwayEffect();
                effectTime = 0;
            }
            else effectTime += Time.deltaTime;
        }
    }

    //敵の動きや関数
    void Movement()
    {
        if (isPlayerExAttack) return;
        //移動方向を初期化して動かす
        if (Direction == 0)
        {
            SetFirstDir();
        }

        //移動関数
        if(Direction == 1 && enemyRb.velocity == Vector2.zero)
        {
            enemyRb.velocity = LeftSpeed;
        }
        else if(Direction == 2 && enemyRb.velocity == Vector2.zero)
        {
            enemyRb.velocity = RightSpeed;
        }

        //転向関数
        switch (firstDir)
        {
            case FirstDir.Left:
                if (BatEndPos.x >= transform.position.x && Direction == 1)
                {
                    TurnAround();
                }
                if(BatStartPos.x <= transform.position.x && Direction == 2)
                {
                    TurnAround();
                }
                break;
            case FirstDir.Right:
                if (BatEndPos.x <= transform.position.x && Direction == 2)
                {
                    TurnAround();
                }
                if (BatStartPos.x >= transform.position.x && Direction == 1)
                {
                    TurnAround();
                }
                break;
            case 0:
                Debug.Log("初期化が正しく動いていないようです");
                break;
        }
    }


    //外部関数
    //転向関数
    public override void TurnAround()
    {
        var check = true;
        //左へ行く
        if (Direction == 2 && check)
        {
            Direction = 1;
            transform.localScale = transform.localScale = new Vector2(1, 1);
            enemyRb.velocity = LeftSpeed;
            check = false;
        }
        //右へ行く
        if (Direction == 1 && check)
        {
            Direction = 2;
            transform.localScale = transform.localScale = new Vector2(-1, 1);
            enemyRb.velocity = RightSpeed;
            //check= false;
        }
    }



    //内部関数
    //方向初期化
    void SetFirstDir()
    {
        //初期化して実際に移動させる
        if (BatEndPos.x < transform.position.x)
        {
            enemyRb.velocity = LeftSpeed;
            Direction = 1;
            firstDir = FirstDir.Left;
        }
        else if (BatEndPos.x > transform.position.x)
        {
            if (transform.localScale.x != -1) transform.localScale = new Vector2(-1, 1);
            enemyRb.velocity = RightSpeed;
            Direction = 2;
            firstDir = FirstDir.Right;
        }
        else
        {
            Debug.Log("移動範囲は不可能の値になっています。");
        }
    }
}
