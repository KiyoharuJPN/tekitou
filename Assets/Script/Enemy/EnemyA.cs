using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class EnemyA : Enemy
{
    Animator animator;  //敵のアニメ関数
    //bool IsBlowing;     //飛ばされる状態のチェック
    override protected void Start()
    {
        //自分用アニメーターの代入
        animator = GetComponent<Animator>();
        ///敵のscriptに基づく
        base.Start();
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
