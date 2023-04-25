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
    internal struct JumpData
    {
        [Tooltip("����")]
        public float firstSpeed;
        [Tooltip("�d�͉����x")]
        public float gravity;
        [Tooltip("�W�����v���Ԃ̏��")]
        public float maxJumpTime;
    }

    [System.Serializable]
    struct KnockBackData
    {
        [Tooltip("KnockBack�������Ԏw��")]
        public float knockBackTime;
        [Tooltip("�s���s�\����")]
        public float cantMovingTime;
        [Tooltip("KnockBack�\���ǂ���")]
        public bool canKnockBack;
        //[Tooltip("KnockBack����鑬��")]
        //public float knockBackForce;
        //[Tooltip("���G����")]
        //public float invincibiltyTime;
    }

    [SerializeField]
    [Header("�v���C���[�X�e�[�^�X")]
    private int HP;

    [SerializeField]
    [Header("�ړ��X�e�[�^�X")]
    internal MoveData moveData = new MoveData { firstSpeed = 1f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("�W�����v�X�e�[�^�X")]
    internal JumpData jumpData = new() { firstSpeed = 16f, gravity = 10f, maxJumpTime = 1f };

    [SerializeField]
    [Header("�m�b�N�o�b�N�X�e�[�^�X")]
    KnockBackData knockBack = new() { /*knockBackForce = 50,*/ knockBackTime = .4f, cantMovingTime = .4f, canKnockBack = true };

    //�ŏI�U���͊i�[�p�ϐ�
    float _power;

    //SideAttack�֘A
    Skill sideAttack;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    //KnockBack�֘A
    Vector2 knockBackDir;   //�m�b�N�o�b�N��������
    bool isKnockingBack;    //�m�b�N�o�b�N����Ă��邩�ǂ���
    internal float knockBackCounter; //���Ԃ𑪂�J�E���^�[
    internal float canMovingCounter;
    float knockBackForce;   //�m�b�N�o�b�N������
    HPparam hpparam;

    //���̓L�[
    internal bool isAttack = false;
    bool isAttackKay = false;
    //���U���̍��E����(true�Ȃ�E�j
    bool sideJudge;

    //�A�j���[�V�����p
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

        //Debug.Log("��b�U����: 15"�@+ "�U���͂�"+ (15 +15*combo.GetPowerUp()));

        //�m�b�N�o�b�N����
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

    //�Z���͌��m�E����
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

        //�㏸�U��
        if (((lsv >= 0.8 && isAttackKay) || rsv >= 0.8) && !isAttack)
        {
            UpAttack._UpAttack(this);
            StartCoroutine(_interval());
        }

        //�����U���U��
        if (((lsv <= -0.8 && isAttackKay) || rsv <= -0.8) && !isAttack &&(isFalling || isJumping))
        {
            DownAttack._DownAttack(this);
            StartCoroutine(_interval());
        }

        //���ړ��U��
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
        canMovingCounter = knockBack.cantMovingTime;
        knockBackCounter = knockBack.knockBackTime;
        isKnockingBack = true;

        knockBackDir = transform.position - position;
        knockBackDir.Normalize();
        knockBackForce = force;
    }

    //�㏸�U���ŃX�e�[�W�ɂԂ��������p
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

    //���U������
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

    //�X�L���A�N�V���������G�Ɏg�p���郁�\�b�h
    public void SkillActionPlayer()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerAction");
    }
    //���G���͂������߂̃��\�b�h
    public void NomalPlayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
