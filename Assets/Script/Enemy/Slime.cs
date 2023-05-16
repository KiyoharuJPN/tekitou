using System.Dynamic;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Slime : Enemy
{
    [Header("移動する時の高さと距離")]
    public float moveHeight, moveWidth;

    Animator animator;      //敵のアニメ関数
    float movingHeight, movingWidth;    //移動に関する内部関数
    //チェック用内部関数
    bool IsBlowing = false, IsMoving = false, moveHideFlag = false, LRmove = false;

    //float TestTime = 0f;
    override protected void Start()
    {
        //移動関係の内部関数に代入
        movingWidth = moveWidth * -1;
        movingHeight = moveHeight;
        //自分用アニメーターの代入
        animator = GetComponent<Animator>();
        ///敵のscriptに基づく
        base.Start();
    }
    override protected void Update()
    {
        //画面内にある
        if (OnCamera)
        {
            //飛ばされてない限り
            if (!IsBlowing) SlimeMove();
        }
        //TestTime += Time.deltaTime;
        //Debug.Log("moving:" + IsMoving + "\nsecond:" + TestTime);


        //アニメーターの設定
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //状態の変更
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //敵のscriptに基づく
        base.Update();
    }

    void SlimeMove()
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
    IEnumerator SetMoveFalse ()
    {
        yield return new WaitForSeconds(1f);
        IsMoving = false;
        moveHideFlag = false;
    }


}
