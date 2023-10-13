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
    [Header("構え時間")]
    public float StanceTime = 1;

    float movingHeight, movingWidth, movingCheck;    //移動に関する内部関数
    //チェック用内部関数
    bool BossSummon = false, BossTurn = false;
    int MovingAnim = 0;

    //float TestTime = 0f;
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

        if (enemyRb.velocity.y < -1) MovingAnim = 1;
        IsMoving = enemyRb.velocity != Vector2.zero;
        //画面内にある
        if (OnCamera)
        {
            //Debug.Log(moveWidth);
            //飛ばされてない限り
            if (!IsBlowing) SlimeMove();
        }
        //TestTime += Time.deltaTime;
        //Debug.Log("moving:" + IsMoving + "\nsecond:" + TestTime);


        //アニメーターの設定
        animator.SetInteger("MovingAnim", MovingAnim);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //状態の変更
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
    }

    void SlimeMove()
    {
        //着地計算

        //Debug.Log(MovingAnim);

        if(IsMoving && movingCheck!=0)movingCheck = 0;
        if (!IsMoving)
        {
            movingCheck += Time.deltaTime;
            if(movingCheck > StanceTime)
            {
                movingCheck = 0;
                enemyRb.AddForce(new Vector2(movingWidth, movingHeight), ForceMode2D.Impulse);
            }
        }

        
    }

    //0.1秒待って移動不可にする
    IEnumerator SetMoveFalse ()
    {
        yield return new WaitForSeconds(0.05f);
        //IsMoving = false;
        MovingAnim = 0;
    }

    protected override void OnColEnter2D(Collider2D col)
    {
        
        base.OnColEnter2D(col);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsMoving && collision.gameObject.CompareTag("Stage") && BossSummon)
        {
            BossSummon = false;
            if (BossTurn) movingWidth *= -1;
        }
        base.OnCollisionEnter2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDestroy)
        {
            if (col.CompareTag("Stage") && MovingAnim == 1)
            {
                MovingAnim = 2;
                if(enemyRb.velocity.y !< 0) enemyRb.velocity = Vector2.zero;
                StartCoroutine(SetMoveFalse());
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        Gravity();
    }

    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }
    public void SetIsMoving(bool im)
    {
        IsMoving = im;
        BossSummon = true;
    }
    public void SummonSlimeTurn()
    {
        Vector2 scale = this.transform.localScale;
        scale.x *= -1;
        this.transform.localScale = scale;
        BossTurn = true;
    }
    public override void TurnAround()
    {
        movingWidth *= -1;
        var eveloX = enemyRb.velocity.x * -1;
        enemyRb.velocity = new Vector2 (eveloX, enemyRb.velocity.y);
        base.TurnAround();
    }

    public float GetBossHP()
    {
        return hp;
    }

    public float GetBossFullHP()
    {
        return enemyData.hp;
    }
}
