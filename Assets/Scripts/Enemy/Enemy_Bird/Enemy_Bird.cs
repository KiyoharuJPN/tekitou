using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bird : Enemy
{
    [Tooltip("移動速度")]
    public float movingSpeed = 0.5f;                //移動速度
    [Header("移動範囲")]
    public float moveArea = 10f;                    //移動範囲
    [Tooltip("trueは左に移動、falseは右に移動")]
    public bool LRMove;                             //移動方向
    [Tooltip("待ち時間の設定")]
    public float idleTime = 2.4f;


    //内部関数
    Vector2 MovingArea;

    protected override void Start()
    {
        MovingArea = gameObject.transform.position;
        moveSpeed = movingSpeed * -1;
        base.Start();
    }

    protected override void Update()
    {
        

        //アニメーターの設定
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //飛ばされているかどうかの判定
        IsBlowing = isDestroy;
        //Enemyの継承
        base.Update();
    }

    void Movement()
    {
        //移動範囲外に移動したら方向を変える
        if(transform.position.x >= MovingArea.x + moveArea / 2 && LRMove)
        {
            TurnAround();
        }else if (transform.position.x <= MovingArea.x - moveArea / 2 && !LRMove)
        {
            TurnAround();
        }

        //移動
        if (IsMoving)
        {
            enemyRb.velocity = new Vector2(moveSpeed, enemyRb.velocity.y);
        }
    }

    override protected void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        if (isDestroy)
        {
            //吹っ飛び中の煙エフェクト
            if (effectTime > effectInterval)
            {
                BlowAwayEffect();
                effectTime = 0;
            }
            else effectTime += Time.deltaTime;
            return;
        }

        if (OnCamera)      //画面内にいるとき
        {
            Movement();
        }

    }

    void Attacking()
    {
        //アニメ調整
        IsAttacking = true;
        PlayerNotAttacked = true;
        IsMoving = false;

        //攻撃するときの動き
        SoundManager.Instance.PlaySE(SESoundData.SE.BirdChirping);
    }
    
    public void StartAttack()
    {
        AttackChecking = true;
        
    }
    public void EndAttack()
    {
        IsAttacking = false;
        IsMoving = true;
    }


    public override void TurnAround()
    {
        LRMove = !LRMove;
        base.TurnAround();
    }

    public override bool  PlayerInAttackArea()
    {
        var InAttack = false;
        if (IsMoving && AttackChecking)
        {
            //鳥攻撃
            AttackChecking = false;
            Attacking();
            InAttack = true;
        }
        return InAttack;
    }

    public bool HadAttacked()
    {
        if (!HadAttack)
        {
            //接触ダメージをいったんなしにする
            HadAttack = true;
            StartCoroutine(HadAttackReset());
            return true;
        }
        return false;
    }

}
