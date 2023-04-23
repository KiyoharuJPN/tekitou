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
    struct StatusData
    {
        [Tooltip("体力")]
        public float hp;
        [Tooltip("攻撃力")]
        public float powar;
    }

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

    struct KnockBackData
    {
        [Tooltip("KnockBackされる期間指定")]
        public float knockBackTime;
        //[Tooltip("KnockBackされる速さ")]
        //public float knockBackForce;
        [Tooltip("KnockBack可能かどうか")]
        public bool canKnockBack;
        //[Tooltip("無敵時間")]
        //public float invincibiltyTime;
    }

    [SerializeField]
    [Header("プレイヤーステータス")]
    StatusData statusData = new StatusData { hp = 6, powar = 1};

    [SerializeField]
    [Header("移動ステータス")]
    internal MoveData moveData = new MoveData { firstSpeed = 1f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("ジャンプステータス")]
    internal JumpData jumpData = new JumpData { firstSpeed = 16f, gravity = 10f, maxJumpTime = 1f };

    [SerializeField]
    [Header("ノックバックステータス")]
    KnockBackData knockBack = new() { /*knockBackForce = 50, */knockBackTime = 0.3f, canKnockBack = true };

    //KnockBack関連
    Vector2 knockBackDir;   //ノックバックされる方向
    bool isKnockingBack;    //ノックバックされているかどうか
    internal float knockBackCounter; //時間を測るカウンター
    float knockBackForce;   //ノックバックされる力
    HPparam hpparam;

    //デバック要
    internal bool isAttack = false;
    bool isAttackKay = false;

    //アニメーション用
    internal bool isFalling = false;
    internal bool isMoving = false;
    internal bool isJumping = false;
    internal bool isLanding = false;
    internal bool isSquatting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hpparam = GameObject.Find("Hero").GetComponentInChildren<HPparam>();
    }

    void Update()
    {
        isFalling = rb.velocity.y < -0.5f;

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
        

        Skill();

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsLanding", isLanding);
    }

    public void _Attack(Collider2D enemy)
    {
        animator.SetTrigger("IsNomalAttack");
        enemy.GetComponent<Enemy>().Damage(statusData.powar);
    }

    public void _Heel(int resilience)
    {
        hpparam.SetHP(hpparam.GetHP() + resilience);
    }

    //技入力検知・発生
    void Skill()
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

        //切り上げ
        if (((lsv >= 0.8 && isAttackKay) || rsv >= 0.8) && !isAttack)
        {
            UpAttack._UpAttack(rb);
            StartCoroutine(_interval());
        }

        //突き刺し
        if (((lsv <= -0.8 && isAttackKay) || rsv <= -0.8) && !isAttack && isFalling)
        {
            Stabbing._Stabbing(rb);
            StartCoroutine(_interval());
        }

        //居合切り
        if ((((lsh >= 0.8 || lsh <= -0.8) && isAttackKay) || (rsh >= 0.8 || rsh <= -0.8 ))&& !isAttack && !isFalling)
        {
            Iaikiri._Iaikiri(rb);
            StartCoroutine(_interval());
        }
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

        knockBackCounter = knockBack.knockBackTime;
        isKnockingBack = true;

        knockBackDir = transform.position - position;
        knockBackDir.Normalize();
        knockBackForce = force;
    }

    public void AnimationBoolReset()
    {
        isFalling = false;
        isMoving = false;
        isJumping = false;
        isLanding = false;
        isSquatting = false;
    }
}
