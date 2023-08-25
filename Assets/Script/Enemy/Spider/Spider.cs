using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
{
    [Tooltip("移動速度")]
    public float MoveSpeed = 4;

    public GameObject SpiderAttackCheckArea, SpiderAttack;

    Coroutine attack;

    protected override void Start()
    {
        base.Start();
        //移動速度を内部関数に代入
        moveSpeed = -MoveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        //画面内かつ死んでいない限り
        if (OnCamera && !isDestroy)
        {
            Movement();
        }

        //アニメーターの設定
        animator.SetBool("IsWalking", IsMoving);
        animator.SetBool("IsBlowing", isDestroy);
    }
    //敵の動き関数
    void Movement()
    {
        if (IsMoving)
        {
            Moving();
        }
    }
    void Moving()
    {
        if(enemyRb.velocity == Vector2.zero)
        {
            enemyRb.velocity = new Vector2(moveSpeed, 0);
        }
    }



    //コルーチン用関数
    IEnumerator SpiderAttacking()
    {
        var i = 0;
        while (i < 25)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        SpiderAttack.SetActive(true);
        SoundManager.Instance.PlaySE(SESoundData.SE.ForefootHeavyAttack);
        while(i < 32)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        SpiderAttack.SetActive(false);
        while (i < 70)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        SpiderAttackCheckArea.SetActive(true);
        IsMoving = true;
        AttackChecking = true;
    }


    //外部関数
    public override void TurnAround()
    {
        base.TurnAround();
        enemyRb.velocity = Vector2.zero;
    }
    public override bool PlayerInAttackArea()
    {
        var InAttack = false;
        //trueの修正は各スクリプトで書いてください。
        if (IsMoving && AttackChecking)
        {
            AttackChecking = false;
            //Spider
            PlayerNotAttacked = true;
            IsMoving = false;
            attack = StartCoroutine(SpiderAttacking());
            enemyRb.velocity = Vector2.zero;
            SpiderAttackCheckArea.SetActive(false);
            //Spider
            InAttack = true;
        }
        return InAttack;
    }



    //内部関数

}
