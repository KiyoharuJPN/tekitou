using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bird : Enemy
{
    Animator animator;

    public float movingSpeed = 0.5f;
    [Header("移動範囲")]
    public float moveArea = 10f;


    bool IsMoving = false, IsAttacking = false, IsBlowing = false;
    float moveSpeed;


    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
    }

    protected override void Update()
    {
        if (OnCamera)
        {
            Movement();
            Attacking();
        }


        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        base.Update();
    }

    void Movement()
    {
        //移動
        if (IsMoving)
        {
            enemyRb.velocity = new Vector2(moveSpeed, enemyRb.velocity.y);
        }
    }

    void Attacking()
    {

    }


    //飛ばされるときにコライダーを変えるとかの操作
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
    }
}
