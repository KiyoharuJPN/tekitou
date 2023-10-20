using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dragon;

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




    // パタンシステム関連
    enum EnemyPatternSettings
    {
        IdleAnim,
        MoveAnim,
        SummonAttackAnim,
        CrushAttackAnim,
        PincerAttackAnim,
    }
    [Header("パタン調整（パタンの動きは敵の仕様書を参照してください）")]
    [SerializeField] List<EnemyPatternSettings> Pattern1, Pattern2, Pattern3;
    public BossHPBar HPBar;
    public GameObject LeftHand, RightHand;

    //内部関数
    //プレイヤーのオブジェクト
    GameObject Player;
    //攻撃パタンを記録する関数
    int EnemyAnim = -1, EnemyPattern = -1, EnemyPatternPreb = -1, AnimationController = -1, JumpAttackAnimCtrl = -1;

    //アニメチェック、パターンチェック
    bool NotInAnim = true, PatternOver = true, patternover = false, isSummonAttack = false, isCrushAttack = false, isPincerAttack = false;

    private void OnEnable()
    {
        if(animator != null)
        {
            animator.SetBool("InAdanim", false);
            LHanimator.SetBool("InAdanim", false);
            RHanimator.SetBool("InAdanim", false);
        }

    }

    protected override void Start()
    {
        base.Start();

        //animator代入
        LHanimator = LeftHand.GetComponent<Animator>();
        RHanimator = RightHand.GetComponent<Animator>();
        //プレイヤーのオブジェクト
        Player = GameObject.Find("Hero");
        //カメラ揺れ
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        //使用方法
        //shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
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
            switch (EnemyPattern)
            {
                case 0:
                    //アニメが流れていなければ次のアニメを流れる(コルーチンでtrue修正)
                    if (EnemyAnim < Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        //アニメを流れる
                        StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //次のアニメ
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
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
        
    }



    //内部関数




    //攻撃関数




    //外部関数
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        //ヒットストップ
        StartCoroutine(DamegeProcess(power, skill, isHitStop, exSkill));
    }

    protected override IEnumerator DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
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
            Vector3 initialPos = this.transform.position;//初期位置保存
            Time.timeScale = 0;

            //ヒットストップ処理開始
            tween = transform.DOShakePosition(power * stopState.shakTime, stopState.shakPowar, stopState.shakNum, stopState.shakRand)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    //アニメーションが終了したら時間を戻す
                    Time.timeScale = 1;
                    //初期位置に戻す
                    this.transform.position = initialPos;

                });
            yield return new WaitForSeconds(power * stopState.shakTime + 0.01f);
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

        if (hp <= 0)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            OnDestroyMode();
        }
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
        GameManager.Instance.AddKillEnemy();
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
        isDestroy = true;
        IsBlowing = true;
    }
}
