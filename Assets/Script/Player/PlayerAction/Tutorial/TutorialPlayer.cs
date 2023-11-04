using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        //InputSystem
        var playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        jumpKay = playerInput.actions["Jump"];
        nomalAttack = playerInput.actions["NomalAttack"];
        skillAttack = playerInput.actions["SkillAttack"];
        exAttack_L = playerInput.actions["ExAttack_L"];
        exAttack_R = playerInput.actions["ExAttack_R"];
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
        animator.SetBool("IsDropAttack", isDropAttack);
        animator.SetBool("IsGround", isGround);
    }

    //�Z���͌��m
    void _Skill()
    {
        var inputMoveAxis = move.ReadValue<Vector2>();
        if (nomalAttack.IsPressed())
        {
            isAttackKay = true;
        }
        else { isAttackKay = false; }
        if (skillAttack.IsPressed())
        {
            isSkillAttackKay = true;
        }
        else { isSkillAttackKay = false; }

        //�㏸�U��
        if (inputMoveAxis.y >= 0.9 && isSkillAttackKay && isTUpAttack)
            // || rsv >= 0.8)
        {
            AttackAction("UpAttack");
        }
        //�����U���U��
        if (inputMoveAxis.y <= -0.9 && isSkillAttackKay && isTDownAttack)
            // || rsv <= -0.8)
        {
            AttackAction("DawnAttack");
        }
        //���ړ��U��
        if (inputMoveAxis.x >= 0.9 && isSkillAttackKay && isTSideAttack)
            // || rsh >= 0.8)
        {
            AttackAction("SideAttack_right");
        }
        else if(inputMoveAxis.x <= -0.9 && isSkillAttackKay && isTSideAttack)
            // || rsh <= -0.8)
        {
            AttackAction("SideAttack_left");
        }
        //�K�E�Z
        if (exAttack_L.IsPressed() && exAttack_R.IsPressed())
        {
            if (!isAttack && canExAttack && isTExAttack && !isExAttack) 
            {
                AttackAction("ExAttack");
            }
        }
        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (nomalAttack.WasPressedThisFrame() && canNomalAttack && isTAttack)
        {
            //�ʏ�U������
            AttackAction("NomalAttack");
        }
        if (nomalAttack.IsPressed() && canNomalAttack && isTAttack)
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
