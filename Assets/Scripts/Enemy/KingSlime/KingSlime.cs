using System.Collections;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class KingSlime : Enemy
{
    [Header("移動する時の高さと距離")]
    public float moveHeightForce, moveWidthForce, AttackHeight = 8;
    public GameObject[] summonSlime;
    public GameObject wallCheck;
    
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

    [System.Serializable]
    public struct BossDownShake
    {
        [Tooltip("揺れの時間")]
        public float ShakeTime;
        [Tooltip("揺れの強さ")]
        public float ShakePower;
        [Tooltip("ストップ長さ（必ず揺れ時間より小さいように設定してください）")]
        public float StopTime;
        [Tooltip("回復速度（0.5より小さいように設定してください）")]
        public float RecoverySpeed;
        //[Tooltip("振動数")]
        //public int ShakeNum;
        //[Tooltip("揺れのランダム性")]
        //public int ShakeRand;
    }
    [SerializeField]
    [Header("ボスが倒されたときの揺れ")]
    public BossDownShake bossDownShake = new BossDownShake() { ShakeTime = 1f, ShakePower = 1.2f, StopTime = 0.3f, RecoverySpeed = 0.01f/*ShakeNum = 40, ShakeRand = 90*/ };

    float movingHeight, movingWidth, summonPosX, summonPosY;            //移動に関する内部関数
    bool KSmovingCheck = true, KSattackingCheck = true, KSNormalAttackLanding = false
        , NoGravity = false, ExSkillCheck = false, inExSkillCheck = false
        , ExSkillFalling = false, SkillTurnAround = false;                                            //判断用内部関数
    int movingCheck = 0, AttackMode = 1, NormalAttackAnimation;         //チェック用int関数
    GameObject playerObj;                                               //プレイヤーオブジェクト宣言

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
                return;
            }
            else
            {
                StartCoroutine(SkillWait());
            }
            
        }
        if (!IsBlowing)
        {
            //攻撃関連
            if (IsAttacking) KingSlimeAttack();
            //移動関連
            if (IsMoving && !inExSkillCheck)KingSlimeMoving();
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
        yield return new WaitForSeconds(1f);
        //attackCheckArea.offset = new Vector2(0, 0f);
        //attackCheckArea.size = new Vector2(5.36f, 4.1f);
        //attackCheckArea.enabled = true;

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
        gameObject.layer = LayerMask.NameToLayer("NoColliderEnemy");
        NormalAttackAnimation++;
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
        while (i < 35)
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

        var Summonslm = GetSummonProbability();
        var newSlime1 = Instantiate(summonSlime[Summonslm], summonPos, Quaternion.identity);
        if (transform.localScale.x < 0) newSlime1.GetComponent<Slime>().SummonSlimeTurn();
        newSlime1.GetComponent<Slime>().SetIsMoving(false);
        newSlime1.GetComponent<Rigidbody2D>().AddForce(new Vector2(summonPosX, summonPosY),ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);

        Summonslm = GetSummonProbability();
        var newSlime2 = Instantiate(summonSlime[Summonslm], summonPos, Quaternion.identity);
        if (transform.localScale.x < 0) newSlime2.GetComponent<Slime>().SummonSlimeTurn();
        newSlime2.GetComponent<Slime>().SetIsMoving(false);
        newSlime2.GetComponent<Rigidbody2D>().AddForce(new Vector2(summonPosX + 1, summonPosY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);

        Summonslm = GetSummonProbability();
        var newSlime3 = Instantiate(summonSlime[Summonslm], summonPos, Quaternion.identity);
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

    //Boss死亡時に呼ぶ関数
    virtual public void Boss_Down()
    {
        ComboParam.Instance.ComboStop();
        GameManager.Instance.PlayerExAttack_Start();
        GameManager.Instance.Result_Start(1);
    }

    //コライダーやトリガーなどのチェック関数
    protected override void OnColEnter2D(Collider2D col)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            //if (col.gameObject.CompareTag("Stage") && KSNormalAttackLanding)
            //{
            //    KSNormalAttackLanding = false;
            //    StartCoroutine(KSBossAtack2());
            //}
            base.OnColEnter2D(col);
        }
    }
    new private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            if (collision.gameObject.CompareTag("Stage") && KSNormalAttackLanding)
            {
                KSNormalAttackLanding = false;
                StartCoroutine(KSBossAtack2());
            }
        }
    }

    protected override void OnColStay2D(Collider2D col)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            //if (col.gameObject.CompareTag("Stage") && KSNormalAttackLanding && enemyRb.velocity == Vector2.zero)
            //{
            //    KSNormalAttackLanding = false;
            //    StartCoroutine(KSBossAtack2());
            //}
            base.OnColStay2D(col);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            if (collision.gameObject.CompareTag("Stage") && KSNormalAttackLanding && enemyRb.velocity == Vector2.zero)
            {
                KSNormalAttackLanding = false;
                StartCoroutine(KSBossAtack2());
            }
        }
    }


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
            col.gameObject.GetComponent<PlayerController>().Damage(enemyData.attackPower);
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
    //停止処理解除
    public override void Stop_End()
    {
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
    }
    //重力関連
    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        if(!NoGravity)Gravity();
    }
    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }

    //ダメージ関連
    protected override async void OnDestroyMode()
    {
        //必殺技ヒットエフェクト消す
        BossCheckOnCamera = false;
        OnCamera = false;


        //OnDestroy時の流れ
        isDestroy = true;
        IsBlowing = true;
        GameManager.Instance.PlayerStop();
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        //BossDown効果音
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);

        //GameManager.Instance.PlayerExAttack_Start();
        Time.timeScale = 0;
        await BossDownProcess();
    }
    public async UniTask BossDownProcess()
    {
        //BossDown画面揺れ
        shake.BossShake(bossDownShake.ShakeTime, bossDownShake.ShakePower, true, true);
        await UniTask.Delay(TimeSpan.FromSeconds(bossDownShake.StopTime), ignoreTimeScale: true);
        int i = (int)((bossDownShake.ShakeTime - bossDownShake.StopTime) / bossDownShake.RecoverySpeed);
        Debug.Log(i);
        while (i > 0)
        {
            Time.timeScale += 1/i;
            i--;
            await UniTask.Delay(TimeSpan.FromSeconds(bossDownShake.RecoverySpeed), ignoreTimeScale: true);
        }
        if (Time.timeScale != 1) Time.timeScale = 1;
        //GameManager.Instance.PlayerExAttack_End();


        //    Vector3 initialPos = transform.position;//初期位置保存
        //    Time.timeScale = 0;

        //    //BossDown効果音
        //    SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
        //    //ヒットストップ処理開始
        //    await transform.DOShakePosition(bossDownShake.ShakeTime, bossDownShake.ShakePower, bossDownShake.ShakeNum, bossDownShake.ShakeRand)
        //        .SetUpdate(true)
        //        .SetLink(gameObject)
        //        .OnComplete(() =>
        //        {
        //            //初期の位置に戻す
        //            transform.position = initialPos;
        //        });
        //    //アニメーションが終了したら時間を戻す
        //    if (!GameManager.Instance.PauseCheck)
        //    {
        //        Time.timeScale = 1;
        //    }

        //    //OnDestroy時の流れ
        //    isDestroy = true;
        //    IsBlowing = true;
        //    GameManager.Instance.PlayerStop();
        //    gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        //    gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //    gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DeadBoss")) return;
        //ダメージ処理
        DamegeProcess(power, skill, isHitStop, exSkill);
    }

    protected override async void DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
    {
        //ヒット時SE・コンボ時間リセット
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
        ComboParam.Instance.ResetTime();

        //ヒットエフェクト生成
        if (skill != null)
        {
            HitEfect(this.transform, skill.hitEffectAngle);
        }
        else HitEfect(this.transform, UnityEngine.Random.Range(0, 360));

        //ヒットストップ処理
        if (isHitStop)
        {
            await EnemyGeneratar.instance.HitStopProcess(power, this.transform);
        }
        //ヒット時演出（敵点滅）
        if (!hadDamaged)
        {
            StartCoroutine(HadDamaged());
            hadDamaged = true;
        }
        hp -= power;

        //HPゲージを使用しているかどうか
        if (HPBar != null)
        {
            HPBar.ReductionHP();
        }

        if (hp <= 0 && !isDestroy)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            OnDestroyMode();
        }
    }

    //修正しない関数
    void ClearCoroutines()
    {
        if (inKSBossAtack1)
        {
            inKSBossAtack1 = false;
            NoGravity = false;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            knockbackAttackCircle.enabled = false;
            attackCheckArea.enabled = false;
            KSNormalAttackLanding = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 1;
            wallCheck.SetActive(true);
        }
        if(NormalAttackAnimation == 1)
        {
            inKSBossAtack1 = false;
            NoGravity = false;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            knockbackAttackCircle.enabled = false;
            attackCheckArea.enabled = false;
            KSNormalAttackLanding = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 1;
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
            AttackMode = 1;
            wallCheck.SetActive(true);
        }
        if (inKSBossAtack3)
        {
            inKSBossAtack3 = false;
            NormalAttackAnimation = 0;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            knockbackAttackCircle.enabled = false;
            attackCheckArea.enabled = false;
            AttackMode = 1;
            wallCheck.SetActive(true);
            SkillTurnAround = false;
        }
        if (inKSBossSummon)
        {
            inKSBossSummon = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 1;
        }
        if (inKSMovingAnim)
        {
            movingCheck = 0;
            KSmovingCheck = true;
        }
        DefaultColor();
    }

    int GetSummonProbability()
    {
        var probability = (int)UnityEngine.Random.Range(0, 99) % 9;

        switch (probability)
        {
            case 0:
            case 1:
            case 2:
            default:
                return 0;
            case 3:
            case 4:
            case 5:
                return 1;
            case 6:
            case 7:
            case 8:
                return 2;
            case 9:
                return 3;
        }
    }
}
