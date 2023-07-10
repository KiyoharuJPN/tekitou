using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Enemy
{
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
    [SerializeField] List<EnemyPatternSettings> Pattern1,Pattern2,Pattern3;
    public BossHPBar HPBar;
    //攻撃パタンを記録する関数
    int EnemyAnim = -1, EnemyPattern = -1, EnemyPatternPreb = -1, AnimationController = -1;

    //アニメチェック、パターンチェック
    bool NotInAnim = true, PatternOver = true, patternover = false, isFlameBracing =false;

    BoxCollider2D EnemyCollider;
    protected override void Start()
    {
        base.Start();
        moveSpeed = MoveSpeed * -1;
        EnemyCollider = GetComponent<BoxCollider2D>();
    }
    //ドラゴンの動き
    protected override void Update()
    {
        base.Update();
        //animatorの設定
        animator.SetBool("IsBlowing", isDestroy);

        //敵のパターンをランダムで選択
        if (PatternOver)
        {
            //ランダムで敵のパターンを選ぶ
            while(EnemyPattern == EnemyPatternPreb)
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
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

        //アニメの終わり（必須）
        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;                   //次の動きに移動できるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
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
        Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");

        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;
        if (patternover)
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
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
            Debug.Log("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
        }

        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;
        if (patternover)
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
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
        ResetAttackCheckArea();
        dragonAttackCheckArea.gameObject.SetActive(true);
        while (animcheck < 79)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea1();
        while (animcheck < 86)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea2();
        while (animcheck < 93)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea3();
        while (animcheck < 100)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea4();
        while (animcheck < 107)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea5();
        while (animcheck < 114)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea1();
        while (animcheck < 121)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea2();
        while (animcheck < 128)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea3();
        while (animcheck < 135)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea4();
        while (animcheck < 142)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea5();
        while (animcheck < 149)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea1();
        while (animcheck < 156)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea2();
        while (animcheck < 163)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea3();
        while (animcheck < 170)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea4();
        while (animcheck < 177)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea5();
        while (animcheck < 177)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckAreaOver();
        while (animcheck < 184)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        dragonAttackCheckArea.gameObject.SetActive(false);
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
            //Debug.Log("Pattern Over");
        }
    }

    IEnumerator SlewAttackAnim()
    {
        AnimationController = 3;        //animator調整（必須）
        animator.SetInteger("AnimationController", AnimationController);

        //攻撃力とノックバックを変える
        var attackdam = enemyData.power;
        enemyData.power = 2;
        var knockbackval = enemyData.knockBackValue;
        enemyData.knockBackValue = 5;

        float animcheck = 0;
        while(animcheck < 15)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        EnemyCollider.offset = new Vector2(1.45f, -1.2f);
        EnemyCollider.size = new Vector2(9.6f, 4);
        while(animcheck < 23)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        EnemyCollider.offset = new Vector2(-3.9f, -1.2f);
        EnemyCollider.size = new Vector2(10, 4);
        while (animcheck < 31)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        EnemyCollider.size = new Vector2(11.3f, 4);
        while (animcheck < 39)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        ResetBoxCollider2D();
        while (animcheck < 65)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //攻撃力とノックバックを戻す
        enemyData.power = attackdam;
        enemyData.knockBackValue = knockbackval;
        //アニメの終わり（必須）
        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;                   //次の動きに移動できるようにする
        if (patternover)                    //パターンが終わる時に呼ばれる関数
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }


    //外部関数
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
                col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 3);
                col.gameObject.GetComponent<PlayerController>()._Damage(2);
            }
            
            
        }
    }



    //内部関数
    protected override void Destroy()
    {
        GameManager.Instance.AddKillEnemy();
        this.GetComponent<BoxCollider2D>().enabled = false;
        isDestroy = true;
        //扉の出現を内部で実装するときにここに書けば実装できる
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
        dragonAttackCheckArea.offset = new Vector2(-8, -1.25f);
        dragonAttackCheckArea.size = new Vector2(7.1f, 3.9f);
    }
    void AttackCheckArea1()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.6f, -0.85f);
        dragonAttackCheckArea.size = new Vector2(8.2f, 4.7f);
    }
    void AttackCheckArea2()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.6f, -0.7f);
        dragonAttackCheckArea.size = new Vector2(8.2f, 5f);
    }
    void AttackCheckArea3()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.6f, -0.35f);
        dragonAttackCheckArea.size = new Vector2(8.5f, 5.7f);
    }
    void AttackCheckArea4()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.6f, -0.1f);
        dragonAttackCheckArea.size = new Vector2(8.5f, 6.2f);
    }
    void AttackCheckArea5()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.5f, -0.1f);
        dragonAttackCheckArea.size = new Vector2(8.25f, 6.2f);
    }
    void AttackCheckAreaOver()
    {
        dragonAttackCheckArea.offset = new Vector2(-9f, -1f);
        dragonAttackCheckArea.size = new Vector2(6.75f, 4.45f);
    }


}
