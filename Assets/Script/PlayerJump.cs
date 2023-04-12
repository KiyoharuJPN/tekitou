using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [SerializeField] private float moveSpeed;   // 初期速度
    [SerializeField] private float maxSpeed;    // 最高速度
    [SerializeField] private int jumpForce;

    float speed;    // 記憶用

    private bool isMoving = false;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isLanding = false;
    private bool isSquatting = false;
    private bool isRun = false;

    float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        speed = moveSpeed;  // 代入
        timer = speed;
    }


    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        isMoving = horizontal != 0;
        isFalling = rb.velocity.y < -0.2f;


        //移動方向に合わせて画像の反転
        if (isMoving)
        {
            Vector3 scale = gameObject.transform.localScale;

            if (horizontal < 0 && scale.x > 0 || horizontal > 0 && scale.x < 0)
            {
                scale.x *= -1;
            }

            gameObject.transform.localScale = scale;

            timer += Time.deltaTime;
        }
        else
        {
            timer = speed;
        }


        //ジャンプの動作
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && !isFalling)
        {
            isSquatting = true;
            Invoke("Jump", 0.1f);
        }


        // 移動速度の加速
        /*if (!rb.IsSleeping())
        {
            if (moveSpeed < maxSpeed)
            {
                moveSpeed += 0.015f;
            }
            else
            {
                moveSpeed = maxSpeed;
            }


            if (moveSpeed >= 5)
            {
                isRun = true;
            }
        }
        else
        {
            if (moveSpeed > speed)
            {
                moveSpeed -= speed;
            }
            else
            {
                moveSpeed = speed;
            }
        }*/



        if (!isMoving)
        {
            isRun = false;
        }

        Debug.Log(rb.velocity);

        //プレイヤーの左右の移動
        rb.velocity = new Vector2(horizontal * moveSpeed * timer, rb.velocity.y);


        //アニメーションの適用
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsLanding", isLanding);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsRun", isRun);
    }


    // ジャンプによる上昇
    void Jump()
    {
        isJumping = true;
        isSquatting = false;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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

        }
    }

    void Landingoff()
    {
        isLanding = false;
    }
}
