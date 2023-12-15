using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using DG.Tweening;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Enemy : MonoBehaviour
{
    protected Animator animator;

    [SerializeField]
    protected GameObject EnemyColliderArea;
    [SerializeField]
    protected string id;
    protected EnemyData enemyData;
    protected Rigidbody2D enemyRb;

    //移動速度内部関数
    protected float moveSpeed;
    //チェック用内部関数
    protected bool IsBlowing = false, IsMoving = true, IsAttacking = false, hadDamaged = false, PlayerNotAttacked = true, HadAttack = false, AttackChecking = true;

    //プレイヤー必殺技中かどうか
    [System.NonSerialized]
    public bool isPlayerExAttack;
    public bool HadContactDamage = true;

    protected float hp;

    //吹っ飛び角度
    protected Vector2 BlowingSpeedPreb = Vector2.zero;
    protected float forceAngle;
    protected Vector2 forceDirection = new Vector3(1.0f, 1.0f), buffForceDirection = new Vector3(1.0f, 1.0f);
    protected float speed = 15f;     //吹っ飛び速度
    //吹っ飛び中の煙エフェクト
    private GameObject smokeEffect;
    //消滅時エフェクト
    private GameObject deathEffect;
    private float effectInterval = 0.5f;
    protected float rotateSpeed = 10f;//吹っ飛び回転速度

    //反射回数＆反射関連
    int maxReflexNum;
    internal int reflexNum;
    float rad, minRad, maxRad;

    protected GameObject player;

    //ダメージ処理中か
    bool isDamege = false;

    //敵の点滅
    SpriteRenderer sprite;
    protected enum moveType
    {
        NotMove, //動かない
        Move,    //動く
        FlyMove　//飛ぶ
    }
    protected moveType type;

    internal bool isDestroy = false;
    internal bool OnCamera = false;

    protected Transform _transform;

    // 前フレームのワールド位置
    protected Vector2 _prevPosition;


    //Buff関連
    EnemyBuffSystem _EnemyBuff;

    //ヒットストップステータス
    internal EnemyGeneratar.HitStopState stopState;
    //ヒットストップバフ
    bool _isDestroyed = false, _isHitStoped = false, hadEnemyBuff = false;

    protected Tween tween;

    protected virtual void Start()
    {
        //idで指定した敵データ読込
        enemyData = EnemyGeneratar.instance.EnemySet(id);
        hp = enemyData.hp;
        enemyRb = GetComponent<Rigidbody2D>();
        maxReflexNum = enemyData.num;
        reflexNum = maxReflexNum;
        forceAngle = enemyData.angle;

        animator = GetComponent<Animator>();

        //吹っ飛び中に使用
        _transform = transform;
        _prevPosition = _transform.position;
        speed = enemyData.speed;
        smokeEffect = EnemyGeneratar.instance.smokeEffect;
        effectInterval = EnemyGeneratar.instance.effectInterval;

        //消滅時に使用
        deathEffect = EnemyGeneratar.instance.deathEffect;

        //敵の点滅
        sprite = GetComponent<SpriteRenderer>();

        //Buff関連
        if (GetComponentInChildren<EnemyBuffSystem>())
        {
            _EnemyBuff = GetComponentInChildren<EnemyBuffSystem>();
            hadEnemyBuff = true;
        }

        stopState = EnemyGeneratar.instance.stopState;

        // 入力された角度をラジアンに変換
        rad = forceAngle * Mathf.Deg2Rad;
        minRad = (forceAngle - 10) * Mathf.Deg2Rad;
        maxRad = (forceAngle + 10) * Mathf.Deg2Rad;
    }

    virtual protected void OnColEnter2D(Collider2D col)
    {
        if (!isDestroy && HadContactDamage)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Attack(col);
            }
        }
        if (col.gameObject.CompareTag("Stage") && isDestroy)
        {
            reflexNum--;
            Debug.Log(reflexNum);
            if (reflexNum == 0)
            {
                EnemyNomalDestroy();
            }
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage") && isDestroy)
        {
            reflexNum--;
            if (reflexNum == 0)
            {
                SoundManager.Instance.PlaySE(SESoundData.SE.MonsterDead);
                GameObject obj = Instantiate(deathEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);

                if (_EnemyBuff) _EnemyBuff._Destroy();
                Destroy(gameObject);
            }
            //EnemyReflection(collision);
        }
    }

    virtual protected void OnColStay2D(Collider2D col)
    {
        if (!isDestroy && HadContactDamage)
        {
            if (col.gameObject.CompareTag("Player")&&!HadAttack)
            {
                Attack(col);
            }
            //if (col.gameObject.CompareTag("Player"))
            //{
            //    enemyRb.velocity = new Vector2(0, enemyRb.velocity.y);
            //}
        }
    }

    virtual protected void Update()
    {
        if (isPlayerExAttack)
        {
            this.transform.Rotate(0, 0, 0);
            if (enemyRb.bodyType != RigidbodyType2D.Static)
            {
                this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            return;
        }

        //吹っ飛び中以外は行わない
        if (!isDestroy)
            return;
        //吹っ飛び中の処理
        if (isDestroy)
        {
            EnemyRotate();//回転
        }
    }

    virtual protected void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        Gravity();
    }

    //攻撃
    protected void Attack(Collider2D col)
    {
        if (!HadAttack)
        {
            //プレイヤーの攻撃を食らっている最中に攻撃できなくする
            if (isDamege) return;
            //一時使用停止
            //攻撃クールダウンタイム
            HadAttack = true;
            StartCoroutine(HadAttackReset());

            //ダメージとノックバック
            col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 15 * enemyData.knockBackValue);
            col.gameObject.GetComponent<PlayerController>().Damage((int)enemyData.power);
        }
    }

    //ダメージ処理呼出し
    public virtual void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        isDamege = true;
        if (!_isHitStoped)
        {
            _isHitStoped = true;
            //ダメージ処理開始
            StartCoroutine(DamegeProcess(power, skill, isHitStop, exSkill));
        }
    }
    
    //ダメージ処理（ヒットストップの関係でコルーチンに変更）
    protected virtual IEnumerator DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
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
            isHitStop = true;
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
            yield return new WaitForSeconds(stopTime);
        }

        //ヒット時演出（敵点滅）
        if (!hadDamaged)
        {
            StartCoroutine(HadDamaged());
            hadDamaged = true;
        }

        //既に死亡状態の場合
        if (isDestroy)
        {
            if (_EnemyBuff != null)
            {
                if (exSkill) _EnemyBuff.SetEXAttackDecrease(7);　//必殺技ヒット時処理

                if (_EnemyBuff.GetBuffAttackCheckCount() > 0)　//追撃回数が０以上の場合
                {
                    //SE・コンボ時間リセット
                    SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
                    ComboParam.Instance.ResetTime();

                    //追撃回数UIに対しての処理
                    _EnemyBuff.ShowAttackChecking();

                    _isHitStoped = false;
                }
                else if ((_EnemyBuff.GetBuffAttackCheckCount() == 0) || (_EnemyBuff.GetBuffAttackCheckCount() <= 0 && exSkill))
                {
                    //追撃回数UIに対しての処理
                    _EnemyBuff.ShowAttackChecking();
                    //消滅時SE再生
                    SoundManager.Instance.PlaySE(SESoundData.SE.MonsterDead);

                    //バフゲット時消滅エフェクト
                    GameObject obj = Instantiate(_EnemyBuff.GetBuffEffect(), new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);
                    obj.GetComponent<SpriteRenderer>().color = _EnemyBuff.GetColorByType();
                    //プレイヤーにバフセット
                    GameManager.Instance.SetBuff((int)_EnemyBuff.GetBuffType());

                    if (tween != null)
                    {
                        tween.Kill();
                        //アニメーションが終了したら時間を戻す
                        Time.timeScale = 1;
                    }

                    Destroy(gameObject);

                    yield break;
                    //if (_isHitStoped)
                    //{
                    //    _isDestroyed = true;
                    //    OnCamera = false;
                    //    gameObject.SetActive(false);
                    //}
                    //else
                    //{
                    //    if (tween != null)
                    //    {
                    //        tween.Kill();
                    //        //アニメーションが終了したら時間を戻す
                    //        Time.timeScale = 1;
                    //    }
                    //    Destroy(gameObject);
                    //}
                }
            }
            else if(_EnemyBuff == null && hadEnemyBuff)
            {
                Destroy(gameObject);
            }

            //反射回数リセット
            reflexNum = maxReflexNum;

            //吹き飛び処理
            BlownAway();
            yield break;
        }
        //死亡時ではない場合
        else if (!isDestroy)
        {
            //体力減少
            hp -= power;

            //体力がなくなった場合死亡
            if (hp <= 0)
            {
                //スコア追加
                PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);

                //死亡時処理
                OnDestroyMode();
            }

            _isHitStoped = false;
        }
        isDamege = false;
    }

    protected void EnemyMove()
    {
        
    }

    //死亡時処理
    protected virtual void OnDestroyMode()
    {
        if (this.gameObject.GetComponent<MonsterHouse_Enemy>())
        {
            this.gameObject.GetComponent<MonsterHouse_Enemy>().Destroy();
        }
        //死亡状態に変更
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

        if (_EnemyBuff != null)
            _EnemyBuff.ShowAttackChecking();

        if(smokeEffect != null)
            StartCoroutine(BlowAwayEffect());
    }

    //吹き飛び処理
    protected void BlownAway()
    {
        //吹っ飛び開始
        CalcForceDirection(); //吹き飛び方向計算
        BoostSphere();        //velocity付与
    }

    //吹っ飛び発生
    protected void BoostSphere()
    {
        // 向きと力の計算
        Vector2 force = speed * forceDirection;
        // 力を加えるメソッド
        enemyRb.velocity = force;
    }
    public void BuffBoostSphere()
    {
        if(_EnemyBuff != null)
        {
            buffForceDirection = enemyRb.velocity.normalized;
            // 向きと力の計算
            Vector2 force = (speed + BuffBlowingSpeed()) * buffForceDirection;
            // 力を加えるメソッド
            enemyRb.velocity = force;
        }
    }

    protected void EnemyReflection(Collision2D collision)
    {
        Debug.Log(collision.GetContact(0).point);
        Debug.Log(transform.position);

        forceDirection = collision.relativeVelocity.normalized;
        // 向きと力の計算
        Vector2 force = (speed + BuffBlowingSpeed()) * forceDirection;
        // 力を加えるメソッド
        enemyRb.velocity = force;
    }

    //吹っ飛び中回転
    private void EnemyRotate()
    {
        //右方向に動いている
        if (enemyRb.velocity.x > 0.1)
        {
            this.transform.Rotate(0, 0, -rotateSpeed);
        }
        //左方向に動いている
        else if (enemyRb.velocity.x < -0.1)
        {
            this.transform.Rotate(0, 0, rotateSpeed);
        }
    }

    //吹き飛び中のエフェクト生成
    private IEnumerator BlowAwayEffect()
    {
        yield return new WaitForSeconds(effectInterval);
        GameObject obj =  Instantiate(smokeEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);
        yield return new WaitForSeconds(0.25f);
        Destroy(obj);
        StartCoroutine(BlowAwayEffect());
    }

    protected void CalcForceDirection()
    {

        //オブジェクトを取得
        player = serchTag(gameObject, "Player");
        if (player == null)
        {
            player = serchTag(gameObject, "InvinciblePlayer");
            //Debug.Log(player);
        }
        if(player == null)
        {
            player = serchTag(gameObject, "DeadPlayer");
            //Debug.Log(player);
        }
        else { /*Debug.Log(player);*/ }

        // それぞれの軸の成分を計算
        float x = Mathf.Cos(rad);
        float y = Mathf.Sin(rad);

        //プレイヤーと自身の位置関係を調査
        if (enemyData.type == EnemyData.EnemyType.FlyEnemy || isDestroy)
        {
            if (player.transform.position.y + 0.3f < this.transform.position.y)
            { y = -y; }
        }
        if(player.transform.position.x > this.transform.position.x) 
        { x = -x; }
        
        // Vector3型に格納
        forceDirection = new Vector2(x, y);
    }

    //指定されたタグの中で最も近いものを取得
    protected GameObject serchTag(GameObject nowObj, string tagName)
    {
        float tmpDis = 0;           //距離用一時変数
        float nearDis = 0;          //最も近いオブジェクトの距離
        //string nearObjName = "";    //オブジェクト名称
        GameObject targetObj = null; //オブジェクト

        //タグ指定されたオブジェクトを配列で取得する
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //オブジェクトの距離が近いか、距離0であればオブジェクト名を取得
            //一時変数に距離を格納
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                //nearObjName = obs.name;
                targetObj = obs;
            }

        }
        //最も近かったオブジェクトを返す
        //return GameObject.Find(nearObjName);
        return targetObj;
    }

    //画面に入ったどうかをチェック
    protected void OnBecameVisible()
    {
        OnCamera = true;
    }
    protected void OnBecameInvisible()
    {
        OnCamera = false;
    }

    //移動方向の回転
    public virtual void TurnAround()
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

    //外から今の移動状態を確認
    public bool GetIsMoving()
    {
        return IsMoving;
    }

    //外から今の吹き飛ばし状態確認
    public bool GetIsBlowing()
    {
        return isDestroy;
    }

    //攻撃力を外で取得する
    public int GetDamage()
    {
        return enemyData.attackPower;
    }

    //ノックバック力を外で取得する/
    public float GetKnockBackForce()
    {
        return enemyData.knockBackValue;
    }

    //プレイヤーが攻撃エリアに要る時の動き（AttackCheckAreaから呼ばれる）
    public virtual bool PlayerInAttackArea()
    {
        var InAttack = false;
        //trueの修正は各スクリプトで書いてください。
        if (IsMoving && AttackChecking)
        {
            AttackChecking = false;
            //コルーチン関数をここで作ります
            InAttack = true;
        }
        return InAttack;
    }
    //攻撃されたどうかをチェック
    public virtual bool GetPlayerAttacked()
    {
        //trueの修正は各スクリプトで書いてください。
        return PlayerNotAttacked;
    }
    public virtual void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }

    //攻撃クールダウン
    protected IEnumerator HadAttackReset()
    {
        var n = 100;
        while(n > 0)
        {
            n--;
            yield return new WaitForSeconds(0.01f);
        }
        HadAttack = false;
    }

    protected IEnumerator HadDamaged()
    {
        sprite.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        sprite.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        sprite.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        sprite.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        sprite.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        sprite.color = new Color(1, 1, 1);
        hadDamaged = false;
    }

    protected void DefaultColor()
    {
        sprite.color = new Color(1, 1, 1);
    }

    //重力
    protected virtual void Gravity()
    {
        if(!isDestroy) enemyRb.AddForce(new Vector2(0, -5));
    }

    //敵停止処理
    public virtual void EnemyStop() 
    {
        if (enemyRb != null)
        {
            isPlayerExAttack = true;
            if (isDestroy) { BlowingSpeedPreb = enemyRb.velocity; }
            enemyRb.velocity = Vector2.zero;
        }
        if(animator != null)
        {
            animator.speed = 0;
        }
    }

    //必殺技が当たっていた場合のダメージ処理呼出し
    public virtual void PlaeyrExAttack_HitEnemyEnd(float power)
    {
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
        Damage(power, null,true, true);
    }

    //停止処理解除
    public virtual void Stop_End()
    {
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        if (isDestroy) { enemyRb.velocity = BlowingSpeedPreb; }
        isPlayerExAttack = false;
    }

    //HPとFullHPの獲得
    public float GetEnemyHP()
    {
        return hp;
    }
    public float GetEnemyFullHP()
    {
        if (enemyData == null)
        {
            enemyData = EnemyGeneratar.instance.EnemySet(id);
            hp = enemyData.hp;
        }
        return enemyData.hp;
    }

    public void OnColEnter(Collider2D col)
    {
        OnColEnter2D(col);
    }
    
    public void OnColStay(Collider2D col)
    {
        OnColStay2D(col);
    }

    //通常削除
    public void EnemyNomalDestroy()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterDead);
        GameObject obj = Instantiate(deathEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);

        if (_EnemyBuff) _EnemyBuff._Destroy();

        if (tween != null)
        {
            tween.Kill();
            //アニメーションが終了したら時間を戻す
            Time.timeScale = 1;
        }
        Destroy(gameObject);
    }

    //ヒットエフェクト生成
    internal void HitEfect(Transform enemy, int angle)
    {
        GameObject prefab =
        Instantiate(GameManager.Instance.hitEffect, new Vector2(enemy.position.x, enemy.position.y), Quaternion.identity);
        prefab.transform.Rotate(new Vector3(0, 0, angle));
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
        _EfectDestroy(prefab, 0.2f);
    }
    //エフェクト削除
    void _EfectDestroy(GameObject prefab, float time)
    {
        Destroy(prefab, time);
    }

    //バフによる吹き飛び速度変更
    float BuffBlowingSpeed()
    {
        switch (_EnemyBuff.GetBuffBlowingSpeed())
        {
            case 0:
                return 0f;
            case 1:
                return 0.2f;
            case 2:
                return 0.4f;
            case 3:
                return 0.7f;
            case 4:
                return 1f;
            case 5:
                return 1.6f;
            case 6:
                return 2.4f;
            case 7:
                return 3.4f;
            case 8:
                return 4.6f;
            case 9:
                return 6.2f;
            default:
                return 8f;
        }
    }
}
