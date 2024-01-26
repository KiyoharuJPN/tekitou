using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Enemy
{
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

    //ジャンプ関連
    [System.Serializable]
    public struct DragonJumpingAttackData
    {
        [Tooltip("ジャンプアタック時ジャンプの高さ")]
        public float DragonJAHeight;
        [Tooltip("ジャンプアタック時一番左のポジション")]
        public Vector2 DragonJALeftPos;
        [Tooltip("ジャンプアタック時一番右のポジション")]
        public Vector2 DragonJARightPos;
        [Tooltip("石生成高さ")]
        public float StoneHeight;
        [Tooltip("石生成の左x")]
        public float StoneMaxLeftPos;
        [Tooltip("石生成の右x")]
        public float StoneMaxRightPos;
        [Tooltip("生成個数")]
        public float StoneQuantity;
        [Tooltip("石の落下速度")]
        public float FallSpeed;
    }
    [SerializeField, Header("ドラゴンジャンプ攻撃に関する")]
    public DragonJumpingAttackData _dragonJumpingAttackData;
    public GameObject JumpAttackStone;
    GameObject[] DragonFallStone;
    float[] subDistanceRdm;


    // パタンシステム関連
    enum EnemyPatternSettings
    {
        IdleAnim,
        MoveAnim,
        SlewAroundAnim,
        FlameBraceAnim,
        JumpAttackAnim,
        SlewAttackAnim,
    }
    public float MoveSpeed = 10;
    public BoxCollider2D dragonAttackCheckArea;
    [Header("パタン調整（パタンの動きは敵の仕様書を参照してください）")]
    [SerializeField] List<EnemyPatternSettings> Pattern1,Pattern2,Pattern3;
    public BossHPBar HPBar;

    //内部関数
    //攻撃パタンを記録する関数
    int EnemyAnim = -1, EnemyPattern = -1, EnemyPatternPreb = -1, AnimationController = -1, JumpAttackAnimCtrl = -1;

    //アニメチェック、パターンチェック
    bool NotInAnim = true, PatternOver = true, patternover = false, isFlameBracing = false, isSlewAttacking = false, isJumpingAttacking = false;

    BoxCollider2D EnemyCollider;
    private void Awake()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        moveSpeed = MoveSpeed * -1;
        EnemyCollider = EnemyColliderArea.GetComponent<BoxCollider2D>();

        //もし石の存在場所が書かれていなかったらドラゴンの動ける場所が目標になる
        if (_dragonJumpingAttackData.StoneMaxLeftPos == 0) _dragonJumpingAttackData.StoneMaxLeftPos = _dragonJumpingAttackData.DragonJALeftPos.x;
        if (_dragonJumpingAttackData.StoneMaxRightPos == 0) _dragonJumpingAttackData.StoneMaxRightPos = _dragonJumpingAttackData.DragonJARightPos.x;
        //必要だけの場所をとる
        DragonFallStone = new GameObject[(int)_dragonJumpingAttackData.StoneQuantity];
        subDistanceRdm = new float[(int)_dragonJumpingAttackData.StoneQuantity];

        //カメラ揺れ
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        //使用方法
        //shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage") && JumpAttackAnimCtrl == 0 && enemyRb.velocity.y > -0.1)
        {
            JumpAttackAnimCtrl = 1;
            StartCoroutine(JumpAttackAnimPlus());
        }
    }

    //ドラゴンの動き
    protected override void Update()
    {
        base.Update();

        //animatorの設定
        animator.SetBool("IsBlowing", isDestroy);
        if (isDestroy) return;

        //敵のパターンをランダムで選択
        if (PatternOver)
        {
            //ランダムで敵のパターンを選ぶ
            while(EnemyPattern == EnemyPatternPreb)
            {
                EnemyPattern = UnityEngine.Random.Range(0, 999) % 3;
            }
            //次回同じものを選ばれないように先に代入をする
            EnemyPatternPreb = EnemyPattern;
            //選んだら二階選べないようにする
            PatternOver = false;        //パターンの最後にリセットする
            EnemyAnim = 0;              //Patternのゼロからアニメを流していく
        }

        //敵のパターンを沿って動きを実行
        if (!PatternOver)
        {
            switch (EnemyPattern)
            {
                case 0:
                    //アニメが流れていなければ次のアニメを流れる(コルーチンでtrue修正)
                    if(EnemyAnim < Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        //アニメを流れる
                        StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //次のアニメ
                        EnemyAnim++;
                    }
                    if(EnemyAnim == Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim=false;
                        StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //パターンをリセット（コルーチンの最後で実行）
                        patternover = true;
                    }
                    break;
                case 1:
                    if (EnemyAnim < Pattern2.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        StartCoroutine(Pattern2[EnemyAnim].ToString());
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern2.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        StartCoroutine(Pattern2[EnemyAnim].ToString());
                        patternover = true;
                    }
                    break;
                case 2:
                    if (EnemyAnim < Pattern3.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        StartCoroutine(Pattern3[EnemyAnim].ToString());
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern3.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        StartCoroutine(Pattern3[EnemyAnim].ToString());
                        patternover = true;
                    }
                    break;
                default:
                    Debug.Log("設定されていないパターンが読み込まれました。");
                    break;
            }
        }

        FixedAnim();
        
    }
    //アニメの修正はここで実行するようにしよう
    void FixedAnim()
    {
        if(enemyRb.velocity.y < 0.2f && JumpAttackAnimCtrl == 2)
        {
            JumpAttackAnimCtrl = 0;

            enemyRb.AddForce(new Vector2(1, -_dragonJumpingAttackData.DragonJAHeight), ForceMode2D.Impulse);
        }
    }

    //動き部分
    IEnumerator IdleAnim()
    {
        AnimationController = 0;        //animator調整（必須）
        animator.SetInteger("AnimationController", AnimationController);

        var animcheck = 0;
        while (animcheck < 60)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }

        //アニメの終わり（必須）
        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;                   //次の動きに移動できるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    IEnumerator MoveAnim()
    {
        AnimationController = 1;
        animator.SetInteger("AnimationController", AnimationController);

        var animcheck = 0;
        while (animcheck < 68)
        {
            animcheck++;
            transform.position = new Vector2(transform.position.x + (moveSpeed * Time.deltaTime * 0.1f), transform.position.y);
            yield return new WaitForSeconds(0.01f);
        }

        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;
        if (patternover)
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    IEnumerator SlewAroundAnim()
    {
        //プレイヤーはどこにいるかを判定する
        var Playerobj = GameObject.Find("Hero");
        var needTurn = false;
        if(moveSpeed < 0)
        {
            if (Playerobj.transform.position.x > gameObject.transform.position.x) needTurn = true;
        }
        if(moveSpeed > 0)
        {
            if (Playerobj.transform.position.x < gameObject.transform.position.x) needTurn = true;
        }

        if (needTurn)
        {
            AnimationController = 2;
            animator.SetInteger("AnimationController", AnimationController);

            yield return new WaitForSeconds(0.3f);
        }

        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;
        if (patternover)
        {
            PatternOver = patternover;
            patternover = false;
        }
    }


    //攻撃部分
    IEnumerator FlameBraceAnim()
    {
        AnimationController = 4;        //animator調整（必須）
        animator.SetInteger("AnimationController", AnimationController);

        //スキル関数部分
        isFlameBracing = true;

        float animcheck = 0;
        while (animcheck < 72)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        while (animcheck < 79)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        ResetAttackCheckArea();
        while (animcheck < 210)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }

        isFlameBracing = false;


        //アニメの終わり（必須）
        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;                   //次の動きに移動できるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    IEnumerator SlewAttackAnim()
    {
        AnimationController = 3;        //animator調整（必須）
        animator.SetInteger("AnimationController", AnimationController);

        isSlewAttacking = true;

        float animcheck = 0;
        while(animcheck < 90)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        while(animcheck < 96)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        while (animcheck < 104)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        while (animcheck < 112)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        ResetAttackCheckArea();
        ResetBoxCollider2D();
        while (animcheck < 150)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        isSlewAttacking = false;

        //アニメの終わり（必須）
        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;                   //次の動きに移動できるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    IEnumerator JumpAttackAnim()
    {
        AnimationController = 5;        //animator調整（必須）
        animator.SetInteger("AnimationController", AnimationController);
        isJumpingAttacking = true;

        yield return new WaitForEndOfFrame();
    }
    IEnumerator JumpAttackAnimPlus()
    {
        //地面に降りるアニメーション
        JumpAttackAnimCtrl = 1;
        animator.SetInteger("JumpAttackAnimCtrl", JumpAttackAnimCtrl);
        gameObject.layer = LayerMask.NameToLayer("BossEnemy");
        ResetAttackCheckArea();
        //落下後の画面揺れ
        shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);

        var animcheck = 0;
        while (animcheck < 50)
        {
            animcheck++;
            transform.position = new Vector2(transform.position.x + (moveSpeed * Time.deltaTime * 0.1f), transform.position.y);
            yield return new WaitForSeconds(0.01f);
        }
        CreateStoneAttack();

        while (animcheck < 75)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }

        isJumpingAttacking = false;
        //アニメの終わり（必須）
        animator.SetInteger("AnimationController", -1);
        JumpAttackAnimCtrl = -1;
        animator.SetInteger("JumpAttackAnimCtrl", JumpAttackAnimCtrl);
        NotInAnim = true;                   //次の動きに移動できるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    //外部関数
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DeadBoss")) return;
        //ヒットストップ
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

        if (hp <= 0&&!isDestroy)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            OnDestroyMode();
        }
    }

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
        moveSpeed *= -1;
    }

    // アニメータで呼ばれるジャンプアニメーションが流されるときにジャンプする関数
    public void BossJAJump()
    {
        dragonAttackCheckArea.offset = new Vector2(0.5f, -1.2f);
        dragonAttackCheckArea.size = new Vector2(12f, 3.9f);
        gameObject.layer = LayerMask.NameToLayer("NoColliderEnemy");
        dragonAttackCheckArea.gameObject.SetActive(true);

        //ドラゴンジャンプ
        var jumpWidth = 1.0f;
        if (transform.localScale.x > 0f)
        {
            jumpWidth = _dragonJumpingAttackData.DragonJALeftPos.x - transform.position.x;
        }else if (transform.localScale.x < 0f)
        {
            jumpWidth = _dragonJumpingAttackData.DragonJARightPos.x - transform.position.x;
        }
        enemyRb.AddForce(new Vector2(jumpWidth * 0.5f, _dragonJumpingAttackData.DragonJAHeight),ForceMode2D.Impulse);

        //接地する時に次のアニメーションを流せるようにif文の判断要素にする
        JumpAttackAnimCtrl = 2;
    }

    public void PlayerInAttackArea(Collider2D col)
    {
        if (!HadAttack)
        {
            if (isFlameBracing)
            {
                //攻撃クールダウンタイム
                HadAttack = true;
                StartCoroutine(HadAttackReset());
                //FlameBracingのダメージとノックバック
                col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 30);
                col.gameObject.GetComponent<PlayerController>().Damage(2);
            }

            if (isSlewAttacking)
            {
                //攻撃クールダウンタイム
                HadAttack = true;
                StartCoroutine(HadAttackReset());
                //SlewAttackingのダメージとノックバック
                col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 30);
                col.gameObject.GetComponent<PlayerController>().Damage(2);
            }

            if (isJumpingAttacking)
            {
                //攻撃クールダウンタイム
                HadAttack = true;
                StartCoroutine(HadAttackReset());
                //SlewAttackingのダメージとノックバック
                col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 30);
                col.gameObject.GetComponent<PlayerController>().Damage(2);
            }
        }
    }

    //ドラゴン死亡時に呼ぶ関数
    virtual public void Boss_Down()
    {
        ComboParam.Instance.ComboStop();
        GameManager.Instance.PlayerExAttack_Start();
        GameManager.Instance.Result_Start(2);
    }



    //内部関数
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
    protected override async void OnDestroyMode()
    {
        //必殺技ヒットエフェクト消す
        BossCheckOnCamera = false;
        OnCamera = false;

        isDestroy = true;
        GameManager.Instance.AddKillEnemy();
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);

        ////BossDown画面揺れ
        //shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        Time.timeScale = 0;
        await BossDownProcess();
    }
    public async UniTask BossDownProcess()
    {
        //BossDown画面揺れ
        shake.BossShake(1f, _shakeInfo.Strength, true, true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.3), ignoreTimeScale: true);
        int i = 70;
        while (i > 0)
        {
            Time.timeScale += 1 / i;
            i--;
            await UniTask.Delay(TimeSpan.FromSeconds(0.01), ignoreTimeScale: true);
        }
        if (Time.timeScale != 1) Time.timeScale = 1;
        //Time.timeScale = 0.3f;

        //await UniTask.Delay(320);
        //Time.timeScale = 1;
    }

    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10));
    }

    void ResetBoxCollider2D()
    {
        EnemyCollider.offset = new Vector2(0, -1.6f);
        EnemyCollider.size = new Vector2(9.6f, 3.18f);
    }

    //Dragon炎の息関連の内部関数
    void ResetAttackCheckArea()
    {
        dragonAttackCheckArea.offset = new Vector2(-6.64f, -2.2f);
        dragonAttackCheckArea.size = new Vector2(4.3f, 2f);
    }
    void AttackCheckArea1()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.1f, -1.5f);
        dragonAttackCheckArea.size = new Vector2(7.4f, 3.4f);
    }
    void AttackCheckArea2()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.3f, -1.3f);
        dragonAttackCheckArea.size = new Vector2(7.8f, 3.8f);
    }
    void AttackCheckArea3()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.5f, -1.1f);
        dragonAttackCheckArea.size = new Vector2(7.5f, 4.2f);
    }
    void AttackCheckArea4()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.4f, -1f);
        dragonAttackCheckArea.size = new Vector2(8f, 4.4f);
    }
    void AttackCheckArea5()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.2f, -1.3f);
        dragonAttackCheckArea.size = new Vector2(7.6f, 3.85f);
    }
    void AttackCheckAreaOver()
    {
        dragonAttackCheckArea.offset = new Vector2(-8f, -1.8f);
        dragonAttackCheckArea.size = new Vector2(6.65f, 2.85f);
    }

    //石生成用
    void CreateStoneAttack()
    {
        var Distance = _dragonJumpingAttackData.StoneMaxRightPos - _dragonJumpingAttackData.StoneMaxLeftPos;
        var subDistance = Distance / (_dragonJumpingAttackData.StoneQuantity);
        //Debug.Log(subDistance+"\n"+Distance);
        for(int i =0; i <= _dragonJumpingAttackData.StoneQuantity - 1; i++)
        {
            if(i == 0)
            {
                subDistanceRdm[i] = UnityEngine.Random.Range(0f, subDistance);
            }
            else
            {
                subDistanceRdm[i] = UnityEngine.Random.Range(2.5f, subDistance);
            }
        }
        subDistance = 0;
        for (int i = 1; i <= _dragonJumpingAttackData.StoneQuantity; i++)
        {
            subDistance += subDistanceRdm[i-1];
            if(subDistance > Distance) { subDistance = Distance; }
            //DragonFallStone[i - 1] = ObjectPool.Instance.GetObject(JumpAttackStone);
            DragonFallStone[i - 1] = Instantiate(JumpAttackStone);
            DragonFallStone[i - 1].transform.position = new Vector2(_dragonJumpingAttackData.StoneMaxLeftPos + subDistance, gameObject.transform.position.y + _dragonJumpingAttackData.StoneHeight);
            DragonFallStone[i - 1].GetComponent<DragonFallStone>().SetSpeed(_dragonJumpingAttackData.FallSpeed);
        }
    }

    void PlayDragonRoarSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.DragonRoar);
    }
}
