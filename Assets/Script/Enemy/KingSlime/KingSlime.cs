using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Enemy
{
    [Header("移動する時の高さと距離")]
    public float moveHeightForce, moveWidthForce;
    public GameObject summonSlime;

    Animator animator;                                          //敵のアニメ関数
    float movingHeight, movingWidth;                            //移動に関する内部関数
    bool KSmovingCheck = true, KSattackingCheck = true;        //判断用内部関数
    int movingCheck = 0, AttackMode = 0;
    protected override void Start()
    {
        animator = GetComponent<Animator>();    //アニメーター代入

        movingHeight = moveHeightForce;
        movingWidth = -moveWidthForce;
        base.Start();
    }

    protected override void Update()
    {
        if (!IsBlowing)
        {
            //移動関連
            if(IsMoving)KingSlimeMoving();
            //攻撃関連
            if(IsAttacking)KingSlimeAttack();
        }


        //倒されることを確認しているのはEnemyのメイン関数で行われています
        if (isDestroy && !IsBlowing)
        {   //倒されたら他のレイヤーの影響を受けないようにするDeadBossLayer
            IsBlowing = true;
            gameObject.layer = LayerMask.NameToLayer("DeadBoss");
            //KingSlimeBlowing();   //特別の動きをするために用意した関数 
        }

        //アニメーション関数の代入
        animator.SetBool("IsMoving", IsMoving);
        animator.SetInteger("AttackMode", AttackMode);
        animator.SetBool("IsBlowing", isDestroy);
        animator.SetBool("IsAnimation",false);
    }

    //キングスライムの攻撃
    void KingSlimeAttack()
    {
        if (KSattackingCheck)
        {
            KSattackingCheck = false;
            switch (AttackMode)
            {
                case 0:
                    //KSBossAtack();
                    KSBossSummon();
                    break;
                case 1:
                    KSBossSummon();
                    break;
            }
        }
    }
    IEnumerator KSBossAtack()
    {
        yield return null;
        IsMoving = true;
        AttackMode = 1;
    }
    IEnumerator KSBossSummon()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeSummon);
        if (GameObject.Find("Hero").transform.position.x > gameObject.transform.position.x && movingWidth < 0) TurnAround();
        if (GameObject.Find("Hero").transform.position.x < gameObject.transform.position.x && movingWidth > 0) TurnAround();
        yield return new WaitForSeconds(0.333f);
        var newSlime1 = Instantiate(summonSlime);
        newSlime1.GetComponent<Rigidbody2D>().AddForce(new Vector2(3, 5),ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);
        var newSlime2 = Instantiate(summonSlime);
        newSlime1.GetComponent<Rigidbody2D>().AddForce(new Vector2(4, 6), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);
        var newSlime3 = Instantiate(summonSlime);
        newSlime1.GetComponent<Rigidbody2D>().AddForce(new Vector2(5, 7), ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.76f);
        IsMoving = true;
        AttackMode = 0;
    }



    //キングスライムの移動
    void KingSlimeMoving()
    {
        if (KSmovingCheck)
        {
            KSmovingCheck = false;
            StartCoroutine(KSMovingAnim());
        }
    }
    IEnumerator KSMovingAnim()
    {
        movingCheck++;
        yield return new WaitForSeconds(0.5f);
        enemyRb.AddForce(new Vector2(movingWidth, movingHeight),ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.75f);
        if (movingCheck == 4)
        {
            movingCheck = 0;
            IsMoving = false;
            IsAttacking = true;
        }
        KSmovingCheck = true;
    }

    



    //壁に当たったら移動量を保ったまま回転を行う
    public override void TurnAround()
    {
        bool InCheck = true;
        if (transform.localScale.x == 1f && InCheck)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            InCheck = false;
        }
        if (transform.localScale.x == -1f && InCheck)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            //InCheck = false;
        }
        enemyRb.velocity = new Vector2(enemyRb.velocity.x * -1,enemyRb.velocity.y);
        movingWidth *= -1;
    }


    //重力関連
    private void FixedUpdate()
    {
        Gravity();
    }
    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }
}
