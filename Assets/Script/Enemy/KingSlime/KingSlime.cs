using System.Collections;
using System;
using UnityEngine;

public class KingSlime : Enemy
{
    [Header("移動する時の高さと距離")]
    public float moveHeightForce, moveWidthForce, AttackHeight = 8;
    public GameObject summonSlime, wallCheck;
    
    public BoxCollider2D attackCheckArea;
    public CircleCollider2D knockbackAttackCircle;
    public BossHPBar HPBar;



    //揺れ関連
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("揺れ時間")]
        public float Duration;
        [Tooltip("揺れの強さ")]
        public float Strength;
    }

    [SerializeField]
    [Header("画面揺れに関する")]
    public ShakeInfo _shakeInfo;
    CameraShake shake;

    float movingHeight, movingWidth, summonPosX, summonPosY;            //移動に関する内部関数
    bool KSmovingCheck = true, KSattackingCheck = true, KSNormalAttackLanding = false
        , NoGravity = false, ExSkillCheck = false, inExSkillCheck = false
        , ExSkillFalling = false, SkillTurnAround = false;                                            //判断用内部関数
    int movingCheck = 0, AttackMode = 0, NormalAttackAnimation;         //チェック用int関数
    GameObject playerObj;                                  //プレイヤーオブジェクト宣言

    //コールチーンよう判断関数
    bool inKSBossAtack1, inKSBossAtack2, inKSBossAtack3, inKSBossSummon, inKSMovingAnim;

    protected override void Start()
    {
        playerObj = GameObject.Find("Hero");    //プレイヤーオブジェクト
        movingHeight = moveHeightForce;
        movingWidth = -moveWidthForce;
        summonPosX = -5f;
        summonPosY = 1;
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        base.Start();
    }

    protected override void Update()
    {
        //Ex関係のアニメーション
        animator.SetBool("ExSkillFalling", ExSkillFalling);
        animator.SetBool("ExSkillCheck", ExSkillCheck);
        if (ExSkillCheck)
        {
            if (!inExSkillCheck)
            {
                inExSkillCheck = true;
                StopAllCoroutines();
                ClearCoroutines();
            }
            enemyRb.velocity = Vector2.zero;
            if (!isPlayerExAttack)
            {
                ExSkillCheck = false;
                StartCoroutine(SkillWait());
            }
            return;
        }
        if (isPlayerExAttack)
        {
            ExSkillCheck = true;
            enemyRb.velocity = Vector2.zero;
            return;
        }
        if (inExSkillCheck)
        {
            ExSkillFalling = enemyRb.velocity != Vector2.zero;
            if (ExSkillFalling)
            {
                StopAllCoroutines();
                return;
            }
            else
            {
                StartCoroutine(SkillWait());
            }
            
        }
        //if (Input.GetKeyDown(KeyCode.K)) shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        if (!IsBlowing)
        {
            //攻撃関連
            if (IsAttacking) KingSlimeAttack();
            //移動関連
            if (IsMoving && !inExSkillCheck)KingSlimeMoving();
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
        animator.SetInteger("NormalAttackAnimation", NormalAttackAnimation);
        animator.SetBool("IsBlowing", isDestroy);
        if(IsMoving)animator.SetBool("IsAnimation",false);
    }

    //内部動き関連の関数をここに
    IEnumerator SkillWait()
    {
        yield return new WaitForSeconds(0.1f);
        inExSkillCheck = false;
    }
    //キングスライムの攻撃用関数
    //ノーマル攻撃関数
    void KingSlimeAttack()
    {
        if (KSattackingCheck)
        {
            KSattackingCheck = false;
            switch (AttackMode)
            {
                case 0:
                    StartCoroutine(KSBossAtack1());
                    break;
                case 1:
                    StartCoroutine(KSBossSummon());
                    break;
            }
        }
    }
    IEnumerator KSBossAtack1()
    {
        wallCheck.SetActive(false);
        inKSBossAtack1 = true;
        NormalAttackAnimation = 0;
        Vector3 PlayerPos = playerObj.transform.position + new Vector3(0, AttackHeight, 0);
        var AttackMoveSpeed = (float)Math.Sqrt(((PlayerPos.x - transform.position.x) * (PlayerPos.x - transform.position.x)) + (PlayerPos.y - transform.position.y) * (PlayerPos.y - transform.position.y));
        AttackMoveSpeed /= 25;
        if (playerObj.transform.position.x > gameObject.transform.position.x && movingWidth < 0) TurnAround();
        if (playerObj.transform.position.x < gameObject.transform.position.x && movingWidth > 0) TurnAround();
        yield return new WaitForSeconds(0.25f);
        NoGravity = true;
        var i = 0;
        while(i < 25)
        {
            this.transform.position = Vector2.MoveTowards(transform.position, PlayerPos, AttackMoveSpeed);
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        enemyRb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.8f);
        NoGravity = false;
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        enemyRb.AddForce(new Vector2(0, -40),ForceMode2D.Impulse);
        knockbackAttackCircle.enabled = true;
        KSNormalAttackLanding = true;
        inKSBossAtack1 = false;
    }
    IEnumerator KSBossAtack2()
    {
        inKSBossAtack2 = true;
        SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
        shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        knockbackAttackCircle.enabled = false;
        NormalAttackAnimation++;
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        int i = 0;
        while(i < 5)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        attackCheckArea.offset = new Vector2(0, -0.2f);
        attackCheckArea.size = new Vector2(6.86f, 3.8f);
        attackCheckArea.enabled = true;
        while (i < 10)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        attackCheckArea.offset = new Vector2(0, 0.05f);
        attackCheckArea.size = new Vector2(8f, 4.3f);
        while (i < 15)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        attackCheckArea.offset = new Vector2(0, 1.55f);
        attackCheckArea.size = new Vector2(8f, 7.3f);
        while (i < 25)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        attackCheckArea.offset = new Vector2(0, -0.1f);
        attackCheckArea.size = new Vector2(7f, 4f);
        while (i < 29)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        attackCheckArea.offset = new Vector2(0, -0.1f);
        attackCheckArea.size = new Vector2(9f, 4f);
        while (i < 33)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        while (i < 37)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        attackCheckArea.offset = new Vector2(0, -0.1f);
        attackCheckArea.size = new Vector2(10f, 4f);
        while (i < 41)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        attackCheckArea.enabled = false;
        while (i < 55)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine(KSBossAtack3());
        inKSBossAtack2 = false;
    }
    IEnumerator KSBossAtack3()
    {
        SkillTurnAround = true;
        inKSBossAtack3 = true;
        NormalAttackAnimation++;

        yield return new WaitForSeconds(0.75f);

        IsAttacking = false;
        IsMoving = true;
        KSattackingCheck = true;
        AttackMode = 1;
        NormalAttackAnimation = 0;
        inKSBossAtack3 = false;
        SkillTurnAround = false;
        
    }
    //召喚攻撃関数
    IEnumerator KSBossSummon()
    {
        inKSBossSummon = true;
        SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeSummon);
        if (playerObj.transform.position.x > gameObject.transform.position.x && movingWidth < 0) TurnAround();
        if (playerObj.transform.position.x < gameObject.transform.position.x && movingWidth > 0) TurnAround();
        var summonPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2.5f, gameObject.transform.position.z);
        yield return new WaitForSeconds(0.333f);
        var newSlime1 = Instantiate(summonSlime, summonPos, Quaternion.identity);
        if (transform.localScale.x < 0) newSlime1.GetComponent<Slime>().SummonSlimeTurn();
        newSlime1.GetComponent<Slime>().SetIsMoving(false);
        newSlime1.GetComponent<Rigidbody2D>().AddForce(new Vector2(summonPosX, summonPosY),ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);
        var newSlime2 = Instantiate(summonSlime, summonPos, Quaternion.identity);
        if (transform.localScale.x < 0) newSlime2.GetComponent<Slime>().SummonSlimeTurn();
        newSlime2.GetComponent<Slime>().SetIsMoving(false);
        newSlime2.GetComponent<Rigidbody2D>().AddForce(new Vector2(summonPosX + 1, summonPosY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);
        var newSlime3 = Instantiate(summonSlime, summonPos, Quaternion.identity);
        if (transform.localScale.x < 0) newSlime3.GetComponent<Slime>().SummonSlimeTurn();
        newSlime3.GetComponent<Slime>().SetIsMoving(false);
        newSlime3.GetComponent<Rigidbody2D>().AddForce(new Vector2(summonPosX + 2, summonPosY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.76f);
        IsAttacking = false;
        IsMoving = true;
        KSattackingCheck = true;
        AttackMode = 0;
        inKSBossSummon = false;
    }


    //移動用関数
    //キングスライムの移動
    void KingSlimeMoving()
    {
        if (KSmovingCheck)
        {
            wallCheck.SetActive(true);
            KSmovingCheck = false;
            StartCoroutine(KSMovingAnim());
        }
    }
    IEnumerator KSMovingAnim()
    {
        inKSMovingAnim = true;
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
        inKSMovingAnim = false;
    }

    //キングスライム死亡時に呼ぶ関数
    public void Boss_Down()
    {
        ComboParam.Instance.ComboStop();
        GameManager.Instance.PlayerExAttack_Start();
        GameManager.Instance.Result_Start(1);
    }

    //コライダーやトリガーなどのチェック関数
    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            if (col.gameObject.CompareTag("Stage") && KSNormalAttackLanding)
            {
                KSNormalAttackLanding = false;
                StartCoroutine(KSBossAtack2());
            }
            base.OnCollisionEnter2D(col);
        }
    }
    protected override void OnCollisionStay2D(Collision2D col)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            if (col.gameObject.CompareTag("Stage") && KSNormalAttackLanding && enemyRb.velocity == Vector2.zero)
            {
                KSNormalAttackLanding = false;
                StartCoroutine(KSBossAtack2());
            }
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (GetComponent<KingSlime>().enabled)
    //    {
    //        if (collision.CompareTag("Player"))
    //        {
    //            //攻撃クールダウンタイム
    //            HadAttack = true;
    //            StartCoroutine(HadAttackReset());
    //            //ダメージとノックバック
    //            collision.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * enemyData.knockBackValue);
    //            collision.GetComponent<PlayerController>()._Damage(enemyData.attackPower * 4);
    //        }
    //    }
    //}



    //外部修正用変数をここに
    //壁に当たったら移動量を保ったまま回転を行うs
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
        enemyRb.velocity = new Vector2(enemyRb.velocity.x * -1, enemyRb.velocity.y);
        movingWidth *= -1;
        summonPosX *= -1;
    }

    public void PlayerInAttackArea(Collider2D col)
    {
        if (!HadAttack)
        {
            //攻撃クールダウンタイム
            HadAttack = true;
            StartCoroutine(HadAttackReset());
            //ダメージとノックバック
            col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * enemyData.knockBackValue);
            col.gameObject.GetComponent<PlayerController>()._Damage(enemyData.attackPower);
        }
    }
    public bool GetSkillTurnAround()
    {
        return SkillTurnAround;
    }
    public void SetSkillTurnAround(bool sta)
    {
        SkillTurnAround = sta;
    }


    //常に動くけど触らなくてもいいコードをここに
    //重力関連
    protected override void FixedUpdate()
    {
        if(!NoGravity)Gravity();
    }
    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }

    protected override void Destroy()
    {
        isDestroy = true;
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    public override void Damage(float power)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
        hp -= power;
        //HPゲージを使用しているかどうか
        if (HPBar != null)
        {
            HPBar.ReductionHP();
        }
        else
        {
            Debug.Log("HPBarはまだ入れてないです。もしHPBar付きで試したい場合はHPBarを付けてから試してください。");
        }

        ComboParam.Instance.ResetTime();
        if (!hadDamaged)
        {
            StartCoroutine(HadDamaged());
            hadDamaged = true;
        }
        if (hp <= 0)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            Destroy();
        }
    }

    void ClearCoroutines()
    {
        if (inKSBossAtack1)
        {
            inKSBossAtack1 = false;
            NoGravity = false;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            knockbackAttackCircle.enabled = false;
            KSNormalAttackLanding = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 0;
            wallCheck.SetActive(true);
        }
        if (inKSBossAtack2)
        {
            inKSBossAtack2 = false;
            NormalAttackAnimation = 0;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            knockbackAttackCircle.enabled = false;
            KSNormalAttackLanding = false;
            attackCheckArea.enabled = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 0;
            wallCheck.SetActive(true);
        }
        if (inKSBossAtack3)
        {
            inKSBossAtack3 = false;
            NormalAttackAnimation = 0;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 0;
            wallCheck.SetActive(true);
            SkillTurnAround = false;
        }
        if (inKSBossSummon)
        {
            inKSBossSummon = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 0;
        }
        if (inKSMovingAnim)
        {
            movingCheck = 0;
            KSmovingCheck = true;
        }
    }
}
