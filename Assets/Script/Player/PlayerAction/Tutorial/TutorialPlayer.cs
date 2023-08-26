using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class TutorialPlayer : PlayerController
{
    [SerializeField, Header("�`���[�g���A���}�l�[�W���[")]
    internal TutorialScene tutorial;

    //�`���[�g���A���ebool
    internal bool isTWalk = false;
    internal bool isTJump = false;
    internal bool isTAirJump = false;
    internal bool isTAttack = false;
    internal bool isTAirAttack = false;
    internal bool isTSideAttack = false;
    internal bool isTUpAttack = false;
    internal bool isTDownAttack = false;
    internal bool isTExAttack = false;

    void Start()
    {
        playerSE = GetComponent<PlayerSE>();
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<Player_Jump>();
        hpparam = GameObject.Find("UI").GetComponentInChildren<HPparam>();

        animator.SetFloat("Speed", animSpeed);
    }

    void Update()
    {
        if (!canMove) return;
        if (isExAttack || isWarpDoor)
        {
            rb.velocity = Vector2.zero;
            gameObject.layer = LayerMask.NameToLayer("PlayerAction");
        }

        //�m�b�N�o�b�N����
        if (knockBack.canKnockBack)
        {
            if (isKnockingBack)
            {
                KnockingBack();
                animator.SetBool("IsknockBack", isKnockingBack);
                if (!canNomalAttack)
                {
                    isAttack = false;
                    canNomalAttack = true;
                }
                return;
            }
        };

        if (canMovingCounter >= 0)
        {
            canMovingCounter -= Time.deltaTime;
        }
        if (!isLanding) { _Skill(); }
        

        BackgroundScroll();

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRun", isRun);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsLanding", isLanding);
        animator.SetBool("IsGround", isGround);
    }

    //�Z���͌��m
    void _Skill()
    {
        float lsh = Input.GetAxis("L_Stick_H");
        float lsv = Input.GetAxis("L_Stick_V");
        //float rsh = Input.GetAxis("R_Stick_H");
        //float rsv = Input.GetAxis("R_Stick_V");

        if (Input.GetKey(KeyCode.JoystickButton1))
        {
            isAttackKay = true;
        }
        else { isAttackKay = false; }

        //�㏸�U��
        if (lsv >= 0.9 && isAttackKay && isTUpAttack)
            // || rsv >= 0.8)
        {
            AttackAction("UpAttack");
        }
        //�����U���U��
        if (lsv <= -0.9 && isAttackKay && isTDownAttack)
            // || rsv <= -0.8)
        {
            Debug.Log("�����U��");
            AttackAction("DawnAttack");
        }
        //���ړ��U��
        if (lsh >= 0.9 && isAttackKay && isTSideAttack)
            // || rsh >= 0.8)
        {
            AttackAction("SideAttack_right");
        }
        else if(lsh <= -0.9 && isAttackKay && isTSideAttack)
            // || rsh <= -0.8)
        {
            AttackAction("SideAttack_left");
        }
        //�K�E�Z
        if (Input.GetKey(KeyCode.JoystickButton4) && Input.GetKey(KeyCode.JoystickButton5) )
        {
            if (!isAttack && canExAttack && isTExAttack && !isExAttack) 
            {
                AttackAction("ExAttack");
            }
        }
        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (Input.GetKeyDown(KeyCode.JoystickButton2) && canNomalAttack && isTAttack)
        {
            //�ʏ�U������
            AttackAction("NomalAttack");
        }
        if (Input.GetKey(KeyCode.JoystickButton2) && canNomalAttack && isTAttack)
        {
            //�ʏ�U����������
            AttackAction("NomalAttack");
        }
    }

    //KnockBack���ꂽ�Ƃ��̏���
    void KnockingBack()
    {
        
        if (knockBackCounter == knockBack.knockBackTime)
        {
            rb.velocity = Vector2.zero;
            isAttack = false;
        }
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;
            //�ȒP�ɐ�������Ɖ��͏㉺�ɔ�΂��Ȃ��R�[�h�ł��B
            rb.AddForce(new Vector2((knockBackDir.y * knockBackForce > 5 || knockBackDir.y * knockBackForce < -5) ? ((knockBackDir.x < 0) ? -Mathf.Abs(knockBackDir.y * knockBackForce) : Math.Abs(knockBackDir.y * knockBackForce)) : knockBackDir.x * knockBackForce, ((knockBackDir.y * knockBackForce> 5 || knockBackDir.y * knockBackForce < -5)? knockBackDir.y : knockBackDir.y * knockBackForce)));//��������΂����R�[�h      �ȒP�ɐ�������Ə�Ɖ��͂T���ł����Ȃ�Ɣ�΂���Ȃ��A���E�Ɋւ��ď㉺���T�ȏ�ɂȂ�ƕS�p�[�Z���g������G�����Ƃ������Ƃ���Ȃ��̂�������̂ŁA�㉺�̔�΂��͂ō��E�̕�����^���Ĕ�΂�����B
        }
        else
        {
            isKnockingBack = false;
        }
    }

    //�w�i�X�N���[������
    private void BackgroundScroll()
    {
        if (parallaxBackground != null)
        {
            parallaxBackground.StartScroll(this.transform.position);
        }
    }
}
