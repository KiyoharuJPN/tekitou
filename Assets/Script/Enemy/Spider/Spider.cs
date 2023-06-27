using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
{
    [Tooltip("移動速度")]
    public float MoveSpeed = 4;

    protected override void Start()
    {
        base.Start();
        //移動速度を内部関数に代入
        moveSpeed = MoveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        //画面内かつ死んでいない限り
        if (OnCamera && !isDestroy)
        {
            Movement();
        }

        //アニメーターの設定
        //animator.SetBool("IsMoving", IsMoving);
        //animator.SetBool("IsBlowing", isDestroy);
    }
    //敵の動き関数
    void Movement()
    {
        if (IsMoving)
        {
            Moving();
        }
        else
        {
            Attacking();
        }
    }
    void Moving()
    {
        enemyRb.AddForce(new Vector2(moveSpeed, 0));
        Debug.Log(moveSpeed);
    }
    void Attacking()
    {

    }



    //アニメーション用関数



    //外部関数
    public override void TurnAround()
    {
        base.TurnAround();
    }
    public override void PlayerInAttackArea()
    {
        base.PlayerInAttackArea();
    }



    //内部関数

}
