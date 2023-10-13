using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class SlimeArmor : Enemy
{
    [Header("移動する時の高さと距離")]
    public float moveHeight, moveWidth;
    [Header("構え時間")]
    public float StanceTime = 1;

    float movingHeight, movingWidth, movingCheck;    //移動に関する内部関数
    int MovingAnim = 0;

    override protected void Start()
    {
        //移動関係の内部関数に代入
        movingWidth = moveWidth * -1;
        movingHeight = moveHeight;
        IsMoving = false;
        ///敵のscriptに基づく
        base.Start();
    }
    override protected void Update()
    {
        //敵のscriptに基づく
        base.Update();
        if (enemyRb.velocity.y < -1)
        {
            MovingAnim = 1;
        }
        IsMoving = MovingAnim == 3 ? true : enemyRb.velocity != Vector2.zero;
        //画面内にある
        if (OnCamera)
        {
            //飛ばされてない限り
            if (!IsBlowing) SlimeMove();
        }


        //アニメーターの設定
        animator.SetInteger("MovingAnim", MovingAnim);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //状態の変更
        IsBlowing = isDestroy;
    }

    void SlimeMove()
    {
        //着地計算

        //Debug.Log(MovingAnim);

        if (IsMoving && movingCheck != 0) movingCheck = 0;
        if (!IsMoving && MovingAnim != 3)
        {
            movingCheck += Time.deltaTime;
            if (movingCheck > StanceTime)
            {
                movingCheck = 0;
                MovingAnim = 3;

                Debug.Log("in 3");
            }
        }

    }

    IEnumerator SetMoveFalse()
    {
        yield return new WaitForSeconds(0.05f);
        MovingAnim = 0;

        Debug.Log("in 0");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Ontrigger");
        if (!isDestroy)
        {
            if (col.CompareTag("Stage") && MovingAnim == 1)
            {
                MovingAnim = 2;

                Debug.Log("in 2");
                if (enemyRb.velocity.y! < 0) enemyRb.velocity = Vector2.zero;
                StartCoroutine(SetMoveFalse());
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        Gravity();
    }

    protected void SlimeJump()
    {
        enemyRb.AddForce(new Vector2(movingWidth, movingHeight), ForceMode2D.Impulse);
    }

    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }
    public override void TurnAround()
    {
        movingWidth *= -1;
        var eveloX = enemyRb.velocity.x * -1;
        enemyRb.velocity = new Vector2(eveloX, enemyRb.velocity.y);
        base.TurnAround();
    }

}
