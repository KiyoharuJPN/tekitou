using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    Animator animator;

    public bool IsBlowing = false, IsMoving = false, IsAttacking = false;
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
    }
    protected override void Update()
    {
        //‰æ–Ê“à‚É‚ ‚é
        if (OnCamera)
        {
            //”ò‚Î‚³‚ê‚Ä‚È‚¢ŒÀ‚è
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
        //ƒSƒuƒŠƒ“‚Ì“®‚«

    }

    void Attacking()
    {

    }
}
