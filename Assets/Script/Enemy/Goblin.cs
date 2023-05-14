using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    Animator animator;

    //public float speed;

    bool IsBlowing = false, IsMoving = false, IsAttacking = false;
    private float moveSpeed;
    protected override void Start()
    {
        moveSpeed = speed * 0.001f;
        animator = GetComponent<Animator>();
        base.Start();
    }
    protected override void Update()
    {
        //画面内にある
        if (OnCamera)
        {
            //飛ばされてない限り
            if (!isDestroy)
            {
                Movement();
                Attacking();
            }
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
        //ゴブリンの動き
        
        //崖判断
        
        //移動
    }

    void Attacking()
    {
        //攻撃するときの動き
        
    }
}
