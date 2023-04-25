using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] internal Rigidbody2D rb;
    [SerializeField] internal Animator animator;

    [System.Serializable]
    internal struct MoveData
    {
        [Tooltip("初期速度")]
        public float firstSpeed;
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
        [Tooltip("初速")]
        public float firstSpeed;
        [Tooltip("重力加速度")]
        public float gravity;
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
    internal MoveData moveData = new MoveData { firstSpeed = 1f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("ジャンプステータス")]
    internal JumpData jumpData = new() { firstSpeed = 16f, gravity = 10f, maxJumpTime = 1f };

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

    //アニメーション用
    internal bool isFalling = false;
    internal bool isMoving = false;
    internal bool isJumping = false;
    internal bool isLanding = false;
    internal bool isSquatting = false;
    internal bool isUpAttack = false;
    internal bool isDropAttack = false;
    internal bool isSideAttack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hpparam = GameObject.Find("Hero").GetComponentInChildren<HPparam>();
    }

    void Update()
    {
        isFalling = rb.velocity.y < -0.5f;

        //Debug.Log("基礎攻撃力: 15"　+ "攻撃力は"+ (15 +15*combo.GetPowerUp()));

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

        _Skill();

        animator.SetBool("IsMoving", isMoving);
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
            StartCoroutine(_interval());
        }

        //落下攻撃攻撃
        if (((lsv <= -0.8 && isAttackKay) || rsv <= -0.8) && !isAttack &&(isFalling || isJumping))
        {
            DownAttack._DownAttack(this);
            StartCoroutine(_interval());
        }

        //横移動攻撃
        if (((lsh >= 0.8 && isAttackKay) || rsh >= 0.8)
            && !isAttack 
            && !isFalling && !isJumping)
        {
            sideJudge = true;
            StartCoroutine(SideAttack());
        }
        else if(((lsh <= -0.8 && isAttackKay) || rsh <= -0.8)
                && !isAttack
                && !isFalling & !isJumping)
        {
            sideJudge = false;
            StartCoroutine(SideAttack());
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
            rb.AddForce(new Vector2(knockBackDir.x * knockBackForce, knockBackDir.y * knockBackForce));
            Debug.Log(new Vector2(knockBackDir.x * knockBackForce, knockBackDir.y * knockBackForce));
        }
        else
        {
            //rb.velocity = Vector2.zero;
            isKnockingBack = false;
        }
    }
    //KnockBackされたら呼ぶ関数
    public void KnockBack(int damage, Vector3 position, float force)
    {
        if (knockBackCounter <= 0)
        {
            hpparam.SetHP(hpparam.GetHP() - damage);
            if (hpparam.GetHP() == 0)
            {
                //プレイヤーが死ぬ
            }
        }
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
        Debug.Log(localScale.x);
    }

    //クールタイム用コルーチン
    IEnumerator _interval()
    {
        float time = 2;

        isAttack = true;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        isAttack = false;
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
}
