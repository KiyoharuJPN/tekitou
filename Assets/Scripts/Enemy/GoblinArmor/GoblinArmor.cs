using System.Collections;
using UnityEngine;

public class GoblinArmor : Enemy
{
    [Tooltip("移動速度")]
    public float movingSpeed;
    [Tooltip("待ち時間の設定")]
    public float idleTime = 2.4f;


    protected override void Start()
    {
        moveSpeed = movingSpeed * -1;           //移動速度の方向修正
        base.Start();                           //敵のscriptに基づく
    }
    protected override void Update()
    {
        //アニメ
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //状態の変更
        IsBlowing = isDestroy;
        //敵のscriptに基づく
        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        if (isDestroy)
        {
            //吹っ飛び中の煙エフェクト
            if (effectTime > effectInterval)
            {
                BlowAwayEffect();
                effectTime = 0;
            }
            else effectTime += Time.deltaTime;
            return;
        }


        //画面内にある
        if (OnCamera)
        {
            //飛ばされてない限り
            if (!isDestroy)
            {
                Movement();
            }
        }
        Gravity();
    }

    //ゴブリンの動き
    void Movement()
    {
        //移動
        if (IsMoving)
        {
            enemyRb.velocity = new Vector2(moveSpeed, enemyRb.velocity.y);
        }
    }

    void Attacking()
    {
        //アニメ調整
        IsMoving = false;
        IsAttacking = true;
    }

    public void EndAttack()
    {
        //アニメ調整
        IsAttacking = false;
        StartCoroutine(Idling());
    }

    public void AttackSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.SwingDownClub);
    }

    IEnumerator Idling()
    {
        yield return new WaitForSeconds(idleTime);
        IsMoving = true;
        AttackChecking = true;
        PlayerNotAttacked = true;
    }

    //プレイヤーが攻撃エリアにいる時の動き（AttackCheckAreaから呼ばれる）
    public override bool PlayerInAttackArea()
    {
        var InAttack = false;
        if (IsMoving && AttackChecking)
        {
            AttackChecking = false;
            Attacking();
            InAttack = true;
        }
        return InAttack;
    }

    public override bool GetPlayerAttacked()
    {
        return PlayerNotAttacked;
    }
    public override void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }
}
