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

    [SerializeField]
    [Header("移動ステータス")]
    MoveData moveData = new MoveData { firstSpeed = 1f, maxSpeed = 10f, accele = 0.03f };

    [SerializeField]
    [Header("ジャンプステータス")]
    JumpData jumpData = new JumpData { firstSpeed = 16f, gravity = 10f, maxJumpTime = 1f };

    float timer;
    float jumpTime = 0;
    private float speed;
    private float dashTime;
    [SerializeField]
    bool isjump = false;
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
    }


    // Update is called once per frame
    void Update()
    {
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

            if(moveInput < 0 && scale.x > 0 || moveInput >0 && scale.x < 0)
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

        // キー入力取得
        if (Input.GetKey(KeyCode.Space) && !isJumping && !isFalling)
        {
            isSquatting = true;
            isjump = true;
        }

        //ジャンプ中の処理
        if(isjump) 
        {
            if(Input.GetKeyUp(KeyCode.Space) || jumpTime >= jumpData.maxJumpTime)
            {
                isjump = false;

            }
            else if (Input.GetKey(KeyCode.Space))
            {
                jumpTime += Time.deltaTime;
            }
        }

        //ダッシュ加速
        if (isMoving && moveData.maxSpeed > speed)
        {
            dashTime+= Time.deltaTime;

            if (dashTime > moveData.acceleTime)
            {
                speed += moveData.accele;
                dashTime= 0;
            }
        }
        

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

        //プレイヤーの左右の移動
        rb.velocity = new Vector2(moveInput * speed * timer, rb.velocity.y);

        //ジャンプ
        if (isjump)
        {
            Jump();
        }
        //重力
        Gravity();
    }

    void Jump()
    {
        isJumping = true;
        isSquatting = false;
        rb.AddForce(transform.up * jumpData.firstSpeed, ForceMode2D.Impulse);
    }

    void Gravity()
    {
        rb.AddForce(new Vector2(0, -jumpData.gravity));
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
        }
    }

    void Landingoff()
    {
        isLanding = false;
    }
}
