using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] internal Rigidbody2D rb;
    [SerializeField] internal Animator animator;
    internal PlayerSE playerSE;
    Player_Jump jump;

    [SerializeField]
    GameObject RunEffect;

    [SerializeField]
    GameObject JumpEffect;

    [SerializeField]
    GameObject ExAttackHitEffect;

    [SerializeField]
    GameObject ExAttackLastEffect;

    [System.Serializable]
    public struct MoveData
    {
        [Tooltip("初期速度")]
        public float firstSpeed;
        [Tooltip("ジャンプ中移動速度")]
        public float jumpFirstSpeed;
        [Tooltip("ダッシュ変化速度")]
        public float dashSpeed;
        [Tooltip("最高速度")]
        public float maxSpeed;
        [Tooltip("加速度")]
        public float accele;
        [Tooltip("加速必要時間")]
        public float acceleTime;
    }

    [System.Serializable]
    internal struct JumpData
    {
        [Tooltip("速度")]
        public float speed;
        [Tooltip("落下速度")]
        public float gravity;
        [Tooltip("ジャンプ可能高さ")]
        public float jumpHeight;
        [Tooltip("2段目ジャンプ可能高さ")]
        public float secondJumpHeight;
        [Tooltip("ジャンプ時間の上限")]
        public float maxJumpTime;
    }

    [System.Serializable]
    struct KnockBackData
    {
        [Tooltip("KnockBackされる期間指定")]
        public float knockBackTime;
        [Tooltip("行動不能期間")]
        public float cantMovingTime;
        [Tooltip("KnockBack可能かどうか")]
        public bool canKnockBack;
        //[Tooltip("KnockBackされる速さ")]
        //public float knockBackForce;
        //[Tooltip("無敵時間")]
        //public float invincibiltyTime;
    }

    [SerializeField]
    [Header("プレイヤーステータス")]
    private int HP;

    [SerializeField]
    [Header("移動ステータス")]
    internal MoveData moveData = new MoveData { firstSpeed = 1f,dashSpeed = 3f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("ジャンプステータス")]
    internal JumpData jumpData = new() { speed = 16f, gravity = 10f,jumpHeight = 5f, maxJumpTime = 1f };

    [SerializeField]
    [Header("ノックバックステータス")]
    KnockBackData knockBack = new() { /*knockBackForce = 50,*/ knockBackTime = .4f, cantMovingTime = .4f, canKnockBack = true };

    //背景
    [SerializeField]
    internal ParallaxBackground parallaxBackground;

    //通常攻撃再使用確認
    bool canNomalAttack = true;

    //上昇攻撃多重発生防止用bool
    internal bool canUpAttack = true;

    //SideAttack関連
    const float dashingTime = 0.2f;
    bool canSideAttack = true;

    //KnockBack関連
    Vector2 knockBackDir;   //ノックバックされる方向
    bool isKnockingBack;    //ノックバックされているかどうか
    internal float knockBackCounter; //時間を測るカウンター
    internal float canMovingCounter;
    float knockBackForce;   //ノックバックされる力
    HPparam hpparam;

    //入力キー
    internal bool isAttack = false;
    bool isAttackKay = false;

    //横攻撃の左右判定(trueなら右）
    bool sideJudge;

    //必殺技時に取得する・保存する為のEnemyList[
    [SerializeField]
    internal List<GameObject> enemylist = new List<GameObject>();

    //ダメージカメラ処理
    [SerializeField]
    CameraShake shake;

    //アニメーション用
    internal bool isFalling = false;
    internal bool isMoving = false;
    internal bool isRun = false;
    internal bool isJumping = false;
    internal bool isLanding = false;
    internal bool isSquatting = false;
    internal bool isUpAttack = false;
    internal bool isDropAttack = false;
    internal bool isSideAttack = false;
    internal bool isExAttack = false;
    internal bool isWarpDoor = false;

    //boss判定用
    internal bool canMove = true;

    void Start()
    {
        playerSE = GetComponent<PlayerSE>();
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<Player_Jump>();
        hpparam = GameObject.Find("Hero").GetComponentInChildren<HPparam>();
    }

    void Update()
    {
        //デバック用シーンリセット
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (isExAttack || isWarpDoor)
        {
            rb.velocity = Vector2.zero;
        }

        //ノックバック処理
        if (knockBack.canKnockBack)
        {
            if (isKnockingBack)
            {
                KnockingBack();
                animator.SetBool("IsknockBack", isKnockingBack);
                return;
            }
        };

        if (canMovingCounter >= 0)
        {
            canMovingCounter -= Time.deltaTime;
        }

        if (canMove) _Skill();

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRun", isRun);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsLanding", isLanding);
    }

    public void _Attack(Collider2D enemy, float powar)
    {
        ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        ExAttackParam.Instance.AddGauge();
        enemy.GetComponent<Enemy>().Damage(powar + ComboParam.Instance.GetPowerUp());
    }

    public void _Heel(int resilience)
    {
        hpparam.SetHP(hpparam.GetHP() + resilience);
    }

    public void _Damage(int power)
    {

        hpparam.DamageHP(hpparam.GetHP() - power);
        shake.Shake(0.2f, 0.8f, true, true);
        if (hpparam.GetHP() <= 0)
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDead);
            SceneManager.LoadScene("FinishScene");
        }
    }

    //技入力検知・発生
    void _Skill()
    {
        float lsh = Input.GetAxis("L_Stick_H");
        float lsv = Input.GetAxis("L_Stick_V");
        float rsh = Input.GetAxis("R_Stick_H");
        float rsv = Input.GetAxis("R_Stick_V");

        float tri = Input.GetAxis("L_R_Trigger");


        if (tri > 0)
        {
            isAttackKay = true;
            
        }
        else { isAttackKay = false; }

        

        //上昇攻撃
        if (((lsv >= 0.8 && isAttackKay) || rsv >= 0.8) 
            && !isAttack && canUpAttack)
        {
            canUpAttack = false;
            jump.UpAttack();
            isAttack = true;
        }

        //落下攻撃攻撃
        if (((lsv <= -0.8 && isAttackKay) || rsv <= -0.8)
            && !isAttack && (isFalling || isJumping) && !isDropAttack)
        {
            DownAttack._DownAttack(this);
            isAttack = true;
        }

        //横移動攻撃
        if (((lsh >= 0.8 && isAttackKay) || rsh >= 0.8)
            && !isAttack && !isSideAttack && canSideAttack)
        {
            canSideAttack = false;
            sideJudge = true;
            StartCoroutine(SideAttack());
        }
        else if(((lsh <= -0.8 && isAttackKay) || rsh <= -0.8)
                && !isAttack && !isSideAttack && canSideAttack)
        {
            canSideAttack = false;
            sideJudge = false;
            StartCoroutine(SideAttack());
        }

        //必殺技
        if (Input.GetKey("joystick button 4") && Input.GetKey("joystick button 5") || Input.GetKeyDown(KeyCode.L))
        {
            if (ExAttackParam.Instance.GetIsExAttack) 
            {
                isExAttack = true;
                animator.SetBool("IsExAttack", isExAttack);
                ExAttackParam.Instance._EXAttack();
                GameManager.Instance.PlayerExAttack_Start();
                ExAttackCutIn.Instance.StartCoroutine("_ExAttackCutIn", this.GetComponent<PlayerController>());
            }
        }

        //手動攻撃：攻撃ボタンが押されせたとき
        if (Input.GetKeyDown("joystick button 2") && canNomalAttack)
        {
            canNomalAttack = false;
            animator.SetTrigger("IsNomalAttack");
            var skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
            StartCoroutine(_interval(skill.coolTime));
        }

        if (Input.GetKey("joystick button 2") && canNomalAttack || Input.GetKey(KeyCode.Mouse0) && canNomalAttack)
        {
            canNomalAttack = false;
            animator.SetTrigger("IsNomalAttack");
            var skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
            StartCoroutine(_interval(skill.coolTime));
        }
    }

    //KnockBackされたときの処理
    void KnockingBack()
    {
        
        if (knockBackCounter == knockBack.knockBackTime)
        {
            rb.velocity = Vector2.zero;
            isAttack = false;
        }
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;
            //簡単に説明すると下は上下に飛ばさないコードです。
            rb.AddForce(new Vector2((knockBackDir.y * knockBackForce > 5 || knockBackDir.y * knockBackForce < -5) ? ((knockBackDir.x < 0) ? -Mathf.Abs(knockBackDir.y * knockBackForce) : Math.Abs(knockBackDir.y * knockBackForce)) : knockBackDir.x * knockBackForce, ((knockBackDir.y * knockBackForce> 5 || knockBackDir.y * knockBackForce < -5)? knockBackDir.y : knockBackDir.y * knockBackForce)));//横だけ飛ばされるコード      簡単に説明すると上と下は５よりでかくなると飛ばされない、左右に関して上下が５以上になると百パーセント横から触ったということじゃないのが分かるので、上下の飛ばす力で左右の方向を与えて飛ばさせる。
        }
        else
        {
            isKnockingBack = false;
        }
    }
    //KnockBackされたら呼ぶ関数
    public void KnockBack(Vector3 position, float force)
    {
        canMovingCounter = knockBack.cantMovingTime;
        knockBackCounter = knockBack.knockBackTime;
        isKnockingBack = true;

        knockBackDir = transform.position - position;
        knockBackDir.Normalize();
        knockBackForce = force;
    }

    //上昇攻撃でステージにぶつかった時用
    private void OnTriggerEnter2D(Collider2D collision)
    {  
        isUpAttack = false;
    }

    //横攻撃左右判定
    private void Side(Skill skill)
    {
        Vector3 localScale = transform.localScale;
        if (transform.localScale.x > 0)
        {
            if (sideJudge)
            {
                rb.velocity = new Vector2(transform.localScale.x * skill.distance, 0f);
                StartScroll();
            }
            else if (!sideJudge)
            {
                rb.velocity = new Vector2(-transform.localScale.x * skill.distance, 0f);
                localScale.x *= -1f;
                transform.localScale = localScale;
                StartScroll();
            }
        }
        else if (transform.localScale.x < 0)
        {
            if (sideJudge)
            {
                rb.velocity = new Vector2(-transform.localScale.x * skill.distance, 0f);
                localScale.x *= -1f;
                transform.localScale = localScale;
                StartScroll();
            }
            else if (!sideJudge)
            {
                rb.velocity = new Vector2(transform.localScale.x * skill.distance, 0f);
                StartScroll();
            }
        }
        
    }

    //横攻撃処理
    private IEnumerator SideAttack()
    {
        isAttack = true;
        isSideAttack = true;
        animator.SetBool("IsSideAttack", isSideAttack);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.SideAttack);
        Side(skill);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isSideAttack = false;
        animator.SetBool("IsSideAttack", isSideAttack);
        enemylist.Clear();
        isAttack = false;
        yield return new WaitForSeconds(skill.coolTime);
        canSideAttack = true;
    }

    //必殺技
    public void ExAttackStart()
    {
        Debug.Log("必殺技実行");
        animator.SetTrigger("ExAttack");
    }

    //ヒット時（アニメーションから呼ぶ）
    public void _ExAttackHitEffect()
    {
        //エフェクト生成
        foreach (var enemy in enemylist)
        {
            _HitEfect(enemy.transform, UnityEngine.Random.Range(0, 360));
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        }
    }

    public void _ExAttackHitEnemyDamage()
    {
        //ダメージ処理
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.ExAttack); ;
        GameManager.Instance.PlayerExAttack_HitEnemyEnd(enemylist ,skill.damage);
        foreach (var enemy in enemylist)
        {
            _HitEfect(enemy.transform, UnityEngine.Random.Range(0, 360));
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        }
    }

    public void ExAttackHitCheck()
    {
        if(enemylist.Count == 0)
        {
            ExAttackEnd();
        }
    }

    //必殺技で使用したEnemyをリセット
    public void ExAttackEnd()
    {
        isExAttack = false;
        enemylist.Clear();
        animator.SetBool("IsExAttack", isExAttack);
        GameManager.Instance.PlayerExAttack_End();
    }

    //スキルアクション中無敵に使用するメソッド
    public void SkillActionPlayer()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerAction");
    }
    //無敵をはがすためのメソッド
    public void NomalPlayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    //スキルアクション終了メソッド
    public void SkillAttackEnd()
    {
        isAttack = false;
        enemylist.Clear();
    }

    //砂埃エフェクト生成
    public void _RunEffect()
    {
        GameObject prefab = 
        Instantiate(RunEffect, new Vector2(this.transform.position.x, this.transform.position.y - 0.8f), Quaternion.identity);
        if(gameObject.transform.localScale.x < 0)
        {
            Vector2 scale = prefab.transform.localScale;
            scale.x *= -1f;
            prefab.transform.localScale = scale;
        }
        _EfectDestroy(prefab, 0.3f);
    }

    //ダブルジャンプエフェクト生成
    public void _JumpEffect()
    {
        GameObject prefab =
        Instantiate(JumpEffect, new Vector2(this.transform.position.x, this.transform.position.y - 0.8f), Quaternion.identity);
        if (gameObject.transform.localScale.x < 0)
        {
            Vector2 scale = prefab.transform.localScale;
            scale.x *= -1f;
            prefab.transform.localScale = scale;
        }
        _EfectDestroy(prefab, 0.2f);
    }

    internal void _HitEfect(Transform enemy, int angle)
    {
        GameObject prefab =
        Instantiate(ExAttackHitEffect, new Vector2(enemy.position.x, enemy.position.y), Quaternion.identity);
        prefab.transform.Rotate(new Vector3(0,0,angle));
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
        _EfectDestroy(prefab, 0.2f);
    }

    public void _ExAttackLastEffect()
    {
        GameObject prefab;
        if (gameObject.transform.localScale.x < 0)
        {
            prefab =
            Instantiate(ExAttackLastEffect, new Vector2(this.transform.position.x - 4f, this.transform.position.y), Quaternion.identity);
        }
        else
        {
            prefab =
            Instantiate(ExAttackLastEffect, new Vector2(this.transform.position.x + 4f, this.transform.position.y), Quaternion.identity);
        }
        _EfectDestroy(prefab, 0.3f);
    }

    void _EfectDestroy(GameObject prefab, float time)
    {
        Destroy(prefab, time);
    }

    public void SetCanMove(bool cM)
    {
        canMove = cM;
    }

    internal void WarpDoor(Transform door)
    {
        isWarpDoor = true;
        var doorPosX = door.position.x;
        var doorPosY = door.position.y;
        this.transform.position = new Vector3(doorPosX, doorPosY, transform.position.z);
        animator.SetBool("IsWarpDoor", isWarpDoor);
        animator.SetTrigger("WarpDoor");
    }

    internal void WarpDoorEnd()
    {
        isWarpDoor = false;
        animator.SetBool("IsWarpDoor", isWarpDoor);
    }

    internal void GoolDoor(Transform door)
    {
        isWarpDoor = true;
        var doorPosX = door.position.x;
        var doorPosY = door.position.y;
        this.transform.position = new Vector3(doorPosX, doorPosY, transform.position.z);
        animator.SetTrigger("GoolDoor");
        SoundManager.Instance.PlaySE(SESoundData.SE.GoalSE);
    }

    //背景スクロール処理
    private void StartScroll()
    {
        if (parallaxBackground != null)
        {
            parallaxBackground.StartScroll(this.transform.position);
        }
    }

    //クールタイム用コルーチン
    IEnumerator _interval(float coolTime)
    {
        
        float time = coolTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        enemylist.Clear();
        canNomalAttack = true;
    }
}
