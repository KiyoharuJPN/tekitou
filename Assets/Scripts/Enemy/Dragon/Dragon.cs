using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KingSlime;
using System.Threading;
using static Dragon;

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


    //ボス撃破されたときの揺れ
    [System.Serializable]
    public struct BossDownShake
    {
        [Tooltip("揺れの時間")]
        public float ShakeTime;
        [Tooltip("揺れの強さ")]
        public float ShakePower;
        [Tooltip("アニメーション長さ")]
        public float StopingTime;
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
    public BossDownShake bossDownShake = new BossDownShake() { ShakeTime = 0.8f, ShakePower = 0.4f, StopingTime = 3f, StopTime = 1.5f, RecoverySpeed = 0.01f/*ShakeNum = 40, ShakeRand = 90*/ };

    //倒されたときの白い幕
    [System.Serializable]
    public struct WhiteCanvas
    {
        public Image bossDownImage;
        public float defaultAlpha, duration, stoptime;
    }
    [SerializeField]
    [Header("ボスが倒された時の白幕")]
    public WhiteCanvas whiteCanvas = new WhiteCanvas() { defaultAlpha = 0.7f, duration = 0.3f, stoptime = 1.35f };
    public float bossDownToResult = 1.6f;

    //爆発演出
    [System.Serializable]
    public struct ExplosionEffect
    {
        public GameObject explosionObj;
        [Tooltip("SlowMotionの進行速度（0=Pause 1=進行）、爆発間隔")]
        public float slowMotion, explosionInterval;
        [Tooltip("繰り返す回数")]
        public int repeat;
        [Tooltip("爆発アニメーションの位置(BOSSデフォルト位置からの視点)")]
        public Vector2[] explosionPosition;
    }
    [SerializeField, Header("爆発演出関連")]
    public ExplosionEffect explodeEffect = new ExplosionEffect() { slowMotion = 0.7f, explosionInterval = 0.15f, repeat = 5 };
    CancellationTokenSource cts;

    //消滅演出
    [System.Serializable]
    public struct BossDisappearParam
    {
        public GameObject bossDisappearObj;
        public float explosionScale;
        [Tooltip("爆発演出からの間隔")]
        public float explosionToDisappear;
        public bool camaraShake;
        [Tooltip("揺れ時間")]
        public float Duration;
        [Tooltip("揺れの強さ")]
        public float Strength;
    }
    [SerializeField, Header("消滅演出関連")]
    public BossDisappearParam bossDisappearParam = new BossDisappearParam() { explosionScale = 3f, explosionToDisappear = 3.3f, camaraShake = true, Duration = 0.8f, Strength = 1.1f };


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
        if (whiteCanvas.bossDownImage == null)
        {
            if (GameObject.Find("BossDownImage") != null)
                whiteCanvas.bossDownImage = GameObject.Find("BossDownImage").GetComponent<Image>();
            else
                Debug.Log("EnemyUICanvasをシーンに追加してください");
        }

        //使用方法
        //shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage") && JumpAttackAnimCtrl == 1 && enemyRb.velocity.y > -0.1)
        {
            JumpAttackAnimCtrl = 2;
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
        if(enemyRb.velocity.y < 0.3f && JumpAttackAnimCtrl == 0)
        {
            JumpAttackAnimCtrl = 1;
            animator.SetInteger("JumpAttackAnimCtrl", JumpAttackAnimCtrl);
            enemyRb.AddForce(new Vector2(1, -_dragonJumpingAttackData.DragonJAHeight+0.3f), ForceMode2D.Impulse);
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
        JumpAttackAnimCtrl = 2;
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
        JumpAttackAnimCtrl = 0;
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
    //Boss死亡時に呼ぶ関数
    virtual public void Boss_Down()
    {
        //Invoke("SetResult", bossDownToResult);
    }
    void SetResult()
    {
        ComboParam.Instance.ComboStop();
        GameManager.Instance.PlayerExAttack_Start();
        GameManager.Instance.Result_Start(1);
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
#if UNITY_EDITOR
#else
        //スチームChallenge
        Accmplisment.Instance.AchvOpen("Stage2");
#endif
        GameManager.Instance.StopRecordTime();
        //必殺技ヒットエフェクト消す
        BossCheckOnCamera = false;
        OnCamera = false;

        isDestroy = true;
        GameManager.Instance.AddKillEnemy();
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
        GameManager.Instance.PlayerStop();

        ////BossDown画面揺れ
        //shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        Time.timeScale = 0;
        await BossDownProcess();
    }
    public async UniTask BossDownWhiteCanvas()
    {
        whiteCanvas.bossDownImage.color = new Color(1, 1, 1, whiteCanvas.defaultAlpha);
        whiteCanvas.bossDownImage.enabled = true;
        //コンボを消す
        GameObject.Find("ComboCanvas").SetActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(whiteCanvas.stoptime), ignoreTimeScale: true);
        int i = (int)(whiteCanvas.duration * 100);
        float unitSpeed = whiteCanvas.defaultAlpha / i;
        while (i > 0)
        {
            float alpha = whiteCanvas.bossDownImage.color.a;
            alpha -= unitSpeed;
            whiteCanvas.bossDownImage.color = new Color(1, 1, 1, alpha);
            i--;

            await UniTask.Delay(TimeSpan.FromSeconds(0.01), ignoreTimeScale: true);
        }

        whiteCanvas.bossDownImage.color = new Color(1, 1, 1, 0);
        whiteCanvas.bossDownImage.enabled = false;
    }
    public async UniTask ExplosionEffectProcess()
    {
        for (int i = 0; i < explodeEffect.repeat; i++)
        {
            for (int j = 0; j < explodeEffect.explosionPosition.Length; j++)
            {
                var pos = explodeEffect.explosionPosition[j] + (Vector2)gameObject.transform.position;
                Instantiate(explodeEffect.explosionObj, pos, Quaternion.identity);
                var _ = BossDownBlink(cts.Token);
                await UniTask.Delay(TimeSpan.FromSeconds(explodeEffect.explosionInterval));
                ResetBossBlinkToken();

            }
        }
        var __ = BossDownBlink(cts.Token);
    }
    public async UniTask BossDownAnim()
    {
        var scale = new Vector3(bossDisappearParam.explosionScale, bossDisappearParam.explosionScale, bossDisappearParam.explosionScale);
        Time.timeScale = 1f;
        await UniTask.Delay(TimeSpan.FromSeconds(bossDisappearParam.explosionToDisappear));
        if (bossDisappearParam.camaraShake)
            shake.Shake(bossDisappearParam.Duration, bossDisappearParam.Strength, true, true);
        Instantiate(bossDisappearParam.bossDisappearObj, gameObject.transform.position, Quaternion.identity).transform.localScale = scale;
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        gameObject.SetActive(false);
        ResetBossBlinkToken();
    }
    public virtual async UniTask BossDownProcess()
    {
        //Debug.Log(Time.unscaledDeltaTime + "and" + Time.realtimeSinceStartup);
        //BossDown効果音
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);

        //GameManager.Instance.PlayerExAttack_Start();
        Time.timeScale = 0;
        var _ = BossDownWhiteCanvas();
        //BossDown画面揺れ
        shake.BossShake(bossDownShake.ShakeTime, bossDownShake.ShakePower, true, true);
        await UniTask.Delay(TimeSpan.FromSeconds(bossDownShake.StopTime), ignoreTimeScale: true);
        //爆発演出
        cts = new CancellationTokenSource();
        var __ = ExplosionEffectProcess();

        //時間を徐々に戻す
        int i = (int)((bossDownShake.StopingTime - bossDownShake.StopTime) / bossDownShake.RecoverySpeed);
        float unitSpeed = explodeEffect.slowMotion / i;
        while (i > 0)
        {
            Time.timeScale += unitSpeed;
            i--;

            await UniTask.Delay(TimeSpan.FromSeconds(bossDownShake.RecoverySpeed), ignoreTimeScale: true);
        }
        if (Time.timeScale != explodeEffect.slowMotion) Time.timeScale = explodeEffect.slowMotion;


        await BossDownAnim();

        await UniTask.Delay(TimeSpan.FromSeconds(bossDownToResult));
        SetResult();
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
    void PlayDragonFrameSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.DragonBlaze);
    }
    void PlayDragonLandSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
    }

    //unitask用
    void ResetBossBlinkToken()
    {
        cts.Cancel();
        cts.Dispose();
        cts = new CancellationTokenSource();
    }
}
