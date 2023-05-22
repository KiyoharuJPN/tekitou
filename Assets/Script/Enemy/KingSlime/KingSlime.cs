using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Enemy
{
    [Header("移動する時の高さと距離")]
    public float moveHeight, moveWidth;

    Animator animator;                          //敵のアニメ関数
    float movingHeight, movingWidth;            //移動に関する内部関数
    bool moveHideFlag = false, LRmove = false;  //判断用内部関数
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
    }

    protected override void Update()
    {
        KingSlimeMoving();



        animator.SetBool("IsMoving", IsMoving);
    }

    void KingSlimeMoving()
    {
        //着地計算


        //移動中 壁に当たった時の反転処理
        if (IsMoving)
        {
            if (enemyRb.velocity.x == 0 && LRmove)
            {
                Vector2 scale = this.transform.localScale;
                scale.x *= -1;
                this.transform.localScale = scale;
                movingWidth *= -1;
                LRmove = false;
            }
        }

        //一秒の待ち処理
        if (IsMoving && !moveHideFlag)
        {
            moveHideFlag = true;
            StartCoroutine(SetMoveFalse());
            //移動処理。
            enemyRb.AddForce(new Vector2(movingWidth, movingHeight), ForceMode2D.Impulse);
            LRmove = true;
            StartCoroutine(Landing());
        }
        if (!IsMoving && !moveHideFlag)
        {
            moveHideFlag = true;
            StartCoroutine(SetMoveTrue());
        }

    }

    //着地までの時間待ちと移動中の確認
    IEnumerator Landing()
    {
        yield return new WaitForSeconds(0.5f);
        LRmove = false;
    }
    //一秒待って移動可能にする
    IEnumerator SetMoveTrue()
    {
        yield return new WaitForSeconds(1f);
        IsMoving = true;
        moveHideFlag = false;
    }
    //一秒待って移動不可にする
    IEnumerator SetMoveFalse()
    {
        yield return new WaitForSeconds(1f);
        IsMoving = false;
        moveHideFlag = false;
    }

    private void FixedUpdate()
    {
        Gravity();
    }
    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }
}
