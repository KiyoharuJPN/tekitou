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

    struct KnockBackData
    {
        [Tooltip("KnockBack�������Ԏw��")]
        public float knockBackTime;
        //[Tooltip("KnockBack����鑬��")]
        //public float knockBackForce;
        [Tooltip("KnockBack�\���ǂ���")]
        public bool canKnockBack;
        //[Tooltip("���G����")]
        //public float invincibiltyTime;
    }


    [SerializeField]
    [Header("�ړ��X�e�[�^�X")]
    MoveData moveData = new MoveData { firstSpeed = 1f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("�W�����v�X�e�[�^�X")]
    JumpData jumpData = new JumpData { firstSpeed = 16f, gravity = 10f, maxJumpTime = 1f };

    [SerializeField]
    [Header("�m�b�N�o�b�N�X�e�[�^�X")]
    KnockBackData knockBack = new() { /*knockBackForce = 50, */knockBackTime = 0.3f, canKnockBack = true };
    //KnockBack�֘A
    Vector2 knockBackDir;   //�m�b�N�o�b�N��������
    bool isKnockingBack;    //�m�b�N�o�b�N����Ă��邩�ǂ���
    float knockBackCounter; //���Ԃ𑪂�J�E���^�[
    float knockBackForce;   //�m�b�N�o�b�N������
    HPparam hpparam;
    PointParam point;
    ComboParam combo;

    float timer;
    float jumpTime = 0;
    private float speed;
    private float dashTime;
    [SerializeField]
    bool isjump = false;
    bool canSecondJump = false;
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

        hpparam = GameObject.Find("Hero").GetComponentInChildren<HPparam>();
        point = GameObject.Find("Hero").GetComponentInChildren<PointParam>();
        combo = GameObject.Find("Hero").GetComponentInChildren<ComboParam>();
    }

    void Update()
    {

        /*if (Input.GetKeyDown("1"))
        {
            point.SetPoint(point.GetPoint() + 1);
        }

        if (Input.GetKeyDown("2"))
        {
            point.SetPoint(point.GetPoint() - 1);
        }

        if (Input.GetKeyDown("3"))
        {
            combo.SetCombo(combo.GetCombo() + 1);
        }

        if (Input.GetKeyDown("4"))
        {
            combo.SetCombo(combo.GetCombo() - 1);
        }

        Debug.Log("��b�U����: 15"�@+ "�U���͂�"+ (15 +15*combo.GetPowerUp()));
*/
        //�m�b�N�o�b�N����
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
        if (knockBackCounter <= 0)
        {
            //�v���C���[�̍��E�̈ړ�
            rb.velocity = new Vector2(moveInput * speed * timer, rb.velocity.y);
            //rb.AddForce(new Vector2(moveInput * 10f, 0), ForceMode2D.Impulse);
        }

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
            canSecondJump = true;
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

        //��i�W�����v
        if (canSecondJump)
        {
            if (Input.GetButtonDown("Jump"))
            {
                canSecondJump = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                jumpTime = 0;
                isjump = true;
            }

        }


    }

    void Jump()
    {
        isJumping = true;
        isSquatting = false;
        if (canSecondJump)
        {
            rb.AddForce(transform.up * jumpData.firstSpeed, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(transform.up * ((jumpData.firstSpeed / 5) * 4), ForceMode2D.Impulse);
        }
    }

    public void _Attack(Collider2D enemy)
    {
        enemy.GetComponent<Enemykari>().Damage();
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
        if (((lsv >= 0.8 && isAttackKay) || rsv >= 0.8) && !isAttack)
        {
            UpAttack._UpAttack(rb);
            StartCoroutine(_interval());
        }

        //�˂��h��
        if (((lsv <= -0.8 && isAttackKay) || rsv <= -0.8) && !isAttack && isFalling)
        {
            Stabbing._Stabbing(rb);
            StartCoroutine(_interval());
        }

        //�����؂�
        if ((((lsh >= 0.8 || lsh <= -0.8) && isAttackKay) || (rsh >= 0.8 || rsh <= -0.8 ))&& !isAttack && !isFalling)
        {
            Iaikiri._Iaikiri(rb);
            StartCoroutine(_interval());
        }
    }

    void Gravity()
    {
        if (knockBackCounter <= 0)
        {
            rb.AddForce(new Vector2(0, -jumpData.gravity));
        }
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
            rb.velocity = Vector2.zero;
            jumpTime = 0;
            canSecondJump = false;
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

    //KnockBack���ꂽ�Ƃ��̏���
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
    //KnockBack���ꂽ��ĂԊ֐�
    public void KnockBack(int damage, Vector3 position, float force)
    {
        if (knockBackCounter <= 0)
        {
            hpparam.SetHP(hpparam.GetHP() - damage);
            if (hpparam.GetHP() == 0)
            {
                //�v���C���[������
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
