using System.Collections;
using UnityEngine;

public class GoblinArmor : Enemy
{
    [Tooltip("移動速度")]
    public float movingSpeed;
    [Tooltip("待ち時間の設定")]
    public float idleTime = 2.4f;

    //ボスが死ぬのと共に召喚された敵の動きを止める
    int enemyType = 0;      //0は普通のモンスター、1は召喚されたモンスター、2は止まるモンスター
    DemonKing BossObj;

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
        animator.SetBool("IsBlowing", isDestroy);
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
        if (OnCamera && enemyType != 2)
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

    //ボスが倒された時に止まる関連
    //死亡時処理
    protected override void OnDestroyMode()
    {
        if (this.gameObject.GetComponent<MonsterHouse_Enemy>())
        {
            this.gameObject.GetComponent<MonsterHouse_Enemy>().Destroy();
        }
        //死亡状態に変更
        if (enemyType == 1) BossObj.EnemyIsDead(gameObject);
        isDestroy = true;
        //死亡SE再生
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterKnock);
        //敵討伐数追加
        GameManager.Instance.AddKillEnemy();
        //反射用の設定に変更
        this.GetComponent<BoxCollider2D>().enabled = false;
        this.GetComponent<CircleCollider2D>().enabled = true;
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        enemyRb.gravityScale = 0;
        enemyRb.constraints = RigidbodyConstraints2D.None;
        gameObject.layer = LayerMask.NameToLayer("PinBallEnemy");

        //吹っ飛び開始
        BlownAway();
        CheckBuff();
    }


    public void BossSummonEnemy(int i,DemonKing BObj = null)
    {
        enemyType = i;
        if (BObj != null)
            BossObj = BObj;
        if(i == 2)
        {
            IsMoving = false;
            IsAttacking = false;
        }
    }
}
