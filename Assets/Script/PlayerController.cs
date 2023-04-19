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
    MoveData moveData = new MoveData { firstSpeed = 1f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

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

    //�f�o�b�N�v
    internal bool isAttack = false;
    bool isAttackKay = false;

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

        //�A�j���[�V�����̓K�p
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsLanding", isLanding);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsRun", isRun);

        //�f�o�b�N�pRay
        //Ray2D ray = new Ray2D(this.transform.position, new Vector2(1, 0));
        //Debug.DrawLine(ray.origin, ray.direction * 10, Color.red);
    }

    void FixedUpdate()
    {
        //�v���C���[�̍��E�̈ړ�
        rb.velocity = new Vector2(moveInput * speed * timer, rb.velocity.y);
        //rb.AddForce(new Vector2(moveInput * 10f, 0), ForceMode2D.Impulse);

        //�W�����v
        if (isjump)
        {
            Jump();
        }
        //�d��
        Gravity();
        //if (isAttack)
        //{
        //    rb.AddForce(transform.right * 10, ForceMode2D.Impulse);
        //}
        
    }

    //�ړ����̏����i�������j
    void Dash()
    {
        //�_�b�V������
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
        // �L�[���͎擾
        if (Input.GetButton("Jump") && !isJumping && !isFalling)
        {
            isSquatting = true;
            isjump = true;
        }

        //�W�����v���̏���
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


    }

    void Jump()
    {
        isJumping = true;
        isSquatting = false;
        rb.AddForce(transform.up * jumpData.firstSpeed, ForceMode2D.Impulse);
    }

    public void _Attack(Collider2D enemy)
    {
        Debug.Log("�ʏ�U��");
        //enemy.GetComponent<Enemy>().Damage();
    }

    //�Z���͌��m�E����
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

        //�؂�グ
        

        //�����؂�
        if ((((lsh >= 0.8 || lsh <= -0.8) && isAttackKay) || rsh >= 0.8 )&& !isAttack && !isFalling)
        {
            Iaikiri._Iaikiri(rb);
            StartCoroutine(_interval());
        }
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

    //�N�[���^�C���p�R���[�`��
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
}
