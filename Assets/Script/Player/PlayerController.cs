using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] internal Rigidbody2D rb;
    [SerializeField] internal Animator animator;
    internal PlayerSE playerSE;
    Player_Jump jump;

    [SerializeField]
    GameObject RunEffect;

    [SerializeField]
    GameObject JumpEffect;

    [SerializeField]
    GameObject ExAttackHitEffect;

    [SerializeField]
    GameObject ExAttackLastEffect;

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

    //[SerializeField]
    //[Header("�v���C���[�X�e�[�^�X")]
    //private int HP;

    [SerializeField]
    [Header("�ړ��X�e�[�^�X")]
    internal MoveData moveData = new MoveData { firstSpeed = 1f,dashSpeed = 3f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("�W�����v�X�e�[�^�X")]
    internal JumpData jumpData = new() { speed = 16f, gravity = 10f,jumpHeight = 5f, maxJumpTime = 1f };

    [SerializeField]
    [Header("�m�b�N�o�b�N�X�e�[�^�X")]
    KnockBackData knockBack = new() { /*knockBackForce = 50,*/ knockBackTime = .4f, cantMovingTime = .4f, canKnockBack = true };

    //�w�i
    [SerializeField]
    internal ParallaxBackground parallaxBackground;

    //�ʏ�U���Ďg�p�m�F
    internal bool canNomalAttack = true;

    //�Z�֌WBool�֘A�iis:���̋Z�����@can:���̋Z���g�p�\���j
    internal bool isUpAttack = false;
    internal bool canUpAttack = true;
    internal bool isDropAttack = false;
    internal bool canDropAttack = true;
    internal bool isSideAttack = false;
    internal bool canSideAttack = true;
    internal bool isExAttack = false;
    internal bool canExAttack = false;

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

    //�K�E�Z���Ɏ擾����E�ۑ�����ׂ�EnemyList[
    internal List<GameObject> enemylist = new List<GameObject>();

    //�_���[�W�J��������
    [SerializeField]
    CameraShake shake;

    //�A�j���[�V�����p
    internal bool isFalling = false;
    internal bool isMoving = false;
    internal bool isRun = false;
    internal bool isJumping = false;
    internal bool isLanding = false;
    internal bool isSquatting = false;
    internal bool isWarpDoor = false;
    internal int attckType;

    //boss����p
    internal bool canMove = true;

    void Start()
    {
        playerSE = GetComponent<PlayerSE>();
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<Player_Jump>();
        hpparam = GameObject.Find("Hero").GetComponentInChildren<HPparam>();
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
        _Skill();

        BackgroundScroll();

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRun", isRun);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsLanding", isLanding);
    }

    public void _Attack(Collider2D enemy, float powar)
    {
        ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        ExAttackParam.Instance.AddGauge();
        if (ExAttackParam.Instance.GetCanExAttack) canExAttack = true;
        enemy.GetComponent<Enemy>().Damage(powar + ComboParam.Instance.GetPowerUp());   
    }

    public void _Heel(int resilience)
    {
        hpparam.SetHP(hpparam.GetHP() + resilience);
    }

    public void _Damage(int power)
    {

        hpparam.DamageHP(hpparam.GetHP() - power);
        shake.Shake(0.2f, 0.8f, true, true);
        if (hpparam.GetHP() <= 0)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerAction");
            isKnockingBack = false;
            SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDead);
            animator.Play("Death");
            shake.Shake(0.2f, 1f, true, true);
            GameManager.Instance.PlayerDeath();
        }
    }

    //�Z���͌��m
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
        if (((lsv >= 0.8 && isAttackKay) || rsv >= 0.8))
        {
            AttackAction("UpAttack");
        }
        //�����U���U��
        if (((lsv <= -0.8 && isAttackKay) || rsv <= -0.8))
        {
            AttackAction("DawnAttack");
        }
        //���ړ��U��
        if (((lsh >= 0.8 && isAttackKay) || rsh >= 0.8))
        {
            AttackAction("SideAttack_right");
        }
        else if(((lsh <= -0.8 && isAttackKay) || rsh <= -0.8))
        {
            AttackAction("SideAttack_left");
        }
        //�K�E�Z
        if (Input.GetKey(KeyCode.JoystickButton4) && Input.GetKey(KeyCode.JoystickButton5))
        {
            if (!isAttack && canExAttack) 
            {
                AttackAction("ExAttack");
            }
        }
        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (Input.GetKeyDown(KeyCode.JoystickButton2) && canNomalAttack)
        {
            AttackAction("NomalAttack");
        }
        if (Input.GetKey(KeyCode.JoystickButton2) && canNomalAttack)
        {
            AttackAction("NomalAttack");
        }
    }

    internal void AttackAction(string actionName)
    {
        //TODO ���݂̓X�L������Ex�A�^�b�N���s���ƃo�O��̂ŏC���K�{
        switch (actionName)
        {
            case "UpAttack":
                if (isAttack || !canUpAttack) break;
                UpAttack.UpAttackStart(this, jump,this);
                break;

            case "DawnAttack":
                if (isAttack || !(isFalling || isJumping) || !canDropAttack) break;
                DropAttack.DropAttackStart(this);
                break;

            case "SideAttack_right":
                if(isAttack || !canSideAttack) break;
                SideAttack.SideAttackStart(this, true, this);
                break;

            case "SideAttack_left":
                if (isAttack || !canSideAttack) break;
                SideAttack.SideAttackStart(this, false, this);
                break;

            case "ExAttack":
                isExAttack = true;
                canExAttack = false;
                animator.SetBool("IsExAttack", isExAttack);
                ExAttackParam.Instance._EXAttack();
                GameManager.Instance.PlayerExAttack_Start();
                ExAttackCutIn.Instance.StartCoroutine("_ExAttackCutIn", this.GetComponent<PlayerController>());
                break;
            case "NomalAttack":
                if(isAttack || !canNomalAttack) break;
                NomalAttack.NomalAttackStart(this);
                Debug.Log(AnimationCipsTime.GetAnimationTime(animator, AnimationCipsTime.ClipType.NomalAttack_Jump));
                break;
        }
    }

    //�ʏ�U���I��
    void _NomalAttackEnd()
    {
        isAttack = false;
        NomalAttack.AttackCool(this,this);
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
        canMovingCounter = knockBack.cantMovingTime;
        knockBackCounter = knockBack.knockBackTime;
        isKnockingBack = true;

        knockBackDir = transform.position - position;
        knockBackDir.Normalize();
        knockBackForce = force;
    }

    //�K�E�Z
    public void ExAttackStart()
    {
        animator.SetTrigger("ExAttack");
    }

    //---------------------------------------
    //�K�E�Z�����i�A�j���[�V��������Ăԁj
    public void _ExAttackHitEffect()
    {
        //�G�t�F�N�g����
        foreach (var enemy in enemylist)
        {
            _HitEfect(enemy.transform, UnityEngine.Random.Range(0, 360));
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        }
    }
    public void _ExAttackHitEnemyDamage()
    {
        //�_���[�W����
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.ExAttack); ;
        GameManager.Instance.PlayerExAttack_HitEnemyEnd(enemylist ,skill.damage);
        foreach (var enemy in enemylist)
        {
            _HitEfect(enemy.transform, UnityEngine.Random.Range(0, 360));
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        }
    }
    public void ExAttackHitCheck()
    {
        if(enemylist.Count == 0)
        {
            ExAttackEnd();
        }
    }
    //�K�E�Z�Ŏg�p����Enemy�����Z�b�g
    public void ExAttackEnd()
    {
        isExAttack = false;
        enemylist.Clear();
        NomalPlayer();
        animator.SetBool("IsExAttack", isExAttack);
        GameManager.Instance.PlayerExAttack_End();
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

    internal void _HitEfect(Transform enemy, int angle)
    {
        GameObject prefab =
        Instantiate(ExAttackHitEffect, new Vector2(enemy.position.x, enemy.position.y), Quaternion.identity);
        prefab.transform.Rotate(new Vector3(0,0,angle));
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
        _EfectDestroy(prefab, 0.2f);
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
        canMove = cM;
    }

    internal void WarpDoor(Transform door)
    {
        isWarpDoor = true;
        var doorPosX = door.position.x;
        var doorPosY = door.position.y;
        this.transform.position = new Vector3(doorPosX, doorPosY, transform.position.z);
        animator.SetBool("IsWarpDoor", isWarpDoor);
        animator.SetTrigger("WarpDoor");
    }

    internal void WarpDoorEnd()
    {
        isWarpDoor = false;
        animator.SetBool("IsWarpDoor", isWarpDoor);
    }

    internal void GoolDoor(Transform door)
    {
        isWarpDoor = true;
        var doorPosX = door.position.x;
        var doorPosY = door.position.y;
        this.transform.position = new Vector3(doorPosX, doorPosY, transform.position.z);
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
}
