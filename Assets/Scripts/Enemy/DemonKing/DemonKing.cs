using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing : Enemy
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
    Animator LHanimator, RHanimator;

    //Idle時
    [System.Serializable]
    struct IdleStatus
    {
        [Tooltip("移動上限、下限")]
        public float upLimit, downLimit;
        [Tooltip("左手と右手の移動スピード")]
        public float handSpeed;
        [Tooltip("移動回数")]
        public int handFrequency;
    }
    [SerializeField] IdleStatus idleStatus;
    float LHSpeed, RHSpeed;
    int HandMoveFrequency;

    //CrushAttack時
    [System.Serializable]
    struct CrushAttckStatus
    {
        [Tooltip("下降速度と追跡時高さ(0でデフォルト)")]
        public float dropSpeed, attackHeight, furthermoreAttackHeight;
        [Tooltip("左の上限、右の上限")]
        public float leftCrushMaxPosX, rightCrushMaxPosX;
    }
    [SerializeField] CrushAttckStatus crushAttckStatus = new CrushAttckStatus { dropSpeed = 30, attackHeight = 8, furthermoreAttackHeight = 2 };

    //SommonAttack時
    [System.Serializable]
    struct SummonAttackStatus
    {
        [Tooltip("召喚する敵オブジェクト(0でデフォルト)")]
        public GameObject summonMonster;
        [Tooltip("攻撃時の上昇上限と速度と下降速度(0でデフォルト)")]
        public float summonHeightLimit, summonHandSpeed, dropSpeed;
        [Tooltip("生成範囲（左上の位置と右下の位置0でデフォルト）")]
        public Vector2 LeftUpPoint, RightDownPoint;
    }
    [SerializeField] SummonAttackStatus summonAttackStatus;

    //PincerAttack時
    [System.Serializable]
    struct PincerAttackStatus
    {
        [Tooltip("攻撃開始時下と横に下がるマス数")]
        public float downMass, BackMass;
        [Tooltip("追跡スピード")]
        public float maxFollowSpeed, bottomLimit;
        [Tooltip("プレイヤーへ攻撃時スピード")]
        public float attackSpeed;
    }
    [SerializeField] PincerAttackStatus pincerAttackStatus = new PincerAttackStatus { downMass = 0, BackMass = 3, maxFollowSpeed = 2, bottomLimit = 0, attackSpeed = 10};
    Vector2 pincerReservePosLH, pincerReservePosRH;


    Rigidbody2D enemyLHRb, enemyRHRb;
    Vector2 LHOriginalPos, RHOriginalPos;

    // パタンシステム関連
    enum EnemyPatternSettings
    {
        IdleAnim,
        //MoveAnim,
        CrushAttackAnim,
        SummonAttackAnim,
        PincerAttackAnim,
    }
    [Header("パタン調整（パタンの動きは敵の仕様書を参照してください）")]
    [SerializeField] List<EnemyPatternSettings> Pattern1, Pattern2, Pattern3;
    public BossHPBar HPBar;
    public GameObject LeftHand, RightHand;
    public RuntimeAnimatorController animControllerL, animControllerR;

    

    //内部関数
    //プレイヤーのオブジェクト
    GameObject Player;
    //攻撃パタンを記録する関数
    int EnemyAnim = -1, EnemyPattern = -1, EnemyPatternPreb = -1, AnimationController = -1;
    int BossLayer;

    //アニメチェック、パターンチェック
    bool NotInAnim = true, PatternOver = true, patternover = false, isSummonAttack = false, isCrushAttack = false, isPincerAttack = false;

    //必殺技用コルーチン
    Coroutine MovementCoroutine;


    private void OnEnable()
    {
        if(animator != null)
        {
            animator.SetBool("InAdanim", false);
            //LHanimator.SetBool("InAdanim", false);
            //RHanimator.SetBool("InAdanim", false);
            LHanimator.runtimeAnimatorController = animControllerL;
            RHanimator.runtimeAnimatorController = animControllerR;
        }
        if(HPBar != null && !HPBar.gameObject.activeSelf)
        {
            HPBar.gameObject.SetActive(true);
        }

    }

    protected override void Start()
    {
        base.Start();
        BossLayer = LayerMask.NameToLayer("BossEnemy");
        //Debug.Log(LayerMask.NameToLayer("BossEnemy"));

        //idle関係
        if (idleStatus.handSpeed == 0) idleStatus.handSpeed = 1;
        if (idleStatus.handFrequency == 0) idleStatus.handFrequency = 1;
        if (idleStatus.upLimit == 0) { idleStatus.upLimit = LeftHand.transform.position.y + 5; } else { idleStatus.upLimit += LeftHand.transform.position.y; }
        if (idleStatus.downLimit == 0) { idleStatus.downLimit = LeftHand.transform.position.y - 5; } else { idleStatus.downLimit = LeftHand.transform.position.y - idleStatus.downLimit; }
        LHSpeed = idleStatus.handSpeed * 0.01f;
        RHSpeed = LHSpeed * -1;
        HandMoveFrequency = idleStatus.handFrequency * 2;

        //crushAttack関係
        if (crushAttckStatus.leftCrushMaxPosX == 0) { crushAttckStatus.leftCrushMaxPosX = transform.position.x - 10; } else { crushAttckStatus.leftCrushMaxPosX = transform.position.x - crushAttckStatus.leftCrushMaxPosX; }
        if (crushAttckStatus.rightCrushMaxPosX == 0) { crushAttckStatus.rightCrushMaxPosX = transform.position.x + 10; } else { crushAttckStatus.leftCrushMaxPosX = transform.position.x + crushAttckStatus.rightCrushMaxPosX; }

        //sommonAttack関係
        if (summonAttackStatus.summonHandSpeed == 0) { summonAttackStatus.summonHandSpeed = 0.01f; } else { summonAttackStatus.summonHandSpeed *= 0.01f; }
        if (summonAttackStatus.summonHeightLimit == 0) { summonAttackStatus.summonHeightLimit = LeftHand.transform.position.y + 5; } else { summonAttackStatus.summonHeightLimit += LeftHand.transform.position.y; }
        if (summonAttackStatus.dropSpeed == 0) { summonAttackStatus.dropSpeed = 30; }
        if (summonAttackStatus.LeftUpPoint == Vector2.zero) { summonAttackStatus.LeftUpPoint = new Vector2(transform.position.x - 7, transform.position.y + 8); }
        if (summonAttackStatus.RightDownPoint == Vector2.zero) { summonAttackStatus.RightDownPoint = new Vector2(transform.position.x +7,transform.position.y+7); }

        //pinverAttack関係
        pincerReservePosLH = new Vector2(LeftHand.transform.position.x - pincerAttackStatus.BackMass, LeftHand.transform.position.y - pincerAttackStatus.downMass);
        pincerReservePosRH = new Vector2(RightHand.transform.position.x + pincerAttackStatus.BackMass, RightHand.transform.position.y - pincerAttackStatus.downMass);
        pincerAttackStatus.maxFollowSpeed *= 0.01f;

        //animator代入
        LHanimator = LeftHand.GetComponent<Animator>();
        RHanimator = RightHand.GetComponent<Animator>();
        //アニメーター代入
        LHanimator.runtimeAnimatorController = animControllerL;
        RHanimator.runtimeAnimatorController = animControllerR;
        //プレイヤーのオブジェクト
        Player = GameObject.Find("Hero");
        //両手のレジッドボディ
        enemyLHRb = LeftHand.GetComponent<Rigidbody2D>();
        enemyRHRb = RightHand.GetComponent<Rigidbody2D>();
        //両手の最初の位置を覚える
        LHOriginalPos = LeftHand.transform.position;
        RHOriginalPos = RightHand.transform.position;
        //カメラ揺れ
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        //使用方法
        //shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
    }

    //魔王の動き
    protected override void Update()
    {
        base.Update();

        if (isDestroy) return;

        //敵のパターンをランダムで選択
        if (PatternOver || NotInAnim)
        {
            //ランダムで敵のパターンを選ぶ
            while (EnemyPattern == EnemyPatternPreb)
            {
                EnemyPattern = Random.Range(0, 999) % 3;
            }

            //デバッグ用
            //EnemyPattern = 0;
            //Debug.Log(EnemyPattern);
            //if (!(EnemyPattern < 3 && EnemyPattern >= 0))
            //{
            //    Debug.Log("ドラゴンのランダム関数にエラーが出ました、計算式をチェックしてください。");
            //}

            //次回同じものを選ばれないように先に代入をする
            EnemyPatternPreb = EnemyPattern;
            //選んだら二階選べないようにする
            PatternOver = false;        //パターンの最後にリセットする
            EnemyAnim = 0;              //Patternのゼロからアニメを流していく
        }

        //敵のパターンを沿って動きを実行
        if (!PatternOver)
        {
            if (isPlayerExAttack) return;
            switch (EnemyPattern)
            {
                case 0:
                    //アニメが流れていなければ次のアニメを流れる(コルーチンでtrue修正)
                    if (EnemyAnim < Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        //アニメを流れる
                        MovementCoroutine = StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //次のアニメ
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //パターンをリセット（コルーチンの最後で実行）
                        patternover = true;
                    }
                    break;
                case 1:
                    if (EnemyAnim < Pattern2.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern2[EnemyAnim].ToString());
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern2.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern2[EnemyAnim].ToString());
                        patternover = true;
                    }
                    break;
                case 2:
                    if (EnemyAnim < Pattern3.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern3[EnemyAnim].ToString());
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern3.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern3[EnemyAnim].ToString());
                        patternover = true;
                    }
                    break;
                default:
                    Debug.Log("設定されていないパターンが読み込まれました。");
                    break;
            }
        }

    }








    //動き部分
    IEnumerator IdleAnim()
    {
        AnimationController = 0;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);


        LeftHand.transform.position = LHOriginalPos;
        RightHand.transform.position = RHOriginalPos;

        int Frequency = 0;
        while(Frequency < HandMoveFrequency)
        {
            LeftHand.transform.position = new Vector2(LeftHand.transform.position.x, LeftHand.transform.position.y + LHSpeed);
            RightHand.transform.position = new Vector2(RightHand.transform.position.x, RightHand.transform.position.y + RHSpeed);

            if (LeftHand.transform.position.y >= LHOriginalPos.y - idleStatus.handSpeed/2&& LeftHand.transform.position.y <= LHOriginalPos.y + idleStatus.handSpeed / 2) Frequency++;
            //Debug.Log(LeftHand.transform.position.y);
            //Debug.Log(LHOriginalPos.y);
            if (LHSpeed > 0 && LeftHand.transform.position.y > idleStatus.upLimit) LHSpeed *= -1;
            if (LHSpeed < 0 && LeftHand.transform.position.y < idleStatus.downLimit) LHSpeed *= -1;
            if (RHSpeed > 0 && RightHand.transform.position.y > idleStatus.upLimit) RHSpeed *= -1;
            if (RHSpeed < 0 && RightHand.transform.position.y < idleStatus.downLimit) RHSpeed *= -1;
            yield return new WaitForSeconds(0.01f);
        }

        LeftHand.transform.position = LHOriginalPos;
        RightHand.transform.position = RHOriginalPos;

        //アニメの終わり（必須）
        //パターンアニメーション関連
        AnimationController = -1;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //パターンループ関連
        NotInAnim = true;                   //次の動きに移せるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }

    IEnumerator CrushAttackAnim()
    {
        //Debug.Log(LayerMask.NameToLayer("BossEnemy"));
        Physics2D.IgnoreLayerCollision(BossLayer, BossLayer);
        AnimationController = 1;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);

        //一次移動用プレイヤーの位置と速度の算出
        Vector2 PlayerPos = Player.transform.position+new Vector3(0, crushAttckStatus.attackHeight, 0);
        var attackMoveSpeed = Vector2.Distance(PlayerPos, RightHand.transform.position);
        attackMoveSpeed /= 25;

        //一旦手をプレイヤーの上に移動する
        var i = 0;
        while(i < 25)
        {
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, PlayerPos, attackMoveSpeed);
            i++;
            yield return new WaitForEndOfFrame();
        }

        ////レイヤーを地形やプレイやの前に移動させる
        //RightHand.GetComponent<SpriteRenderer>().sortingOrder = 11;
        //手を3秒間プレイヤーの頭の上に残す
        i = 0;
        while (i < 180)
        {
            if(Player.transform.position.x < crushAttckStatus.leftCrushMaxPosX )
            {
                RightHand.transform.position = new Vector2(crushAttckStatus.leftCrushMaxPosX, Player.transform.position.y + crushAttckStatus.attackHeight);
            }
            else if(Player.transform.position.x > crushAttckStatus.rightCrushMaxPosX)
            {

                RightHand.transform.position = new Vector2(crushAttckStatus.rightCrushMaxPosX, Player.transform.position.y + crushAttckStatus.attackHeight);
            }
            else
            {
                RightHand.transform.position = new Vector2(Player.transform.position.x, Player.transform.position.y + crushAttckStatus.attackHeight);
            }
            i++;
            yield return new WaitForSeconds(0.01f);
        }

        //手の位置を上に指定のマス移動
        i = 0;
        attackMoveSpeed = crushAttckStatus.furthermoreAttackHeight / 25;
        while(i < 30)
        {
            if (i > 5)
            {
                RightHand.transform.position = new Vector2(RightHand.transform.position.x, RightHand.transform.position.y + attackMoveSpeed);
            }
            i++;
            yield return new WaitForSeconds(0.01f);
        }

        //下降攻撃
        isCrushAttack = true;
        enemyRHRb.AddForce(new Vector2(0, -crushAttckStatus.dropSpeed),ForceMode2D.Impulse);
    }
    IEnumerator CrushAttackAnim2()
    {
        Physics2D.IgnoreLayerCollision(BossLayer, BossLayer,false);
        yield return new WaitForSeconds(3);

        ////レイヤーの位置を戻す
        //RightHand.GetComponent<SpriteRenderer>().sortingOrder = 0;


        //手を戻す
        var attackMoveSpeed = Vector2.Distance(RHOriginalPos, RightHand.transform.position);
        attackMoveSpeed /= 25;
        var i = 0;
        while (i < 25)
        {
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, RHOriginalPos, attackMoveSpeed);
            i++;
            if (i == 5)
            {
                AnimationController = 0;        //animator調整（必須）
                LHanimator.SetInteger("AnimationController", AnimationController);
                RHanimator.SetInteger("AnimationController", AnimationController);
            }
            yield return new WaitForSeconds(0.01f);
        }

        RightHand.transform.position = RHOriginalPos;

        //アニメの終わり（必須）
        //パターンアニメーション関連
        AnimationController = -1;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //パターンループ関連
        NotInAnim = true;                   //次の動きに移せるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }

    IEnumerator SummonAttackAnim()
    {
        AnimationController = 2;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);

        //上昇上限まで上昇する
        while (LeftHand.transform.position.y < summonAttackStatus.summonHeightLimit)
        {
            LeftHand.transform.position = new Vector2(LeftHand.transform.position.x, LeftHand.transform.position.y + summonAttackStatus.summonHandSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        enemyLHRb.velocity = Vector3.zero;
        StartAnimatorLH();
        isSummonAttack = true;
        enemyLHRb.AddForce(new Vector2(0,-summonAttackStatus.dropSpeed),ForceMode2D.Impulse);


    }
    IEnumerator SummonAttackAnim2()
    {

        yield return new WaitForSeconds(1.5f);
        //一人目
        Vector2 summonPos = new Vector2(Random.Range(summonAttackStatus.LeftUpPoint.x,summonAttackStatus.RightDownPoint.x),Random.Range(summonAttackStatus.LeftUpPoint.y,summonAttackStatus.RightDownPoint.y));
        var newEnemy = Instantiate(summonAttackStatus.summonMonster, summonPos, Quaternion.identity);
        newEnemy.GetComponentInChildren<EnemyBuffSystem>().SetBuffTypeByScript(GetSummonProbability());
        //二人目
        summonPos = new Vector2(Random.Range(summonAttackStatus.LeftUpPoint.x, summonAttackStatus.RightDownPoint.x), Random.Range(summonAttackStatus.LeftUpPoint.y, summonAttackStatus.RightDownPoint.y));
        newEnemy = Instantiate(summonAttackStatus.summonMonster, summonPos, Quaternion.identity);
        newEnemy.GetComponentInChildren<EnemyBuffSystem>().SetBuffTypeByScript(GetSummonProbability());
        //三人目
        summonPos = new Vector2(Random.Range(summonAttackStatus.LeftUpPoint.x, summonAttackStatus.RightDownPoint.x), Random.Range(summonAttackStatus.LeftUpPoint.y, summonAttackStatus.RightDownPoint.y));
        newEnemy = Instantiate(summonAttackStatus.summonMonster, summonPos, Quaternion.identity);
        newEnemy.GetComponentInChildren<EnemyBuffSystem>().SetBuffTypeByScript(GetSummonProbability());

        yield return new WaitForSeconds(3);

        //手を戻す
        var backMoveSpeed = Vector2.Distance(LHOriginalPos, LeftHand.transform.position);
        backMoveSpeed /= 25;
        var i = 0;
        while (i < 25)
        {
            LeftHand.transform.position = Vector2.MoveTowards(LeftHand.transform.position, LHOriginalPos, backMoveSpeed);
            i++;
            if (i == 5)
            {
                AnimationController = 0;        //animator調整（必須）
                LHanimator.SetInteger("AnimationController", AnimationController);
                RHanimator.SetInteger("AnimationController", AnimationController);
            }
            yield return new WaitForSeconds(0.01f);
        }

        LeftHand.transform.position = LHOriginalPos;

        //アニメの終わり（必須）
        //パターンアニメーション関連
        AnimationController = -1;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //パターンループ関連
        NotInAnim = true;                   //次の動きに移せるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }

    IEnumerator PincerAttackAnim()
    {
        AnimationController = 3;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);


        //手を横の下方向へ移動
        var attackMoveSpeed = Vector2.Distance(LeftHand.transform.position, pincerReservePosLH);
        attackMoveSpeed /= 25;
        var i = 0;
        while (i < 25)
        {
            LeftHand.transform.position = Vector2.MoveTowards(LeftHand.transform.position, pincerReservePosLH, attackMoveSpeed);
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, pincerReservePosRH, attackMoveSpeed);
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        LeftHand.transform.position = pincerReservePosLH;
        RightHand.transform.position = pincerReservePosRH;

        float timer = 0;
        while (timer < 2)
        {
            timer += Time.deltaTime;
            float PlayerPosY = Player.transform.position.y;
            if(Mathf.Abs(LeftHand.transform.position.y - PlayerPosY) > pincerAttackStatus.maxFollowSpeed)
            {
                if(LeftHand.transform.position.y - PlayerPosY > 0)
                {
                    PlayerPosY = LeftHand.transform.position.y - pincerAttackStatus.maxFollowSpeed;
                }
                else
                {
                    PlayerPosY = LeftHand.transform.position.y +pincerAttackStatus.maxFollowSpeed;
                }
            }
            if(PlayerPosY > pincerAttackStatus.bottomLimit)
            {
                LeftHand.transform.position = new Vector2(LeftHand.transform.position.x, PlayerPosY);
                RightHand.transform.position = new Vector2(RightHand.transform.position.x, PlayerPosY);
            }
            else
            {
                LeftHand.transform.position = new Vector2(LeftHand.transform.position.x, pincerAttackStatus.bottomLimit);
                RightHand.transform.position = new Vector2(RightHand.transform.position.x, pincerAttackStatus.bottomLimit);
            }
            yield return new WaitForEndOfFrame();
        }

        isPincerAttack = true;
        enemyLHRb.velocity = new Vector2 (pincerAttackStatus.attackSpeed, 0);
        enemyRHRb.velocity = new Vector2(-pincerAttackStatus.attackSpeed, 0);



        
    }
    IEnumerator PincerAttackAnim2()
    {
        yield return new WaitForSeconds(3);

        //手を戻す
        var backMoveSpeedLH = Vector2.Distance(LHOriginalPos, LeftHand.transform.position);
        var backMoveSpeedRH = Vector2.Distance(RHOriginalPos, RightHand.transform.position);
        backMoveSpeedLH /= 25;
        backMoveSpeedRH /= 25;
        var i = 0;
        while (i < 25)
        {
            LeftHand.transform.position = Vector2.MoveTowards(LeftHand.transform.position, LHOriginalPos, backMoveSpeedLH);
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, RHOriginalPos, backMoveSpeedRH);
            i++;
            if (i == 5)
            {
                AnimationController = 0;        //animator調整（必須）
                LHanimator.SetInteger("AnimationController", AnimationController);
                RHanimator.SetInteger("AnimationController", AnimationController);
            }
            yield return new WaitForSeconds(0.01f);
        }


        LeftHand.transform.position = LHOriginalPos;
        RightHand.transform.position = RHOriginalPos;


        //アニメの終わり（必須）
        //パターンアニメーション関連
        AnimationController = -1;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //パターンループ関連
        NotInAnim = true;                   //次の動きに移せるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }

    IEnumerator ExBackIdleAnim()
    {
        AnimationController = 0;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);


        yield return new WaitForSeconds(1f);

        //手の位置と戻るための速度を計算する
        var backMoveSpeedLH = Vector2.Distance(LHOriginalPos, LeftHand.transform.position);
        var backMoveSpeedRH = Vector2.Distance(RHOriginalPos, RightHand.transform.position);
        backMoveSpeedLH /= 25;
        backMoveSpeedRH /= 25;

        //手をデフォルト位置に戻す
        var i = 0;
        while (i < 25)
        {
            LeftHand.transform.position = Vector2.MoveTowards(LeftHand.transform.position, LHOriginalPos, backMoveSpeedLH);
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, RHOriginalPos, backMoveSpeedRH);
            i++;
            if (i == 5)
            {
                AnimationController = 0;        //animator調整（必須）
                LHanimator.SetInteger("AnimationController", AnimationController);
                RHanimator.SetInteger("AnimationController", AnimationController);
            }
            yield return new WaitForSeconds(0.01f);
        }


        LeftHand.transform.position = LHOriginalPos;
        RightHand.transform.position = RHOriginalPos;

        Physics2D.IgnoreLayerCollision(BossLayer, BossLayer, false);
        yield return new WaitForSeconds(1f);

        //アニメの終わり（必須）
        //パターンアニメーション関連
        AnimationController = -1;        //animator調整（必須）
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //パターンループ関連
        NotInAnim = true;                   //次の動きに移せるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }

    //内部関数
    protected override void FixedUpdate()
    {
        if (isCrushAttack)  enemyRHRb.AddForce(new Vector2(0, -5));
        if (isSummonAttack) enemyLHRb.AddForce(new Vector2(0, -5));
    }








    //攻撃関数
    public void PlayerInAttackArea(Collider2D collider,bool isLH)
    {
        if (!HadAttack)
        {
            if (isCrushAttack && !isLH)
            {
                //攻撃クールダウンタイム
                HadAttack = true;
                StartCoroutine(HadAttackReset());

                //CrushAttackのダメージとノックバック
                collider.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 50);
                collider.gameObject.GetComponent<PlayerController>().Damage(2);
            }

            if(isSummonAttack && isLH)
            {
                //攻撃クールダウンタイム
                HadAttack = true;
                StartCoroutine(HadAttackReset());

                //isSummonAttackのダメージとノックバック
                collider.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 40);
                collider.gameObject.GetComponent<PlayerController>().Damage(1);
            }

            if(isPincerAttack)
            {
                //攻撃クールダウンタイム
                HadAttack = true;
                StartCoroutine(HadAttackReset());

                //isPincerAttackのダメージとノックバック
                collider.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 50);
                collider.gameObject.GetComponent<PlayerController>().Damage(2);
            }
        }
    }








    //外部関数
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DeadBoss")) return;
        //ヒットストップ
        StartCoroutine(DamegeProcess(power, skill, isHitStop, exSkill));
    }

    protected override IEnumerator DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
    {
        //ヒット時SE・コンボ時間リセット
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
        ComboParam.Instance.ResetTime();

        ////ヒットエフェクト生成
        //if (skill != null)
        //{
        //    HitEfect(this.transform, skill.hitEffectAngle);
        //}
        //else HitEfect(this.transform, UnityEngine.Random.Range(0, 360));

        //ヒットストップ処理
        if (isHitStop)
        {
            Vector3 initialPos = this.transform.position;//初期位置保存
            Time.timeScale = 0;

            var stopTime = power * stopState.shakTime;
            if (stopTime > stopState.shakTimeMax)
            {
                stopTime = stopState.shakTimeMax;
            }

            //ヒットストップ処理開始
            tween = transform.DOShakePosition(stopTime, stopState.shakPowar, stopState.shakNum, stopState.shakRand)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    //アニメーションが終了したら時間を戻す
                    Time.timeScale = 1;
                    //初期位置に戻す
                    this.transform.position = initialPos;

                });
            yield return new WaitForSeconds(stopTime + 0.01f);
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

    //必殺技関連
    //敵停止処理
    public override void EnemyStop()
    {
        if (isPlayerExAttack) return;
        if (enemyRb != null)
        {
            isPlayerExAttack = true;
            if (MovementCoroutine != null)
            {
                StopAllCoroutines();

                //停止用調整
                NotInAnim = false;
                if (isCrushAttack) isCrushAttack = false;
                if (isPincerAttack) isPincerAttack = false;
                if (isSummonAttack) isSummonAttack = false;
            }
            enemyRb.velocity = enemyLHRb.velocity = enemyRHRb.velocity = Vector2.zero;
        }
        if (animator != null)
        {
            animator.speed = 0;
        }
    }
    //必殺技が当たっていた場合のダメージ処理呼出し
    public override void PlaeyrExAttack_HitEnemyEnd(float power)
    {
        if (!isPlayerExAttack) return;
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        MovementCoroutine = StartCoroutine(ExBackIdleAnim());
    }
    //停止処理解除
    public override void Stop_End()
    {
        if (!isPlayerExAttack) return;
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        MovementCoroutine = StartCoroutine(ExBackIdleAnim());
    }


    public void OnHandTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HandScript>() != null && isPincerAttack)
        {
            isPincerAttack = false;
            enemyLHRb.velocity = Vector2.zero;
            enemyRHRb.velocity = Vector2.zero;
            SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
            StartCoroutine(PincerAttackAnim2());
        }
    }
    public void OnHandTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage") && enemyRHRb.velocity.y == 0 && isCrushAttack)
        {
            isCrushAttack = false;
            SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
            StartCoroutine(CrushAttackAnim2());
        }
        if (collision.CompareTag("Stage") && enemyLHRb.velocity.y == 0 && isSummonAttack)
        {
            isSummonAttack = false;
            SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
            shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
            StartCoroutine(SummonAttackAnim2());
        }
    }
    public void OnHandTriggerExit2D(Collider2D collision)
    {
        
    }


    //調整しない関数
    //Boss死亡時に呼ぶ関数
    virtual public void Boss_Down()
    {
        ComboParam.Instance.ComboStop();
        GameManager.Instance.PlayerExAttack_Start();
        GameManager.Instance.Result_Start(3);
    }

    protected override void OnDestroyMode()
    {
        isDestroy = true;
        IsBlowing = true;
        //必殺技ヒットエフェクト消す
        BossCheckOnCamera = false;
        OnCamera = false;
        //両手の当たり判定を消す
        var children = GetComponentsInChildren<HandScript>();
        foreach (var child in children)
        {
            child.DemonDead();
        }
        GameManager.Instance.AddKillEnemy();
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
        animator.SetBool("IsDestroy", isDestroy);
        LHanimator.SetBool("IsDestroy", isDestroy);
        RHanimator.SetBool("IsDestroy", isDestroy);
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

    protected void StopAnimatorRH()
    {
        RHanimator.speed = 0;
    }
    protected void StopAnimatorLH()
    {
        LHanimator.speed = 0;
    }
    protected void StartAnimatorRH()
    {
        RHanimator.speed = 1;
    }
    protected void StartAnimatorLH()
    {
        LHanimator.speed = 1;
    }
    protected void StopAnimatorLRH()
    {
        RHanimator.speed = LHanimator.speed = 0;
    }
    protected void StartAnimatorLRH()
    {
        RHanimator.speed = LHanimator.speed = 1;
    }
    protected void StopAllAnimator()
    {
        RHanimator.speed = LHanimator.speed = animator.speed = 0;
    }
    protected void StartAllAnimator()
    {
        RHanimator.speed = LHanimator.speed = animator.speed = 1;
    }
}
