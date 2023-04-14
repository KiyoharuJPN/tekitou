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
        [Tooltip("�������x")]
        public float firstSpeed;
        [Tooltip("�ō����x")]
        public float maxSpeed;
        [Tooltip("�����x")]
        public float accele;
        [Tooltip("�����K�v����")]
        public float acceleTime;
    }

    [System.Serializable]
    struct JumpData
    {
        [Tooltip("����")]
        public float firstSpeed; 
        [Tooltip("�d�͉����x")]
        public float gravity;
        [Tooltip("�W�����v���Ԃ̏��")]
        public float maxJumpTime;
    }

    [SerializeField]
    [Header("�ړ��X�e�[�^�X")]
    MoveData moveData = new MoveData { firstSpeed = 1f, maxSpeed = 10f, accele = 0.03f };

    [SerializeField]
    [Header("�W�����v�X�e�[�^�X")]
    JumpData jumpData = new JumpData { firstSpeed = 16f, gravity = 10f, maxJumpTime = 1f };

    float timer;
    float jumpTime = 0;
    private float speed;
    private float dashTime;
    [SerializeField]
    bool isjump = false;
    float moveInput; //���͒l

    //�A�j���[�V�����p
    private bool isMoving = false;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isLanding = false;
    private bool isSquatting = false;
    private bool isRun = false;

    enum Status //�v���C���[�̏��
    {
        GROUND = 1, //�ڒn
        UP = 2,�@�@ //�㏸��
        DOWN = 3�@�@//���~��
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


        //�ړ������ɍ��킹�ĉ摜�̔��]
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

        // �L�[���͎擾
        if (Input.GetKey(KeyCode.Space) && !isJumping && !isFalling)
        {
            isSquatting = true;
            isjump = true;
        }

        //�W�����v���̏���
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

        //�_�b�V������
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

        //�A�j���[�V�����̓K�p
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsLanding", isLanding);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsRun", isRun);
    }

    void FixedUpdate()
    {

        //�v���C���[�̍��E�̈ړ�
        rb.velocity = new Vector2(moveInput * speed * timer, rb.velocity.y);

        //�W�����v
        if (isjump)
        {
            Jump();
        }
        //�d��
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

    //���n�̔���
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
