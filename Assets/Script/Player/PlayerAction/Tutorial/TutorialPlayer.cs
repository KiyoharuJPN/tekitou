using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class TutorialPlayer : PlayerController
{
    [SerializeField, Header("チュートリアルマネージャー")]
    internal TutorialScene tutorial;

    //チュートリアル各bool
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

        //ノックバック処理
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

    //技入力検知
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

        //上昇攻撃
        if (inputMoveAxis.y >= 0.9 && isSkillAttackKay && isTUpAttack)
            // || rsv >= 0.8)
        {
            AttackAction("UpAttack");
        }
        //落下攻撃攻撃
        if (inputMoveAxis.y <= -0.9 && isSkillAttackKay && isTDownAttack)
            // || rsv <= -0.8)
        {
            AttackAction("DawnAttack");
        }
        //横移動攻撃
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
        //必殺技
        if (exAttack_L.IsPressed() && exAttack_R.IsPressed())
        {
            if (!isAttack && canExAttack && isTExAttack && !isExAttack) 
            {
                AttackAction("ExAttack");
            }
        }
        //手動攻撃：攻撃ボタンが押されせたとき
        if (nomalAttack.WasPressedThisFrame() && canNomalAttack && isTAttack)
        {
            //通常攻撃入力
            AttackAction("NomalAttack");
        }
        if (nomalAttack.IsPressed() && canNomalAttack && isTAttack)
        {
            //通常攻撃長押し中
            AttackAction("NomalAttack");
        }
    }

    //KnockBackされたときの処理
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
            //簡単に説明すると下は上下に飛ばさないコードです。
            rb.AddForce(new Vector2((knockBackDir.y * knockBackForce > 5 || knockBackDir.y * knockBackForce < -5) ? ((knockBackDir.x < 0) ? -Mathf.Abs(knockBackDir.y * knockBackForce) : Math.Abs(knockBackDir.y * knockBackForce)) : knockBackDir.x * knockBackForce, ((knockBackDir.y * knockBackForce> 5 || knockBackDir.y * knockBackForce < -5)? knockBackDir.y : knockBackDir.y * knockBackForce)));//横だけ飛ばされるコード      簡単に説明すると上と下は５よりでかくなると飛ばされない、左右に関して上下が５以上になると百パーセント横から触ったということじゃないのが分かるので、上下の飛ばす力で左右の方向を与えて飛ばさせる。
        }
        else
        {
            isKnockingBack = false;
        }
    }

    //背景スクロール処理
    private void BackgroundScroll()
    {
        if (parallaxBackground != null)
        {
            parallaxBackground.StartScroll(this.transform.position);
        }
    }
}
