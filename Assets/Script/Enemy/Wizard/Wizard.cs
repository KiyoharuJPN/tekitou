using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{
    [Tooltip("wizardが扱う攻撃")]
    public GameObject Wizard_MagicBall;

    //プレイヤーのオブジェクト
    GameObject playerObj;



    protected override void Start()
    {
        //playerのオブジェクトを取得する
        playerObj = GameObject.Find("Hero");

        base.Start();
    }

    protected override void Update()
    {
        //飛ばされていない限り動きを続く
        if (!isDestroy)
        {
            Movement();
        }

        //毎秒アニメを更新する
        animator.SetBool("IsAttacking",IsAttacking);
        animator.SetBool("IsBlowing",IsBlowing);
        //状態の変更
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //敵のscriptに基づく
        base.Update();
    }

    //動き関連の関数
    void Movement()
    {
        //プレイヤー位置を判断して、回転を行う
        LookForPlayer();


    }






    //外部接続用関数
    //方向反転
    public override void TurnAround()
    {
        //方向回転だけ
        gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);

    }




    //内部用関数
    //プレイヤー位置を判断して、回転を行う
    void LookForPlayer()
    {
        if (playerObj.transform.position.x < gameObject.transform.position.x && gameObject.transform.localScale.x > 0)
        {
            TurnAround();
        }else if(playerObj.transform.position.x > gameObject.transform.position.x && gameObject.transform.localScale.x < 0)
        {
            TurnAround();
        }
    }
    //重力関連
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Gravity();
        if(enemyRb.velocity.y > 0)
        {
            enemyRb.velocity = Vector3.zero;
        }
    }
}
