using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using PlayerData;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] internal Rigidbody2D rb;
    [SerializeField] internal Animator animator;

    internal PlayerSE playerSE;
    internal Player_Jump jump;

    [SerializeField]
    internal GameObject RunEffect;

    [SerializeField]
    internal GameObject JumpEffect;

    [SerializeField]
    internal GameObject heelEffect;

    [SerializeField]
    internal GameObject ExAttackHitEffect;

    [SerializeField]
    internal GameObject ExAttackLastEffect;

    [System.Serializable]
    public struct MoveData
    {
        [Tooltip("�������x")]
        public float firstSpeed;
        [Tooltip("�W�����v���ړ����x")]
        public float jumpFirstSpeed;
        [Tooltip("�_�b�V���ω����x")]
        public float dashSpeed;
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
        [Tooltip("���x")]
        public float speed;
        [Tooltip("�������x")]
        public float gravity;
        [Tooltip("�W�����v�\����")]
        public float jumpHeight;
        [Tooltip("2�i�ڃW�����v�\����")]
        public float secondJumpHeight;
        [Tooltip("�W�����v���Ԃ̏��")]
        public float maxJumpTime;
    }

    [System.Serializable]
    internal struct KnockBackData
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

    internal enum PlayerState
    {
        Idle,
        NomalAttack,
        UpAttack,
        SideAttack,
        DownAttack,
        ExAttack,
        Event
    }
    
    internal PlayerState playerState;

    [SerializeField]
    [Header("�ړ��X�e�[�^�X")]
    internal MoveData moveData = new MoveData { firstSpeed = 1f,dashSpeed = 3f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("�W�����v�X�e�[�^�X")]
    internal JumpData jumpData = new() { speed = 16f, gravity = 10f,jumpHeight = 5f, maxJumpTime = 1f };

    [SerializeField]
    [Header("�m�b�N�o�b�N�X�e�[�^�X")]
    internal KnockBackData knockBack = new() { /*knockBackForce = 50,*/ knockBackTime = .4f, cantMovingTime = .4f, canKnockBack = true };

    //�w�i
    [SerializeField]
    internal ParallaxBackground parallaxBackground;

    //�ʏ�U���Ďg�p�m�F
    internal bool isNomalAttack = false;
    internal bool canNomalAttack = true;

    //�Z�֌WBool�֘A�ican:���̋Z���g�p�\���j
    internal bool canUpAttack = true;
    internal bool canDropAttack = true;
    internal bool canSideAttack = true;
    internal bool isExAttack = false;
    internal bool canExAttack = false;

    internal bool isUpAttack = false;
    internal bool isSideAttack = false;

    //KnockBack�֘A
    internal Vector2 knockBackDir;   //�m�b�N�o�b�N��������
    internal bool isKnockingBack;    //�m�b�N�o�b�N����Ă��邩�ǂ���
    internal float knockBackCounter; //���Ԃ𑪂�J�E���^�[
    internal float canMovingCounter;
    internal float knockBackForce;   //�m�b�N�o�b�N������
    [SerializeField, Header("HP�Q�[�W")]
    internal HPparam hpparam;

    //���̓L�[
    internal bool isAttack = false;
    internal bool isNomalAttackKay = false;
    internal bool isSkillAttackKay = false;

    //�U�����E�K�E�Z���Ɏg�p����ׂ�EnemyList
    [SerializeField, Header("exAttackArea")]
    internal ExAttackArea exAttacArea;
    internal List<GameObject> enemylist = new List<GameObject>();
    internal List<GameObject> exAttackEnemylist = new List<GameObject>();

    //�_���[�W�J��������
    [SerializeField]
    internal CameraShake shake;

    //�A�j���[�V�����p
    internal bool isFalling = false;
    internal bool isMoving = false;
    internal bool isRun = false;
    internal bool isJumping = false;
    internal bool isLanding = false;
    internal bool isSquatting = false;
    internal bool isWarpDoor = false;

    internal bool isGround = false;
    internal float animSpeed = 1f;

    //���G����
    internal bool inInvincibleTimeKnockBack = false, inInvincibleTimeLife = false;
    public float InvincibleTime = 20;
    SpriteRenderer sprite;

    //boss����p
    internal bool canMove = true;

    //���S����
    bool isDead = false, isgroundpreb = false;
    public bool GetIsDead {  get { return isDead; } }

    //InputSystem
    internal InputAction move, jumpKay, nomalAttack, skillAttack, exAttack_L, exAttack_R;

    void Start()
    {
        playerSE = GetComponent<PlayerSE>();
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<Player_Jump>();
        //hpparam = GameObject.Find("UI").GetComponentInChildren<HPparam>();
        animator.SetFloat("Speed", animSpeed);

        sprite = GetComponent<SpriteRenderer>();

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
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRun", isRun);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsLanding", isLanding);
        animator.SetBool("IsGround", isGround);

        if (playerState == PlayerState.Event)
        {
            rb.velocity = Vector2.zero;
            gameObject.layer = LayerMask.NameToLayer("PlayerAction");
            return;
        }

        if (isGround != isgroundpreb) { isgroundpreb = isGround; Debug.Log("player" + isGround + "and" + isgroundpreb); }
        //Debug.Log(isgroundpreb);

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

        AttacKInputKay();

        
    }

    public void Attack(Collider2D enemy, float powar, Skill skill, bool isHitStop)
    {
        ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        ExAttackParam.Instance.AddGauge();
        if (enemy.GetComponent<Enemy>() != null)
        {
            enemy.GetComponent<Enemy>().Damage(powar + ComboParam.Instance.GetPowerUp(), skill, isHitStop);
        }
        else
        {
            enemy.GetComponent<PartsEnemy>().Damage(powar + ComboParam.Instance.GetPowerUp(), skill, isHitStop);
        }
    }

    public void Heel(int resilience)
    {
        hpparam.SetHP(hpparam.GetHP() + resilience);
        HeelEffect();
    }

    public virtual void Damage(int power)
    {

        if (gameObject.GetComponent<InvinciblBuff>()) { return; }
        if (!inInvincibleTimeLife)
        {
            //���G���Ԃ̌v�Z
            inInvincibleTimeLife = true;
            StartCoroutine(InvincibleLife());

            //���C�t�v�Z
            hpparam.DamageHP(hpparam.GetHP() - power);
            shake.Shake(0.2f, 0.8f, true, true);
            if (hpparam.GetHP() <= 0 && !isDead)
            {
                PlayerDead();
            }
        }
    }

    public void PlayerDead()
    {
        if(isDead) { return; }
        playerState = PlayerState.Event;
        isDead = true;
        this.tag = "DeadPlayer";
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        isKnockingBack = false;
        SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDead);
        animator.Play("Death");
        shake.Shake(0.2f, 1f, true, true);
        GameManager.Instance.PlayerDeath();
    }

    //�Z���͌��m
    protected virtual void AttacKInputKay()
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

        //�㏸�U��
        if (inputMoveAxis.y >= 0.9 && isSkillAttackKay)
        {
            AttackAction("UpAttack");
        }
        //�����U���U��
        if (inputMoveAxis.y <= -0.9 && isSkillAttackKay)
        {
            AttackAction("DawnAttack");
        }
        //���ړ��U��
        if (inputMoveAxis.x >= 0.9 && isSkillAttackKay)
        {
            AttackAction("SideAttack_right");
        }
        else if (inputMoveAxis.x <= -0.9 && isSkillAttackKay)
        {
            AttackAction("SideAttack_left");
        }
        //�K�E�Z
        if (exAttack_L.IsPressed() && exAttack_R.IsPressed())
        {
            AttackAction("ExAttack");
        }
        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (nomalAttack.WasPressedThisFrame())
        {
            //�ʏ�U������
            AttackAction("NomalAttack");
        }
    }

    //�A�N�V�������s
    internal void AttackAction(string actionName)
    {
        switch (actionName)
        {
            case "UpAttack":
                if (!canUpAttack) return; 
                playerState = PlayerState.UpAttack;
                canUpAttack = false;
                UpAttack.UpAttackStart(this, jump,this);
                break;

            case "DawnAttack":
                if (!canDropAttack) return;
                playerState = PlayerState.DownAttack;
                canDropAttack = false;
                DropAttack.DropAttackStart(this,this);
                break;

            case "SideAttack_right":
                if (!canSideAttack) return;
                playerState = PlayerState.SideAttack;
                canSideAttack = false;
                SideAttack.SideAttackStart(this, true, this);
                break;

            case "SideAttack_left":
                if (!canSideAttack) return;
                playerState = PlayerState.SideAttack;
                canSideAttack = false;
                SideAttack.SideAttackStart(this, false, this);
                break;

            case "ExAttack":
                if(!canExAttack) return;
                playerState = PlayerState.ExAttack;
                canExAttack = false;
                ExAttack.ExAttackStart(this);
                break;

            case "NomalAttack":
                playerState = PlayerState.NomalAttack;
                NomalAttack.NomalAttackStart(this,this);
                break;
        }
    }

    //�U���I�����̏���
    internal async void AttackEnd()
    {
        switch (playerState)
        {
            case PlayerState.Event: break;
            case PlayerState.ExAttack:
                animator.SetBool("IsExAttack", false);
                exAttackEnemylist.Clear();
                break;
            case PlayerState.SideAttack:
                animator.SetBool("IsSideAttack", false);
                break;
            case PlayerState.UpAttack:
                animator.SetBool("IsUpAttack", false);
                break;
            case PlayerState.DownAttack:
                animator.Play("Hero_DropAttack_End");
                animator.SetBool("IsDropAttack", false);
                await UniTask.Delay(270);
                break;
        }
        if(playerState != PlayerState.Event) 
        {
            Debug.Log("�A�C�h���Ɉڍs");
            playerState = PlayerState.Idle;
        }
        enemylist.Clear();
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

    //KnockBack���ꂽ��ĂԊ֐�
    public void KnockBack(Vector3 position, float force)
    {
        if (!inInvincibleTimeKnockBack)
        {
            // ���G���Ԃ̌v�Z
            inInvincibleTimeKnockBack = true;
            StartCoroutine(InvincibleKnockBack());

            //�m�b�N�o�b�N
            canMovingCounter = knockBack.cantMovingTime;
            knockBackCounter = knockBack.knockBackTime;
            isKnockingBack = true;

            knockBackDir = transform.position - position;
            knockBackDir.Normalize();
            knockBackForce = force;
        }
    }

    //�K�E�Z
    internal void CanExAttackCheck() //�K�E�Z�g�p�\�`�F�b�N
    {
        if (ExAttackParam.Instance.GetCanExAttack)
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.exGageMax);
            canExAttack = true;
        }
    }

    public void ExAttackStart()
    {
        animator.SetTrigger("ExAttack");
    }

    //---------------------------------------
    //�K�E�Z�����i�A�j���[�V��������Ăԁj
    public void _ExAttackHitEffect()
    {
        //�G�t�F�N�g����
        foreach (var enemy in exAttackEnemylist)
        {
            if (enemy == null) return;
            HitEfect(enemy.transform, UnityEngine.Random.Range(0, 360));
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        }
    }

    public void _ExAttackHitEnemyDamage()
    {
        //�_���[�W����
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.ExAttack);
        
        GameManager.Instance.PlayerExAttack_HitEnemyEnd(exAttackEnemylist, skill.damage + ComboParam.Instance.GetPowerUp());
    }
    public void ExAttackHitCheck()
    {
        if(exAttackEnemylist.Count == 0)
        {
            ExAttackEnd();
        }
    }
    //�K�E�Z�Ŏg�p����Enemy�����Z�b�g
    public void ExAttackEnd()
    {
        exAttackEnemylist.Clear();
        NomalPlayer();
        GameManager.Instance.PlayerExAttack_End();
        AttackEnd();
    }
    //---------------------------------------

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

    //�X�L���A�N�V�����I�����\�b�h
    public void SkillAttackEnd()
    {
        isAttack = false;
        enemylist.Clear();
    }

    //�C�x���g�J�n
    public void EventStart()
    {
        playerState = PlayerState.Event;
    }
    //�C�x���g�I��
    public void EventEnd()
    {
        playerState = PlayerState.Idle;
    }

    //�����G�t�F�N�g����
    public void _RunEffect()
    {
        GameObject prefab = 
        Instantiate(RunEffect, new Vector2(this.transform.position.x, this.transform.position.y - 0.8f), Quaternion.identity);
        if(gameObject.transform.localScale.x < 0)
        {
            Vector2 scale = prefab.transform.localScale;
            scale.x *= -1f;
            prefab.transform.localScale = scale;
        }
        _EfectDestroy(prefab, 0.3f);
    }

    //�_�u���W�����v�G�t�F�N�g����
    public void _JumpEffect()
    {
        GameObject prefab =
        Instantiate(JumpEffect, new Vector2(this.transform.position.x, this.transform.position.y - 0.8f), Quaternion.identity);
        if (gameObject.transform.localScale.x < 0)
        {
            Vector2 scale = prefab.transform.localScale;
            scale.x *= -1f;
            prefab.transform.localScale = scale;
        }
        _EfectDestroy(prefab, 0.2f);
    }

    internal void HitEfect(Transform enemy, int angle)
    {
        GameObject prefab =
        Instantiate(ExAttackHitEffect, new Vector2(enemy.position.x, enemy.position.y), Quaternion.identity);
        prefab.transform.Rotate(new Vector3(0,0,angle));
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
        _EfectDestroy(prefab, 0.2f);
    }

    //�q�[���G�t�F�N�g����
    void HeelEffect()
    {
        GameObject prefab =
        Instantiate(heelEffect, new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
        prefab.transform.parent = this.transform.transform;
        _EfectDestroy(prefab, 1f);
    }

    public void _ExAttackLastEffect()
    {
        GameObject prefab;
        if (gameObject.transform.localScale.x < 0)
        {
            prefab =
            Instantiate(ExAttackLastEffect, new Vector2(this.transform.position.x - 4f, this.transform.position.y), Quaternion.identity);
        }
        else
        {
            prefab =
            Instantiate(ExAttackLastEffect, new Vector2(this.transform.position.x + 4f, this.transform.position.y), Quaternion.identity);
        }
        _EfectDestroy(prefab, 0.3f);
    }

    void _EfectDestroy(GameObject prefab, float time)
    {
        Destroy(prefab, time);
    }

    public void SetCanMove(bool cM)
    {
        rb.velocity = Vector2.zero;
        if(cM)
        {
            playerState = PlayerState.Idle;
        }
        else if(!cM) 
        {
            playerState = PlayerState.Event;
        }
        canMove = cM;
        isUpAttack = false;
        isSideAttack = false;
        isNomalAttack = false;
        canDropAttack = true;
        animator.SetBool("IsDropAttack", false);
        AttackEnd();
    }

    internal void WarpDoor(Transform door)
    {
        isRun = false;
        isMoving = false;
        isJumping = false;
        isFalling = false;
        isSquatting = false;
        animator.SetBool("IsRun", isRun);
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsSquatting", isSquatting);
        var doorPosX = door.position.x;
        var doorPosY = door.position.y;
        transform.localScale = new Vector3(1, 1, 1);
        this.transform.position = new Vector3(doorPosX, doorPosY, transform.position.z);
        this.rb.velocity = new Vector2 (0, 0);
        animator.SetBool("IsWarpDoor", true);
        gameObject.layer = LayerMask.NameToLayer("PlayerAction");
        animator.SetTrigger("WarpDoor");
    }

    internal void WarpDoorEnd()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        animator.SetBool("IsWarpDoor", false);
    }

    internal void GoolDoor(Transform door)
    {
        isWarpDoor = true;
        var doorPosX = door.position.x;
        var doorPosY = door.position.y;
        this.transform.position = new Vector3(doorPosX, doorPosY, transform.position.z);
        this.rb.velocity = new Vector2(0, 0);
        animator.SetTrigger("GoolDoor");
        SoundManager.Instance.PlaySE(SESoundData.SE.GoalSE);
    }

    //�w�i�X�N���[������
    private void BackgroundScroll()
    {
        if (parallaxBackground != null)
        {
            parallaxBackground.StartScroll(this.transform.position);
        }
    }

    protected IEnumerator InvincibleKnockBack()
    {
        var n = InvincibleTime;
        while (n > 0)
        {
            n--;
            if(n%2 == 0)
            {
                sprite.color = new Color(1, 1, 1);
            }
            else
            {
                sprite.color = new Color(1, .3f, .3f);
            }
            yield return new WaitForSeconds(0.01f);
        }
        inInvincibleTimeKnockBack = false;
    }

    protected IEnumerator InvincibleLife()
    {
        var n = InvincibleTime;
        while (n > 0)
        {
            n--;
            yield return new WaitForSeconds(0.01f);
        }
        inInvincibleTimeLife = false;
    }

    public void AddAttack(SlashingBuff.SlashingType type)
    {
        if (gameObject.GetComponent<SlashingBuff>())
            gameObject.GetComponent<SlashingBuff>().Slashing(type, gameObject);
    }
}
