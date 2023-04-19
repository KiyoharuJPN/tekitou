using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [System.Serializable]
    struct MoveData
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
    struct JumpData
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
    [Header("移動ステータス")]
    MoveData moveData = new MoveData { firstSpeed = 1f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("ジャンプステータス")]
    JumpData jumpData = new JumpData { firstSpeed = 16f, gravity = 10f, maxJumpTime = 1f };

    [SerializeField]
    [Header("ノックバックステータス")]
    KnockBackData knockBack = new() { /*knockBackForce = 50, */knockBackTime = 0.3f, canKnockBack = true };
    //KnockBack関連
    Vector2 knockBackDir;   //ノックバックされる方向
    bool isKnockingBack;    //ノックバックされているかどうか
    float knockBackCounter; //時間を測るカウンター
    float knockBackForce;   //ノックバックされる力
    HPparam hpparam;

    float timer;
    float jumpTime = 0;
    private float speed;
    private float dashTime;
    [SerializeField]
    bool isjump = false;
    bool canSecondJump = false;
    float moveInput; //入力値

    //アニメーション用
    private bool isMoving = false;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isLanding = false;
    private bool isSquatting = false;
    private bool isRun = false;
    enum Status //プレイヤーの状態
    {
        GROUND = 1, //接地
        UP = 2,　　 //上昇中
        DOWN = 3　　//下降中
    }

    [SerializeField]
    Status playerStatus;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = moveData.firstSpeed;

        hpparam = GameObject.Find("Hero").GetComponentInChildren<HPparam>();
    }

    void Update()
    {

        //ノックバック処理
        if (knockBack.canKnockBack) 
        {
            if (isKnockingBack)
            {
                KnockingBack();
                return;
            }
        };


        moveInput = Input.GetAxis("Horizontal");

        isMoving = moveInput != 0;
        isFalling = rb.velocity.y < -0.5f;

        if (moveInput > 0 && moveInput < 0)
        {
            dashTime += Time.deltaTime;
        }


        //移動方向に合わせて画像の反転
        if (isMoving)
        {
            Vector3 scale = gameObject.transform.localScale;

            if (moveInput < 0 && scale.x > 0 || moveInput > 0 && scale.x < 0)
            {
                scale.x *= -1;
            }

            gameObject.transform.localScale = scale;

            timer += Time.deltaTime;
        }
        else
        {
            timer = moveData.firstSpeed;
        }

        Dash();
        JumpBottan();
        Skill();

        if (!isMoving)
        {
            isRun = false;
            dashTime = 0;
            speed = moveData.firstSpeed;
        }

        //アニメーションの適用
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsLanding", isLanding);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsRun", isRun);
    }

    void FixedUpdate()
    {
        if(knockBackCounter <= 0)
        {
            //プレイヤーの左右の移動
            rb.velocity = new Vector2(moveInput * speed * timer, rb.velocity.y);
        }
        //ジャンプ
        if (isjump)
        {
            Jump();
        }
        //重力
        Gravity();
    }

    //移動中の処理（加速等）
    void Dash()
    {
        //ダッシュ加速
        if (isMoving && moveData.maxSpeed > speed)
        {
            dashTime += Time.deltaTime;

            if (dashTime > moveData.acceleTime)
            {
                speed += moveData.accele;
                dashTime = 0;
            }
        }
    }

    void JumpBottan()
    {
        // キー入力取得
        if (Input.GetButton("Jump") && !isJumping && !isFalling)
        {
            isSquatting = true;
            isjump = true;
            canSecondJump = true;
        }

        //ジャンプ中の処理
        if (isjump)
        {
            if (!Input.GetButton("Jump") || jumpTime >= jumpData.maxJumpTime)
            {
                isjump = false;
            }
            else if (Input.GetButton("Jump"))
            {
                jumpTime += Time.deltaTime;
            }
        }

        //二段ジャンプ
        if (canSecondJump)
        {
            if(Input.GetButtonDown("Jump"))
            {
                canSecondJump = false;
                jumpTime = 0;
                isjump = true;
            }

        }


    }

    void Jump()
    {
        isJumping = true;
        isSquatting = false;
        rb.AddForce(transform.up * jumpData.firstSpeed, ForceMode2D.Impulse);
    }

    //技入力検知・発生
    void Skill()
    {
        //TODO　突き刺し(RTボタンコード不明な為テスト用）
        if (Input.GetKeyDown("joystick button 6"))
        {
            Stabbing._Stabbing(rb);
        }
        if (Input.GetKeyDown("joystick button 5"))
        {
            RoundingUp._RoundingUp(rb);
        }
        if (Input.GetKeyDown("joystick button 5"))
        {
            IaiCut._IaiCut(rb);
        }
    }

    void Gravity()
    {
        if(knockBackCounter <= 0)
        {
            rb.AddForce(new Vector2(0, -jumpData.gravity));
        }
    }

    //着地の判定
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
        {
            if (isFalling == true)
            {
                isLanding = true;
            }
            isJumping = false;
            Invoke("Landingoff", 0.1f);
            jumpTime = 0;
            canSecondJump = false;
        }
    }

    void Landingoff()
    {
        isLanding = false;
    }


    //KnockBackされたときの処理
    void KnockingBack()
    {
        if(knockBackCounter == knockBack.knockBackTime)
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
        if(knockBackCounter <= 0)
        {
            hpparam.SetHP(hpparam.GetHP() - damage);
            if(hpparam.GetHP() == 0)
            {
                //プレイヤーが死ぬ
            }
        }

        knockBackCounter = knockBack.knockBackTime;
        isKnockingBack = true;

        knockBackDir = transform.position - position;
        knockBackDir.Normalize();
        knockBackForce = force;

        Debug.Log("emypos: " + position + "\nplayerpos: " + gameObject.transform.position + "\nknockDir: " + knockBackDir);
    }


    
}
