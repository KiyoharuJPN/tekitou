using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] internal Rigidbody2D rb;
    [SerializeField] internal Animator animator;
    internal PlayerSE playerSE;

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

    //最終攻撃力格納用変数
    float _power;

    //SideAttack関連
    Skill sideAttack;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

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
        hpparam = GameObject.Find("Hero").GetComponentInChildren<HPparam>();
    }

    void Update()
    {
        if (isExAttack)
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
        enemy.GetComponent<Enemy>().Damage(powar + ComboParam.Instance.GetPowerUp());
    }

    public void _Heel(int resilience)
    {
        hpparam.SetHP(hpparam.GetHP() + resilience);
    }

    public void _Damage(int power)
    {
        hpparam.DamageHP(hpparam.GetHP() - power);
        if (hpparam.GetHP() <= 0)
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDead);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        if (((lsv >= 0.8 && isAttackKay) || rsv >= 0.8) && !isAttack)
        {
            UpAttack._UpAttack(this);
            isAttack = true;
        }

        //落下攻撃攻撃
        if (((lsv <= -0.8 && isAttackKay) || rsv <= -0.8) && !isAttack &&(isFalling || isJumping))
        {
            DownAttack._DownAttack(this);
            isAttack = true;
        }

        //横移動攻撃
        if (((lsh >= 0.8 && isAttackKay) || rsh >= 0.8)
            && !isAttack )
        {
            sideJudge = true;
            StartCoroutine(SideAttack());
        }
        else if(((lsh <= -0.8 && isAttackKay) || rsh <= -0.8)
                && !isAttack)
        {
            sideJudge = false;
            StartCoroutine(SideAttack());
        }

        //必殺技
        if (Input.GetKey("joystick button 4") && Input.GetKey("joystick button 5"))
        {
            if (ExAttackParam.Instance.GetIsExAttack) 
            {
                isExAttack = true;
                animator.SetBool("IsExAttack", isExAttack);
                ExAttackParam.Instance._EXAttack();
                ExAttackCutIn.Instance.StartCoroutine("_ExAttackCutIn", this.GetComponent<PlayerController>());
            }
        }

    }

    //KnockBackされたときの処理
    void KnockingBack()
    {
        
        if (knockBackCounter == knockBack.knockBackTime)
        {
            rb.velocity = Vector2.zero;
        }
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;
            //簡単に説明すると下は上下に飛ばさないコードです。
            rb.AddForce(new Vector2((knockBackDir.y * knockBackForce > 5 || knockBackDir.y * knockBackForce < -5) ? ((knockBackDir.x < 0) ? -Mathf.Abs(knockBackDir.y * knockBackForce) : Math.Abs(knockBackDir.y * knockBackForce)) : knockBackDir.x * knockBackForce, ((knockBackDir.y * knockBackForce> 5 || knockBackDir.y * knockBackForce < -5)? knockBackDir.y : knockBackDir.y * knockBackForce)));//横だけ飛ばされるコード      簡単に説明すると上と下は５よりでかくなると飛ばされない、左右に関して上下が５以上になると百パーセント横から触ったということじゃないのが分かるので、上下の飛ばす力で左右の方向を与えて飛ばさせる。                                                        //(knockBackDir.y * knockBackForce > 7 || knockBackDir.y * knockBackForce < -7) ? knockBackDir.y * knockBackForce : knockBackDir.x * knockBackForce, (knockBackDir.y * knockBackForce > 3 || knockBackDir.y * knockBackForce < -3) ? knockBackDir.y : knockBackDir.y * knockBackForce)
            Debug.Log(new Vector2(knockBackDir.x * knockBackForce, knockBackDir.y * knockBackForce));
        }
        else
        {
            //rb.velocity = Vector2.zero;
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

    public void AnimationBoolReset()
    {
        isFalling = false;
        isMoving = false;
        isJumping = false;
        isLanding = false;
        isSquatting = false;
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
        yield return new WaitForSeconds(dashingCooldown);
        isAttack = false;
    }

    //横攻撃
    private void Side(Skill skill)
    {
        Vector3 localScale = transform.localScale;
        if (transform.localScale.x > 0)
        {
            if (sideJudge)
            {
                rb.velocity = new Vector2(transform.localScale.x * skill.distance, 0f);
            }
            else if (!sideJudge)
            {
                rb.velocity = new Vector2(-transform.localScale.x * skill.distance, 0f);
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
        else if(transform.localScale.x < 0)
        {
            if (sideJudge)
            {
                rb.velocity = new Vector2(-transform.localScale.x * skill.distance, 0f);
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            else if (!sideJudge)
            {
                rb.velocity = new Vector2(transform.localScale.x * skill.distance, 0f);
            }
        }
    }

    //必殺技
    public void ExAttackStart()
    {
        Debug.Log("必殺技実行");
        animator.SetTrigger("ExAttack");
    }

    //ヒット時（アニメーションから呼ぶ）
    public void _ExAttackHit()
    {
        //エフェクト生成
        foreach (var enemy in enemylist)
        {
            _HitEfect(enemy);
        }
        //ダメージ処理（現在では実装未定）

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

    private void _HitEfect(GameObject enemy)
    {
        GameObject prefab =
        Instantiate(ExAttackHitEffect, new Vector2(enemy.transform.position.x, enemy.transform.position.y), Quaternion.identity);
        var angle = UnityEngine.Random.Range(0, 360);
        prefab.transform.Rotate(new Vector3(0,0,angle));
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
        ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
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

    internal void WarpDoor()
    {
        isWarpDoor = true;
        animator.SetBool("IsWarpDoor", isWarpDoor);
        animator.SetTrigger("WarpDoor");
    }

    internal void WarpDoorEnd()
    {
        isWarpDoor = false;
        animator.SetBool("IsWarpDoor", isWarpDoor);
    }
}
