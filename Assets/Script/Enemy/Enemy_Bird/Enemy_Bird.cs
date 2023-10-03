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
        if (OnCamera)      //画面内にいるとき
        {
            Movement();
        }


        //アニメーターの設定
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //飛ばされているかどうかの判定
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
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

    void Attacking()
    {
        //アニメ調整
        IsAttacking = true;
        PlayerNotAttacked = true;
        IsMoving = false;

        //攻撃するときの動き
        SoundManager.Instance.PlaySE(SESoundData.SE.BirdChirping);

        //55
        //AttackChecking = true;
        //BirdAttackArea.SetActive(true);

        //66
        //IsAttacking = false;
        //IsMoving = true;
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


    /*    //飛ばされるときにコライダーを変えるとかの操作
        protected override void _Destroy()
        {
            //反射用のコライダーに変更
            this.GetComponent<BoxCollider2D>().enabled = false;
            this.GetComponent<CircleCollider2D>().enabled = true;
            enemyRb.bodyType = RigidbodyType2D.Dynamic;
            enemyRb.constraints = RigidbodyConstraints2D.None;
            CalcForceDirection();
            //吹っ飛び開始
            BoostSphere();
            isDestroy = true;
            gameObject.layer = LayerMask.NameToLayer("PinBallEnemy");
        }*/

}
