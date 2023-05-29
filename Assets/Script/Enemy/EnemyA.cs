using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class EnemyA : Enemy
{
    override protected void Start()
    {
        ///敵のscriptに基づく
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Gravity();
    }

    override protected void Update()
    {
        //アニメーターの設定
        animator.SetBool("IsBlowing",IsBlowing);
        //状態の変更
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //敵のscriptに基づく
        base.Update();
    }
}
