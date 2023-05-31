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

    //�w�i
    [SerializeField]
    internal ParallaxBackground parallaxBackground;

    //�ʏ�U���Ďg�p�m�F
    bool canNomalAttack = true;

    //�㏸�U�����d�����h�~�pbool
    internal bool canUpAttack = true;

    //SideAttack�֘A
    const float dashingTime = 0.2f;
    bool canSideAttack = true;

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
        jump = GetComponent<Player_Jump>();
        hpparam = GameObject.Find("Hero").GetComponentInChildren<HPparam>();
    }

    void Update()
    {
        //�f�o�b�N�p�V�[�����Z�b�g
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (isExAttack || isWarpDoor)
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
        ExAttackParam.Instance.AddGauge();
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
            SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDead);
            SceneManager.LoadScene("FinishScene");
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
        if (((lsv >= 0.8 && isAttackKay) || rsv >= 0.8) 
            && !isAttack && canUpAttack)
        {
            canUpAttack = false;
            jump.UpAttack();
            isAttack = true;
        }

        //�����U���U��
        if (((lsv <= -0.8 && isAttackKay) || rsv <= -0.8)
            && !isAttack && (isFalling || isJumping) && !isDropAttack)
        {
            DownAttack._DownAttack(this);
            isAttack = true;
        }

        //���ړ��U��
        if (((lsh >= 0.8 && isAttackKay) || rsh >= 0.8)
            && !isAttack && !isSideAttack && canSideAttack)
        {
            canSideAttack = false;
            sideJudge = true;
            StartCoroutine(SideAttack());
        }
        else if(((lsh <= -0.8 && isAttackKay) || rsh <= -0.8)
                && !isAttack && !isSideAttack && canSideAttack)
        {
            canSideAttack = false;
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
                GameManager.Instance.PlayerExAttack_Start();
                ExAttackCutIn.Instance.StartCoroutine("_ExAttackCutIn", this.GetComponent<PlayerController>());
            }
        }

        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (Input.GetKeyDown("joystick button 2") && canNomalAttack)
        {
            canNomalAttack = false;
            animator.SetTrigger("IsNomalAttack");
            var skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
            StartCoroutine(_interval(skill.coolTime));
        }

        if (Input.GetKey("joystick button 2") && canNomalAttack || Input.GetKey(KeyCode.Mouse0) && canNomalAttack)
        {
            canNomalAttack = false;
            animator.SetTrigger("IsNomalAttack");
            var skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
            StartCoroutine(_interval(skill.coolTime));
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

    //���U�����E����
    private void Side(Skill skill)
    {
        Vector3 localScale = transform.localScale;
        if (transform.localScale.x > 0)
        {
            if (sideJudge)
            {
                rb.velocity = new Vector2(transform.localScale.x * skill.distance, 0f);
                StartScroll();
            }
            else if (!sideJudge)
            {
                rb.velocity = new Vector2(-transform.localScale.x * skill.distance, 0f);
                localScale.x *= -1f;
                transform.localScale = localScale;
                StartScroll();
            }
        }
        else if (transform.localScale.x < 0)
        {
            if (sideJudge)
            {
                rb.velocity = new Vector2(-transform.localScale.x * skill.distance, 0f);
                localScale.x *= -1f;
                transform.localScale = localScale;
                StartScroll();
            }
            else if (!sideJudge)
            {
                rb.velocity = new Vector2(transform.localScale.x * skill.distance, 0f);
                StartScroll();
            }
        }
        
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
        enemylist.Clear();
        isAttack = false;
        yield return new WaitForSeconds(skill.coolTime);
        canSideAttack = true;
    }

    //�K�E�Z
    public void ExAttackStart()
    {
        Debug.Log("�K�E�Z���s");
        animator.SetTrigger("ExAttack");
    }

    //�q�b�g���i�A�j���[�V��������Ăԁj
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
        animator.SetBool("IsExAttack", isExAttack);
        GameManager.Instance.PlayerExAttack_End();
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
    private void StartScroll()
    {
        if (parallaxBackground != null)
        {
            parallaxBackground.StartScroll(this.transform.position);
        }
    }

    //�N�[���^�C���p�R���[�`��
    IEnumerator _interval(float coolTime)
    {
        
        float time = coolTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        enemylist.Clear();
        canNomalAttack = true;
    }
}
