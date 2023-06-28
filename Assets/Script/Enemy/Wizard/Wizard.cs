using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{
    [Tooltip("wizardが扱う攻撃")]
    public GameObject Wizard_MagicBall,WMBAttackPos;
    [Tooltip("攻撃距離")]
    public int Distance = 15;
    [Tooltip("攻撃スピード")]
    public float MagicSpeed = 5;

    //プレイヤーのオブジェクト
    GameObject playerObj;
    Vector2 direction;


    protected override void Start()
    {
        //playerのオブジェクトを取得する
        playerObj = GameObject.Find("Hero");
        direction = new Vector2(MagicSpeed, 0);

        base.Start();
    }

    protected override void Update()
    {
        //敵のscriptに基づく
        base.Update();

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

        //プレイヤーが範囲内に入った時
        if(Mathf.Abs(gameObject.transform.position.x - playerObj.transform.position.x) < Distance)
        {
            //攻撃モーションを行う
            if (!IsAttacking) IsAttacking = true;
        }
        else
        {
            //待機モーション
            if (IsAttacking) IsAttacking = false;
        }
        
    }

    //外部接続用関数
    //方向反転
    public override void TurnAround()
    {
        //方向回転だけ
        gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
    }
    //外部じゃないけど、方向を変える時に使わなければならないコードなので並んでみた。
    //攻撃方向を反転する
    void TurnADAround()
    {
        var check = true;
        //攻撃する方向を変更する
        if (direction.x > 0 && check)
        {
            direction = new Vector2(-MagicSpeed, 0);
            check = false;
        }
        if (direction.x < 0 && check)
        {
            direction = new Vector2(MagicSpeed, 0);
            //check = false;
        }
    }




    //内部用関数
    //プレイヤー位置を判断して、回転を行う
    void LookForPlayer()
    {
        if (playerObj.transform.position.x < gameObject.transform.position.x && gameObject.transform.localScale.x > 0)
        {
            TurnAround();
            TurnADAround();
        }else if(playerObj.transform.position.x > gameObject.transform.position.x && gameObject.transform.localScale.x < 0)
        {
            TurnAround();
            TurnADAround();
        }
    }
    //重力関連
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Gravity();
        if(enemyRb.velocity.y > 0 && !isDestroy)
        {
            enemyRb.velocity = Vector3.zero;
        }
    }
    //攻撃用関数
    void ShotMagicBall()
    {
        GameObject Magic = ObjectPool.Instance.GetObject(Wizard_MagicBall);
        Magic.transform.position = WMBAttackPos.transform.position;
        Magic.GetComponent<Wizard_MagicBall>().AKForce(enemyData.attackPower, enemyData.knockBackValue, direction);
    }
}
