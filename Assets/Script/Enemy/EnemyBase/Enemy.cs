using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Enemy : MonoBehaviour
{
    protected Animator animator;

    [SerializeField]
    protected string id;
    protected EnemyData enemyData;
    protected Rigidbody2D enemyRb;

    //移動速度内部関数
    protected float moveSpeed;
    //チェック用内部関数
    protected bool IsBlowing = false, IsMoving = true, IsAttacking = false, HadAttack = false, hadDamaged = false, PlayerNotAttacked = true, AttackChecking = true;

    //プレイヤー必殺技中かどうか
    [System.NonSerialized]
    public bool isPlayerExAttack;

    protected float hp;

    //吹っ飛び角度
    protected float forceAngle;
    protected Vector2 forceDirection = new Vector3(1.0f, 1.0f);
    [SerializeField, Header("吹っ飛び速度")]
    protected float speed = 15f;     //吹っ飛び速度
    //吹っ飛び中の煙エフェクト
    private GameObject smokeEffect;
    //消滅時エフェクト
    private GameObject deathEffect;
    private float effectInterval = 0.5f;
    protected float rotateSpeed = 10f;//吹っ飛び回転速度

    //反射回数
    int maxReflexNum;
    internal int reflexNum;

    protected GameObject player;

    //敵の点滅
    SpriteRenderer sprite;
    protected enum moveType
    {
        NotMove, //動かない
        Move,    //動く
        FlyMove　//飛ぶ
    }
    protected moveType type;

    protected bool isDestroy = false;
    protected bool OnCamera = false;

    protected Transform _transform;

    // 前フレームのワールド位置
    protected Vector2 _prevPosition;

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
        speed = EnemyGeneratar.instance.speed;
        smokeEffect = EnemyGeneratar.instance.smokeEffect;
        effectInterval = EnemyGeneratar.instance.effectInterval;

        //消滅時に使用
        deathEffect = EnemyGeneratar.instance.deathEffect;

        //敵の点滅
        sprite = GetComponent<SpriteRenderer>();
    }

    virtual protected void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("Player"))
        {
            Attack(col);
        }
        if (col.gameObject.CompareTag("Stage") && isDestroy)
        {
            reflexNum--;
            if (reflexNum == 0)
            {
                SoundManager.Instance.PlaySE(SESoundData.SE.MonsterDead);
                GameObject obj = Instantiate(deathEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    virtual protected void OnCollisionStay2D(Collision2D col)
    {
        if (!isDestroy)
        {
            if (col.gameObject.CompareTag("Player")&&!HadAttack)
            {
                Attack(col);
            }
            if (col.gameObject.CompareTag("Player"))
            {
                enemyRb.velocity = new Vector2(0, enemyRb.velocity.y);
            }
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
            
            EnemyRotate();
        }
    }

    virtual protected void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        Gravity();
    }

    //攻撃
    protected void Attack(Collision2D col)
    {
        if (!HadAttack)
        {
            //攻撃クールダウンタイム
            HadAttack = true;
            StartCoroutine(HadAttackReset());
            //ダメージとノックバック
            col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 15 * enemyData.knockBackValue);
            col.gameObject.GetComponent<PlayerController>()._Damage((int)enemyData.power);
        }
    }

    public virtual void Damage(float power)
    {
        //既に死亡状態の場合
        if (isDestroy)
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
            ComboParam.Instance.ResetTime();
            reflexNum = maxReflexNum;
            CalcForceDirection();
            BoostSphere();
            return;
        }

        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
        ComboParam.Instance.ResetTime();
        hp -= power;
        
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

    protected void EnemyMove()
    {
        
    }

    protected virtual void Destroy()
    {
        GameManager.Instance.AddKillEnemy();
        //反射用のコライダーに変更
        this.GetComponent<BoxCollider2D>().enabled = false;
        this.GetComponent<CircleCollider2D>().enabled = true;
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        enemyRb.gravityScale = 0;
        enemyRb.constraints = RigidbodyConstraints2D.None;
        CalcForceDirection();
        //吹っ飛び開始
        BoostSphere();
        isDestroy = true;
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterKnock);
        gameObject.layer = LayerMask.NameToLayer("PinBallEnemy");
        if(smokeEffect != null)
        {
            StartCoroutine(BlowAwayEffect());
        }
    }

    //吹っ飛び発生
    protected void BoostSphere()
    {
        // 向きと力の計算
        Vector2 force = speed * forceDirection;

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
        StartCoroutine(BlowAwayEffect());
        yield return new WaitForSeconds(0.25f);
        Destroy(obj);
    }

    protected void CalcForceDirection()
    {
        // 入力された角度をラジアンに変換
        float rad = forceAngle * Mathf.Deg2Rad;

        //オブジェクトを取得
        player = serchTag(gameObject, "Player");

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
        var n = 50;
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


    //重力
    protected virtual void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -5));
    }

    public virtual void EnemyStop() 
    {
        isPlayerExAttack = true;
        enemyRb.velocity = Vector2.zero;
        if(animator != null)
        {
            animator.speed = 0;
        }
    }

    //必殺技が当たっていた場合
    public virtual void PlaeyrExAttack_HitEnemyEnd(float powar)
    {
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
        Damage(powar);
    }

    //停止処理解除
    public virtual void Stop_End()
    {
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
    }

    //HPとFullHPの獲得
    public float GetEnemyHP()
    {
        return hp;
    }
    public float GetEnemyFullHP()
    {
        return enemyData.hp;
    }



}
