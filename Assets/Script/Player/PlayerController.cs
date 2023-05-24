using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] internal Rigidbody2D rb;
    [SerializeField] internal Animator animator;
    internal PlayerSE playerSE;

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

    [SerializeField]
    [Header("�v���C���[�X�e�[�^�X")]
    private int HP;

    [SerializeField]
    [Header("�ړ��X�e�[�^�X")]
    internal MoveData moveData = new MoveData { firstSpeed = 1f,dashSpeed = 3f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("�W�����v�X�e�[�^�X")]
    internal JumpData jumpData = new() { speed = 16f, gravity = 10f,jumpHeight = 5f, maxJumpTime = 1f };

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

    //�K�E�Z���Ɏ擾����E�ۑ�����ׂ�EnemyList[
    [SerializeField]
    internal List<GameObject> enemylist = new List<GameObject>();

    //�A�j���[�V�����p
    internal bool isFalling = false;
    internal bool isMoving = false;
    internal bool isRun = false;
    internal bool isJumping = false;
    internal bool isLanding = false;
    internal bool isSquatting = false;
    internal bool isUpAttack = false;
    internal bool isDropAttack = false;
    internal bool isSideAttack = false;
    internal bool isExAttack = false;
    internal bool isWarpDoor = false;

    //boss����p
    internal bool canMove = true;

    void Start()
    {
        playerSE = GetComponent<PlayerSE>();
        rb = GetComponent<Rigidbody2D>();
        hpparam = GameObject.Find("Hero").GetComponentInChildren<HPparam>();
    }

    void Update()
    {
        if (isExAttack)
        {
            rb.velocity = Vector2.zero;
        }

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

        if (canMove) _Skill();

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
        enemy.GetComponent<Enemy>().Damage(powar + ComboParam.Instance.GetPowerUp());
    }

    public void _Heel(int resilience)
    {
        hpparam.SetHP(hpparam.GetHP() + resilience);
    }

    public void _Damage(int power)
    {
        hpparam.DamageHP(hpparam.GetHP() - power);
        if (hpparam.GetHP() <= 0)
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDead);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
            isAttack = true;
        }

        //�����U���U��
        if (((lsv <= -0.8 && isAttackKay) || rsv <= -0.8) && !isAttack &&(isFalling || isJumping))
        {
            DownAttack._DownAttack(this);
            isAttack = true;
        }

        //���ړ��U��
        if (((lsh >= 0.8 && isAttackKay) || rsh >= 0.8)
            && !isAttack )
        {
            sideJudge = true;
            StartCoroutine(SideAttack());
        }
        else if(((lsh <= -0.8 && isAttackKay) || rsh <= -0.8)
                && !isAttack)
        {
            sideJudge = false;
            StartCoroutine(SideAttack());
        }

        //�K�E�Z
        if (Input.GetKey("joystick button 4") && Input.GetKey("joystick button 5"))
        {
            if (ExAttackParam.Instance.GetIsExAttack) 
            {
                isExAttack = true;
                animator.SetBool("IsExAttack", isExAttack);
                ExAttackParam.Instance._EXAttack();
                ExAttackCutIn.Instance.StartCoroutine("_ExAttackCutIn", this.GetComponent<PlayerController>());
            }
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
            //�ȒP�ɐ�������Ɖ��͏㉺�ɔ�΂��Ȃ��R�[�h�ł��B
            rb.AddForce(new Vector2((knockBackDir.y * knockBackForce > 5 || knockBackDir.y * knockBackForce < -5) ? ((knockBackDir.x < 0) ? -Mathf.Abs(knockBackDir.y * knockBackForce) : Math.Abs(knockBackDir.y * knockBackForce)) : knockBackDir.x * knockBackForce, ((knockBackDir.y * knockBackForce> 5 || knockBackDir.y * knockBackForce < -5)? knockBackDir.y : knockBackDir.y * knockBackForce)));//��������΂����R�[�h      �ȒP�ɐ�������Ə�Ɖ��͂T���ł����Ȃ�Ɣ�΂���Ȃ��A���E�Ɋւ��ď㉺���T�ȏ�ɂȂ�ƕS�p�[�Z���g������G�����Ƃ������Ƃ���Ȃ��̂�������̂ŁA�㉺�̔�΂��͂ō��E�̕�����^���Ĕ�΂�����B                                                        //(knockBackDir.y * knockBackForce > 7 || knockBackDir.y * knockBackForce < -7) ? knockBackDir.y * knockBackForce : knockBackDir.x * knockBackForce, (knockBackDir.y * knockBackForce > 3 || knockBackDir.y * knockBackForce < -3) ? knockBackDir.y : knockBackDir.y * knockBackForce)
            Debug.Log(new Vector2(knockBackDir.x * knockBackForce, knockBackDir.y * knockBackForce));
        }
        else
        {
            //rb.velocity = Vector2.zero;
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

    //���U��
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
    }

    //�K�E�Z
    public void ExAttackStart()
    {
        Debug.Log("�K�E�Z���s");
        animator.SetTrigger("ExAttack");
    }

    //�q�b�g���i�A�j���[�V��������Ăԁj
    public void _ExAttackHit()
    {
        //�G�t�F�N�g����
        foreach (var enemy in enemylist)
        {
            _HitEfect(enemy);
        }
        //�_���[�W�����i���݂ł͎�������j

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
        animator.SetBool("IsExAttack", isExAttack);
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

    //�X�L���A�N�V�����I�����\�b�h
    public void SkillAttackEnd()
    {
        isAttack = false;
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

    private void _HitEfect(GameObject enemy)
    {
        GameObject prefab =
        Instantiate(ExAttackHitEffect, new Vector2(enemy.transform.position.x, enemy.transform.position.y), Quaternion.identity);
        var angle = UnityEngine.Random.Range(0, 360);
        prefab.transform.Rotate(new Vector3(0,0,angle));
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
        ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
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

    internal void WarpDoor()
    {
        isWarpDoor = true;
        animator.SetBool("IsWarpDoor", isWarpDoor);
        animator.SetTrigger("WarpDoor");
    }

    internal void WarpDoorEnd()
    {
        isWarpDoor = false;
        animator.SetBool("IsWarpDoor", isWarpDoor);
    }
}
