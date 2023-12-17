using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class TutorialPlayer : PlayerController
{
    [SerializeField, Header("�`���[�g���A���}�l�[�W���[")]
    internal TutorialScene tutorial;

    //�`���[�g���A���ebool
    internal bool canTWalk = false;
    internal bool canTJump = false;
    internal bool canTAirJump = false;
    internal bool canTAttack = false;
    internal bool canTAirAttack = false;
    internal bool canTSideAttack = false;
    internal bool canTUpAttack = false;
    internal bool canTDownAttack = false;
    internal bool canTExAttack = false;
    internal bool tExAttackActivCheck = false;
    internal bool canTExGageGet = false;

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

        if (!canTExGageGet)
        {
            ExAttackParam.Instance.SetGage(0);
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

        InputKay();

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRun", isRun);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsLanding", isLanding);
        animator.SetBool("IsGround", isGround);
    }
    public void Attack(Collider2D enemy, float powar, Skill skill, bool isHitStop)
    {
        ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        if (canTExGageGet)
        {
            ExAttackParam.Instance.AddGauge();
        }
        if (enemy.GetComponent<Enemy>() != null)
        {
            enemy.GetComponent<Enemy>().Damage(powar + ComboParam.Instance.GetPowerUp(), skill, isHitStop);
        }
        else
        {
            enemy.GetComponent<PartsEnemy>().Damage(powar + ComboParam.Instance.GetPowerUp(), skill, isHitStop);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<IEventStart>() != null)
        {
            eventObj = collision.gameObject.GetComponent<IEventStart>();
        };
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<IEventStart>() != null)
        {
            eventObj = null;
        }
    }

    //�Z���͌��m
    protected override void InputKay()
    {
        var inputMoveAxis = move.ReadValue<Vector2>();

        if (nomalAttack.IsPressed())
        {
            isNomalAttackKay = true;
        }
        else { isNomalAttackKay = false; }
        if (skillAttack.IsPressed())
        {
            isSkillAttackKay = true;
        }
        else { isSkillAttackKay = false; }

        if (playerState != PlayerState.Idle || playerState == PlayerState.Event) return;

        //�K�E�Z
        if (exAttack_L.IsPressed() && exAttack_R.IsPressed()/* || Input.GetKey(KeyCode.E)*/)
        {
            if (canTExAttack)
            {
                AttackAction("ExAttack"); return;
            }
        }
        //�㏸�U��
        if (inputMoveAxis.y >= 0.9 && isSkillAttackKay && canTUpAttack/* || Input.GetKey(KeyCode.I) && canTUpAttack*/)
        // || rsv >= 0.8)
        {
            AttackAction("UpAttack"); return;
        }
        //�����U���U��
        if (inputMoveAxis.y <= -0.9 && isSkillAttackKay && canTDownAttack/* || Input.GetKey(KeyCode.K) && canTDownAttack*/)
        // || rsv <= -0.8)
        {
            AttackAction("DawnAttack"); return;
        }
        //���ړ��U��
        if (inputMoveAxis.x >= 0.9 && isSkillAttackKay && canTSideAttack/* || Input.GetKey(KeyCode.L) && canTSideAttack*/)
        // || rsh >= 0.8)
        {
            AttackAction("SideAttack_right"); return;
        }
        else if(inputMoveAxis.x <= -0.9 && isSkillAttackKay && canTSideAttack/* || Input.GetKey(KeyCode.J) && canTSideAttack*/)
        // || rsh <= -0.8)
        {
            AttackAction("SideAttack_left"); return;
        }
        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (nomalAttack.WasPressedThisFrame() && canNomalAttack && canTAttack)
        {
            //�ʏ�U������
            AttackAction("NomalAttack"); return;
        }

        //�C�x���g����
        if (eventObj != null && isGround == true
            && inputMoveAxis.y >= 0.9)
        {
            eventObj.EventStart(this);
        }
    }

    //KnockBack����
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

    public new void ExAttackHitCheck()
    {
        if (exAttackEnemylist.Count == 0)
        {
            tExAttackActivCheck = true;
            ExAttackEnd();
        }
    }

    //�`���[�g���A��ExAttack�I����
    public new void ExAttackEnd()
    {
        exAttackEnemylist.Clear();
        NomalPlayer();
        GameManager.Instance.PlayerExAttack_End();
        tExAttackActivCheck = true;
        AttackEnd();
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
