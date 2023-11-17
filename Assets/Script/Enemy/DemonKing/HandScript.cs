using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : Enemy
{
    DemonKing demonKing;
    Renderer spritehand;

    protected override void Start()
    {
        demonKing = GetComponentInParent<DemonKing>();
        //敵の点滅
        spritehand = GetComponent<Renderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (demonKing != null)
        {
            demonKing.OnHandTriggerEnter2D(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(demonKing != null)
        {
            demonKing.OnHandTriggerStay2D(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (demonKing != null)
        {
            demonKing.OnHandTriggerExit2D(collision);
        }
    }

    private void StopHandAnimation()
    {
        GetComponent<Animator>().speed = 0;
    }

    //protected virtual void Start()
    //{
    //    //idで指定した敵データ読込
    //    enemyData = EnemyGeneratar.instance.EnemySet(id);
    //    hp = enemyData.hp;
    //    enemyRb = GetComponent<Rigidbody2D>();
    //    maxReflexNum = enemyData.num;
    //    reflexNum = maxReflexNum;
    //    forceAngle = enemyData.angle;

    //    animator = GetComponent<Animator>();

    //    //吹っ飛び中に使用
    //    _transform = transform;
    //    _prevPosition = _transform.position;
    //    speed = enemyData.speed;
    //    smokeEffect = EnemyGeneratar.instance.smokeEffect;
    //    effectInterval = EnemyGeneratar.instance.effectInterval;

    //    //消滅時に使用
    //    deathEffect = EnemyGeneratar.instance.deathEffect;

    //    //敵の点滅
    //    sprite = GetComponent<SpriteRenderer>();

    //    //Buff関連
    //    if (GetComponentInChildren<EnemyBuffSystem>())
    //    {
    //        _EnemyBuff = GetComponentInChildren<EnemyBuffSystem>();
    //        hadEnemyBuff = true;
    //    }

    //    stopState = EnemyGeneratar.instance.stopState;
    //}

    override protected void OnColEnter2D(Collider2D col)
    {

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {

    }

    override protected void OnColStay2D(Collider2D col)
    {

    }

    override protected void Update()
    {

    }

    override protected void FixedUpdate()
    {

    }

    ////攻撃
    //protected void Attack(Collider2D col)
    //{
    //    if (!HadAttack)
    //    {
    //        //一時使用停止
    //        //攻撃クールダウンタイム
    //        //HadAttack = true;
    //        //StartCoroutine(HadAttackReset());

    //        //ダメージとノックバック
    //        col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 15 * enemyData.knockBackValue);
    //        col.gameObject.GetComponent<PlayerController>()._Damage((int)enemyData.power);
    //    }
    //}

    //ダメージ処理呼出し
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        demonKing.Damage(power, skill, isHitStop, exSkill);
        StartCoroutine(DamegeProcess(power, skill, isHitStop, exSkill));
    }

    //ダメージ処理（ヒットストップの関係でコルーチンに変更）
    protected override IEnumerator DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
    {
        //ヒットエフェクト生成
        if (skill != null)
        {
            HitEfect(this.transform, skill.hitEffectAngle);
        }
        else HitEfect(this.transform, UnityEngine.Random.Range(0, 360));


        //ヒット時演出（敵点滅）
        if (!hadDamaged)
        {
            StartCoroutine(HadDamagedHand());
            hadDamaged = true;
        }

        yield return null;
    }

    //protected void EnemyMove()
    //{

    //}

    //死亡時処理
    protected override void OnDestroyMode()
    {

    }

    ////吹き飛び処理
    //protected void BlownAway()
    //{
    //    //吹っ飛び開始
    //    CalcForceDirection(); //吹き飛び方向計算
    //    BoostSphere();        //velocity付与
    //}

    ////吹っ飛び発生
    //protected void BoostSphere()
    //{
    //    // 向きと力の計算
    //    Vector2 force = speed * forceDirection;

    //    // 力を加えるメソッド
    //    enemyRb.velocity = force;
    //}
    //public void BuffBoostSphere()
    //{
    //    if (_EnemyBuff != null)
    //    {
    //        // 向きと力の計算
    //        Vector2 force = (speed + BuffBlowingSpeed()) * forceDirection;

    //        // 力を加えるメソッド
    //        enemyRb.velocity = force;
    //    }
    //}

    ////吹っ飛び中回転
    //private void EnemyRotate()
    //{
    //    //右方向に動いている
    //    if (enemyRb.velocity.x > 0.1)
    //    {
    //        this.transform.Rotate(0, 0, -rotateSpeed);
    //    }
    //    //左方向に動いている
    //    else if (enemyRb.velocity.x < -0.1)
    //    {
    //        this.transform.Rotate(0, 0, rotateSpeed);
    //    }
    //}

    ////吹き飛び中のエフェクト生成
    //private IEnumerator BlowAwayEffect()
    //{
    //    yield return new WaitForSeconds(effectInterval);
    //    GameObject obj = Instantiate(smokeEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);
    //    yield return new WaitForSeconds(0.25f);
    //    Destroy(obj);
    //    StartCoroutine(BlowAwayEffect());
    //}

    //protected void CalcForceDirection()
    //{
    //    // 入力された角度をラジアンに変換
    //    float rad = forceAngle * Mathf.Deg2Rad;

    //    //オブジェクトを取得
    //    player = serchTag(gameObject, "Player");
    //    if (player == null)
    //    {
    //        player = serchTag(gameObject, "InvinciblePlayer");
    //    }

    //    // それぞれの軸の成分を計算
    //    float x = Mathf.Cos(rad);
    //    float y = Mathf.Sin(rad);

    //    //プレイヤーと自身の位置関係を調査
    //    if (enemyData.type == EnemyData.EnemyType.FlyEnemy || isDestroy)
    //    {
    //        if (player.transform.position.y + 0.3f < this.transform.position.y)
    //        { y = -y; }
    //    }
    //    if (player.transform.position.x > this.transform.position.x)
    //    { x = -x; }

    //    // Vector3型に格納
    //    forceDirection = new Vector2(x, y);
    //}

    ////指定されたタグの中で最も近いものを取得
    //protected GameObject serchTag(GameObject nowObj, string tagName)
    //{
    //    float tmpDis = 0;           //距離用一時変数
    //    float nearDis = 0;          //最も近いオブジェクトの距離
    //    //string nearObjName = "";    //オブジェクト名称
    //    GameObject targetObj = null; //オブジェクト

    //    //タグ指定されたオブジェクトを配列で取得する
    //    foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
    //    {
    //        //自身と取得したオブジェクトの距離を取得
    //        tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

    //        //オブジェクトの距離が近いか、距離0であればオブジェクト名を取得
    //        //一時変数に距離を格納
    //        if (nearDis == 0 || nearDis > tmpDis)
    //        {
    //            nearDis = tmpDis;
    //            //nearObjName = obs.name;
    //            targetObj = obs;
    //        }

    //    }
    //    //最も近かったオブジェクトを返す
    //    //return GameObject.Find(nearObjName);
    //    return targetObj;
    //}

    ////画面に入ったどうかをチェック
    //protected void OnBecameVisible()
    //{
    //    OnCamera = true;
    //}
    //protected void OnBecameInvisible()
    //{
    //    OnCamera = false;
    //}

    ////移動方向の回転
    //public override void TurnAround()
    //{
    //    bool InCheck = true;
    //    if (transform.localScale.x == 1f && InCheck)
    //    {
    //        transform.localScale = new Vector3(-1f, 1f, 1f);
    //        InCheck = false;
    //    }
    //    if (transform.localScale.x == -1f && InCheck)
    //    {
    //        transform.localScale = new Vector3(1f, 1f, 1f);
    //        //InCheck = false;
    //    }
    //    moveSpeed *= -1;
    //}

    ////外から今の移動状態を確認
    //public bool GetIsMoving()
    //{
    //    return IsMoving;
    //}

    ////外から今の吹き飛ばし状態確認
    //public bool GetIsBlowing()
    //{
    //    return isDestroy;
    //}

    ////攻撃力を外で取得する
    //public int GetDamage()
    //{
    //    return enemyData.attackPower;
    //}

    ////ノックバック力を外で取得する/
    //public float GetKnockBackForce()
    //{
    //    return enemyData.knockBackValue;
    //}

    //プレイヤーが攻撃エリアに要る時の動き（AttackCheckAreaから呼ばれる）
    public override bool PlayerInAttackArea()
    {
        var InAttack = false;
        ////trueの修正は各スクリプトで書いてください。
        //if (IsMoving && AttackChecking)
        //{
        //    AttackChecking = false;
        //    //コルーチン関数をここで作ります
        //    InAttack = true;
        //}
        return InAttack;
    }
    //攻撃されたどうかをチェック
    public override bool GetPlayerAttacked()
    {
        //trueの修正は各スクリプトで書いてください。
        return PlayerNotAttacked;
    }
    public override void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }

    ////攻撃クールダウン
    //protected IEnumerator HadAttackReset()
    //{
    //    var n = 100;
    //    while (n > 0)
    //    {
    //        n--;
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    HadAttack = false;
    //}

    protected IEnumerator HadDamagedHand()
    {
        spritehand.material.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        spritehand.material.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        spritehand.material.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        spritehand.material.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        spritehand.material.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        spritehand.material.color = new Color(1, 1, 1);
        hadDamaged = false;
    }

    //protected void DefaultColor()
    //{
    //    sprite.color = new Color(1, 1, 1);
    //}

    //重力
    protected override void Gravity()
    {
        
    }

    //敵停止処理
    public override void EnemyStop()
    {
        demonKing.EnemyStop();
        if (enemyRb != null)
        {
            isPlayerExAttack = true;
            enemyRb.velocity = Vector2.zero;
        }
        if (animator != null)
        {
            animator.speed = 0;
        }
    }

    //必殺技が当たっていた場合のダメージ処理呼出し
    public override void PlaeyrExAttack_HitEnemyEnd(float power)
    {
        demonKing.PlaeyrExAttack_HitEnemyEnd(power);
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
        Damage(power, null, true, true);
    }

    //停止処理解除
    public override void Stop_End()
    {
        demonKing.Stop_End();
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
    }






    ////HPとFullHPの獲得
    //public float GetEnemyHP()
    //{
    //    return hp;
    //}
    //public float GetEnemyFullHP()
    //{
    //    return enemyData.hp;
    //}

    //public void OnColEnter(Collider2D col)
    //{
    //    OnColEnter2D(col);
    //}

    //public void OnColStay(Collider2D col)
    //{
    //    OnColStay2D(col);
    //}

    ////通常削除
    //public void EnemyNomalDestroy()
    //{
    //    SoundManager.Instance.PlaySE(SESoundData.SE.MonsterDead);
    //    GameObject obj = Instantiate(deathEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);

    //    if (_EnemyBuff) _EnemyBuff._Destroy();

    //    if (tween != null)
    //    {
    //        tween.Kill();
    //        //アニメーションが終了したら時間を戻す
    //        Time.timeScale = 1;
    //    }
    //    Destroy(gameObject);
    //}

    ////ヒットエフェクト生成
    //internal void HitEfect(Transform enemy, int angle)
    //{
    //    GameObject prefab =
    //    Instantiate(GameManager.Instance.hitEffect, new Vector2(enemy.position.x, enemy.position.y), Quaternion.identity);
    //    prefab.transform.Rotate(new Vector3(0, 0, angle));
    //    SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
    //    _EfectDestroy(prefab, 0.2f);
    //}
    ////エフェクト削除
    //void _EfectDestroy(GameObject prefab, float time)
    //{
    //    Destroy(prefab, time);
    //}
}
