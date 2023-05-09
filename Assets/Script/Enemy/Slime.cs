using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Slime : Enemy
{
    Animator animator;
    bool IsBlowing = false, IsMoving = false;
    override protected void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
    }
    override protected void Update()
    {
        SlimeMove();

        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        base.Update();
    }

    void SlimeMove()
    {
        //à⁄ìÆèàóùÅB
    }
}
