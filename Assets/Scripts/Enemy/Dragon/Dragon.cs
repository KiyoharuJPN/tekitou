using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Enemy
{
    //�h��֘A
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("�h�ꎞ��")]
        public float Duration;
        [Tooltip("�h��̋���")]
        public float Strength;
    }

    [SerializeField]
    [Header("��ʗh��Ɋւ���")]
    public ShakeInfo _shakeInfo;
    CameraShake shake;

/*    //Platform���C��ʂ�
    [SerializeField, Tooltip("Platform�̃I�u�W�F�N�g��")]
    string Platformname;
    BoxCollider2D BoxColthis;*/

    //�W�����v�֘A
    [System.Serializable]
    public struct DragonJumpingAttackData
    {
        [Tooltip("�W�����v�A�^�b�N���W�����v�̍���")]
        public float DragonJAHeight;
        [Tooltip("�W�����v�A�^�b�N����ԍ��̃|�W�V����")]
        public Vector2 DragonJALeftPos;
        [Tooltip("�W�����v�A�^�b�N����ԉE�̃|�W�V����")]
        public Vector2 DragonJARightPos;
        [Tooltip("�ΐ�������")]
        public float StoneHeight;
        [Tooltip("�ΐ����̍�x")]
        public float StoneMaxLeftPos;
        [Tooltip("�ΐ����̉Ex")]
        public float StoneMaxRightPos;
        [Tooltip("������")]
        public float StoneQuantity;
        [Tooltip("�΂̗������x")]
        public float FallSpeed;
    }
    [SerializeField, Header("�h���S���W�����v�U���Ɋւ���")]
    public DragonJumpingAttackData _dragonJumpingAttackData;
    public GameObject JumpAttackStone;
    GameObject[] DragonFallStone;
    float[] subDistanceRdm;


    // �p�^���V�X�e���֘A
    enum EnemyPatternSettings
    {
        IdleAnim,
        MoveAnim,
        SlewAroundAnim,
        FlameBraceAnim,
        JumpAttackAnim,
        SlewAttackAnim,
    }
    public float MoveSpeed = 10;
    public BoxCollider2D dragonAttackCheckArea;
    [Header("�p�^�������i�p�^���̓����͓G�̎d�l�����Q�Ƃ��Ă��������j")]
    [SerializeField] List<EnemyPatternSettings> Pattern1,Pattern2,Pattern3;
    public BossHPBar HPBar;

    //�����֐�
    //�U���p�^�����L�^����֐�
    int EnemyAnim = -1, EnemyPattern = -1, EnemyPatternPreb = -1, AnimationController = -1, JumpAttackAnimCtrl = -1;

    //�A�j���`�F�b�N�A�p�^�[���`�F�b�N
    bool NotInAnim = true, PatternOver = true, patternover = false, isFlameBracing = false, isSlewAttacking = false, isJumpingAttacking = false;

    BoxCollider2D EnemyCollider;
    private void Awake()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        moveSpeed = MoveSpeed * -1;
        EnemyCollider = EnemyColliderArea.GetComponent<BoxCollider2D>();

        //�����΂̑��ݏꏊ��������Ă��Ȃ�������h���S���̓�����ꏊ���ڕW�ɂȂ�
        if (_dragonJumpingAttackData.StoneMaxLeftPos == 0) _dragonJumpingAttackData.StoneMaxLeftPos = _dragonJumpingAttackData.DragonJALeftPos.x;
        if (_dragonJumpingAttackData.StoneMaxRightPos == 0) _dragonJumpingAttackData.StoneMaxRightPos = _dragonJumpingAttackData.DragonJARightPos.x;
        //�K�v�����̏ꏊ���Ƃ�
        DragonFallStone = new GameObject[(int)_dragonJumpingAttackData.StoneQuantity];
        subDistanceRdm = new float[(int)_dragonJumpingAttackData.StoneQuantity];

        //�J�����h��
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        //�g�p���@
        //shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        
        //���蔲���֌W
        //BoxColthis = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage") && JumpAttackAnimCtrl == 0 && enemyRb.velocity.y > -0.1)
        {
            JumpAttackAnimCtrl = 1;
            StartCoroutine(JumpAttackAnimPlus());
        }
    }

    //�h���S���̓���
    protected override void Update()
    {
        base.Update();

        //animator�̐ݒ�
        animator.SetBool("IsBlowing", isDestroy);
        if (isDestroy) return;

        //�G�̃p�^�[���������_���őI��
        if (PatternOver)
        {
            //�����_���œG�̃p�^�[����I��
            while(EnemyPattern == EnemyPatternPreb)
            {
                EnemyPattern = Random.Range(0, 999) % 3;
            }

            //�f�o�b�O�p
            //EnemyPattern = 0;
            //Debug.Log(EnemyPattern);
            //if (!(EnemyPattern < 3 && EnemyPattern >= 0))
            //{
            //    Debug.Log("�h���S���̃����_���֐��ɃG���[���o�܂����A�v�Z�����`�F�b�N���Ă��������B");
            //}

            //���񓯂����̂�I�΂�Ȃ��悤�ɐ�ɑ��������
            EnemyPatternPreb = EnemyPattern;
            //�I�񂾂��K�I�ׂȂ��悤�ɂ���
            PatternOver = false;        //�p�^�[���̍Ō�Ƀ��Z�b�g����
            EnemyAnim = 0;              //Pattern�̃[������A�j���𗬂��Ă���
        }

        //�G�̃p�^�[���������ē��������s
        if (!PatternOver)
        {
            switch (EnemyPattern)
            {
                case 0:
                    //�A�j��������Ă��Ȃ���Ύ��̃A�j���𗬂��(�R���[�`����true�C��)
                    if(EnemyAnim < Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        //�A�j���𗬂��
                        StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //���̃A�j��
                        EnemyAnim++;
                    }
                    if(EnemyAnim == Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim=false;
                        StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //�p�^�[�������Z�b�g�i�R���[�`���̍Ō�Ŏ��s�j
                        patternover = true;
                    }
                    break;
                case 1:
                    if (EnemyAnim < Pattern2.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        StartCoroutine(Pattern2[EnemyAnim].ToString());
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern2.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        StartCoroutine(Pattern2[EnemyAnim].ToString());
                        patternover = true;
                    }
                    break;
                case 2:
                    if (EnemyAnim < Pattern3.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        StartCoroutine(Pattern3[EnemyAnim].ToString());
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern3.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        StartCoroutine(Pattern3[EnemyAnim].ToString());
                        patternover = true;
                    }
                    break;
                default:
                    Debug.Log("�ݒ肳��Ă��Ȃ��p�^�[�����ǂݍ��܂�܂����B");
                    break;
            }
        }

        FixedAnim();
        
    }
    //�A�j���̏C���͂����Ŏ��s����悤�ɂ��悤
    void FixedAnim()
    {
        if(enemyRb.velocity.y < 0.2f && JumpAttackAnimCtrl == 2)
        {
            JumpAttackAnimCtrl = 0;

            enemyRb.AddForce(new Vector2(1, -_dragonJumpingAttackData.DragonJAHeight), ForceMode2D.Impulse);
        }
    }

    //��������
    IEnumerator IdleAnim()
    {
        AnimationController = 0;        //animator�����i�K�{�j
        animator.SetInteger("AnimationController", AnimationController);

        var animcheck = 0;
        while (animcheck < 60)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

        //�A�j���̏I���i�K�{�j
        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;                   //���̓����Ɉړ��ł���悤�ɂ���
        if (patternover)                    //�p�^�[�����I��鎞�ɌĂ΂��֐�
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }

    IEnumerator MoveAnim()
    {
        AnimationController = 1;
        animator.SetInteger("AnimationController", AnimationController);

        var animcheck = 0;
        while (animcheck < 68)
        {
            animcheck++;
            transform.position = new Vector2(transform.position.x + (moveSpeed * Time.deltaTime * 0.1f), transform.position.y);
            yield return new WaitForSeconds(0.01f);
        }
        //Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");

        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;
        if (patternover)
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    IEnumerator SlewAroundAnim()
    {
        //�v���C���[�͂ǂ��ɂ��邩�𔻒肷��
        var Playerobj = GameObject.Find("Hero");
        var needTurn = false;
        if(moveSpeed < 0)
        {
            if (Playerobj.transform.position.x > gameObject.transform.position.x) needTurn = true;
        }
        if(moveSpeed > 0)
        {
            if (Playerobj.transform.position.x < gameObject.transform.position.x) needTurn = true;
        }

        if (needTurn)
        {
            AnimationController = 2;
            animator.SetInteger("AnimationController", AnimationController);

            yield return new WaitForSeconds(0.3f);
            //Debug.Log("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
        }

        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;
        if (patternover)
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }


    //�U������
    IEnumerator FlameBraceAnim()
    {
        AnimationController = 4;        //animator�����i�K�{�j
        animator.SetInteger("AnimationController", AnimationController);

        //�X�L���֐�����
        isFlameBracing = true;

        float animcheck = 0;
        while (animcheck < 72)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        while (animcheck < 79)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        ResetAttackCheckArea();
        //dragonAttackCheckArea.gameObject.SetActive(true);
        while (animcheck < 86)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea1();
        while (animcheck < 93)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea2();
        while (animcheck < 100)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea3();
        while (animcheck < 107)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea4();
        while (animcheck < 114)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea5();
        while (animcheck < 121)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea1();
        while (animcheck < 128)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea2();
        while (animcheck < 135)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea3();
        while (animcheck < 142)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea4();
        while (animcheck < 149)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea5();
        while (animcheck < 156)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea1();
        while (animcheck < 163)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea2();
        while (animcheck < 170)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea3();
        while (animcheck < 177)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea4();
        while (animcheck < 177)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckArea5();
        while (animcheck < 184)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //AttackCheckAreaOver();
        while (animcheck < 210)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //dragonAttackCheckArea.gameObject.SetActive(false);

        isFlameBracing = false;


        //�A�j���̏I���i�K�{�j
        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;                   //���̓����Ɉړ��ł���悤�ɂ���
        if (patternover)                    //�p�^�[�����I��鎞�ɌĂ΂��֐�
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }

    IEnumerator SlewAttackAnim()
    {
        AnimationController = 3;        //animator�����i�K�{�j
        animator.SetInteger("AnimationController", AnimationController);

        isSlewAttacking = true;
        ////�U���͂ƃm�b�N�o�b�N��ς���
        //var attackdam = enemyData.power;
        //enemyData.power = 2;
        //var knockbackval = enemyData.knockBackValue;
        //enemyData.knockBackValue = 100;

        float animcheck = 0;
        while(animcheck < 90)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //�h���S���R���C�_�[
        //EnemyCollider.offset = new Vector2(0.67f, -1.6f);
        //EnemyCollider.size = new Vector2(7.3f, 3.18f);
        //�U���`�F�b�NArea
        //dragonAttackCheckArea.offset = new Vector2(1.34f, -1.2f);
        //dragonAttackCheckArea.size = new Vector2(9.3f, 4f);
        //dragonAttackCheckArea.offset = new Vector2(0.9f, -1f);
        //dragonAttackCheckArea.size = new Vector2(11f, 4.45f);
        //dragonAttackCheckArea.gameObject.SetActive(true);
        while(animcheck < 96)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //EnemyCollider.offset = new Vector2(-3.3f, -1.6f);
        //EnemyCollider.size = new Vector2(7, 3.18f);
        //�U���`�F�b�NArea
        //dragonAttackCheckArea.offset = new Vector2(-3.9f, -1.2f);
        //dragonAttackCheckArea.size = new Vector2(10, 4f);
        //dragonAttackCheckArea.offset = new Vector2(-4.2f, -1f);
        //dragonAttackCheckArea.size = new Vector2(11f, 4.45f);
        while (animcheck < 104)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //EnemyCollider.offset = new Vector2(-1.7f, -1.6f);
        //EnemyCollider.size = new Vector2(6.65f, 3.18f);
        //�U���`�F�b�NArea
        //dragonAttackCheckArea.offset = new Vector2(-4.9f, -1.2f);
        //dragonAttackCheckArea.size = new Vector2(8.2f, 4f);
        //dragonAttackCheckArea.offset = new Vector2(-3.5f, -1f);
        //dragonAttackCheckArea.size = new Vector2(12.35f, 4.45f);
        while (animcheck < 112)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //dragonAttackCheckArea.gameObject.SetActive(false);
        ResetAttackCheckArea();
        ResetBoxCollider2D();
        while (animcheck < 150)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        ////�U���͂ƃm�b�N�o�b�N��߂�
        //enemyData.power = attackdam;
        //enemyData.knockBackValue = knockbackval;
        isSlewAttacking = false;

        //�A�j���̏I���i�K�{�j
        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;                   //���̓����Ɉړ��ł���悤�ɂ���
        if (patternover)                    //�p�^�[�����I��鎞�ɌĂ΂��֐�
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }

    IEnumerator JumpAttackAnim()
    {
        AnimationController = 5;        //animator�����i�K�{�j
        animator.SetInteger("AnimationController", AnimationController);
        isJumpingAttacking = true;

        yield return new WaitForEndOfFrame();
        //�A�j���[�V�����̎��Ԃ�}���āA�A�j���[�V�����̊֐��ŃW�����v��������A
        //�ڒn����ƃA�j���[�V�������������֐��ɕς���
        //var animcheck = 0;
        //while (animcheck < 51)
        //{
        //    animcheck++;
        //    yield return new WaitForSeconds(0.01f);
        //}
    }
    IEnumerator JumpAttackAnimPlus()
    {
        //�n�ʂɍ~���A�j���[�V����
        JumpAttackAnimCtrl = 1;
        animator.SetInteger("JumpAttackAnimCtrl", JumpAttackAnimCtrl);
        //�R���C�_�[�ƍU���͈͂̒���
        ////dragonAttackCheckArea.gameObject.SetActive(false);
        //dragonAttackCheckArea.offset = new Vector2(0f, -1.45f);
        //dragonAttackCheckArea.size = new Vector2(10f, 3.5f);
        gameObject.layer = LayerMask.NameToLayer("BossEnemy");
        ResetAttackCheckArea();
        //������̉�ʗh��
        shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);

        var animcheck = 0;
        while (animcheck < 5)
        {
            animcheck++;
            transform.position = new Vector2(transform.position.x + (moveSpeed * Time.deltaTime * 0.1f), transform.position.y);
            yield return new WaitForSeconds(0.01f);
        }
        //dragonAttackCheckArea.offset = new Vector2(0f, -1.45f);
        //dragonAttackCheckArea.size = new Vector2(11.5f, 3.5f);
        while (animcheck < 10)
        {
            animcheck++;
            transform.position = new Vector2(transform.position.x + (moveSpeed * Time.deltaTime * 0.1f), transform.position.y);
            yield return new WaitForSeconds(0.01f);
        }
        //dragonAttackCheckArea.offset = new Vector2(0f, -1f);
        //dragonAttackCheckArea.size = new Vector2(13f, 4.6f);
        while (animcheck < 15)
        {
            animcheck++;
            transform.position = new Vector2(transform.position.x + (moveSpeed * Time.deltaTime * 0.1f), transform.position.y);
            yield return new WaitForSeconds(0.01f);
        }
        while (animcheck < 20)
        {
            animcheck++;
            transform.position = new Vector2(transform.position.x + (moveSpeed * Time.deltaTime * 0.1f), transform.position.y);
            yield return new WaitForSeconds(0.01f);
        }
        //dragonAttackCheckArea.gameObject.SetActive(false);
        while (animcheck < 50)
        {
            animcheck++;
            transform.position = new Vector2(transform.position.x + (moveSpeed * Time.deltaTime * 0.1f), transform.position.y);
            yield return new WaitForSeconds(0.01f);
        }
        CreateStoneAttack();

        while (animcheck < 75)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }

        isJumpingAttacking = false;
        //�A�j���̏I���i�K�{�j
        animator.SetInteger("AnimationController", -1);
        JumpAttackAnimCtrl = -1;
        animator.SetInteger("JumpAttackAnimCtrl", JumpAttackAnimCtrl);
        NotInAnim = true;                   //���̓����Ɉړ��ł���悤�ɂ���
        if (patternover)                    //�p�^�[�����I��鎞�ɌĂ΂��֐�
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
        }
    }

    //�O���֐�
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DeadBoss")) return;
        //�q�b�g�X�g�b�v
        StartCoroutine(DamegeProcess(power, skill, isHitStop, exSkill));
    }

    protected override IEnumerator DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
    {
        //�q�b�g��SE�E�R���{���ԃ��Z�b�g
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
        ComboParam.Instance.ResetTime();

        //�q�b�g�G�t�F�N�g����
        if (skill != null)
        {
            HitEfect(this.transform, skill.hitEffectAngle);
        }
        else HitEfect(this.transform, UnityEngine.Random.Range(0, 360));

        //�q�b�g�X�g�b�v����
        if (isHitStop)
        {
            Vector3 initialPos = this.transform.position;//�����ʒu�ۑ�
            Time.timeScale = 0;

            var stopTime = power * stopState.shakTime;
            if (stopTime > stopState.shakTimeMax)
            {
                stopTime = stopState.shakTimeMax;
            }

            //�q�b�g�X�g�b�v�����J�n
            tween = transform.DOShakePosition(stopTime, stopState.shakPowar, stopState.shakNum, stopState.shakRand)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    //�A�j���[�V�������I�������玞�Ԃ�߂�
                    Time.timeScale = 1;
                    //�����ʒu�ɖ߂�
                    this.transform.position = initialPos;

                });
            yield return new WaitForSeconds(stopTime + 0.01f);
        }

        //�q�b�g�����o�i�G�_�Łj
        if (!hadDamaged)
        {
            StartCoroutine(HadDamaged());
            hadDamaged = true;
        }

        hp -= power;

        //HP�Q�[�W���g�p���Ă��邩�ǂ���
        if (HPBar != null)
        {
            HPBar.ReductionHP();
        }

        if (hp <= 0&&!isDestroy)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            OnDestroyMode();
        }
    }

    public override void TurnAround()
    {
        bool InCheck = true;
        if (transform.localScale.x == 1f && InCheck)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            InCheck = false;
        }
        if (transform.localScale.x == -1f && InCheck)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            //InCheck = false;
        }
        moveSpeed *= -1;
    }

    // �A�j���[�^�ŌĂ΂��W�����v�A�j���[�V�������������Ƃ��ɃW�����v����֐�
    public void BossJAJump()
    {
        //�R���C�_�[�ݒ�
        //dragonAttackCheckArea.offset = new Vector2(0, -1.6f);
        //dragonAttackCheckArea.size = new Vector2(9.6f, 3.18f);
        dragonAttackCheckArea.offset = new Vector2(0.5f, -1.2f);
        dragonAttackCheckArea.size = new Vector2(12f, 3.9f);
        gameObject.layer = LayerMask.NameToLayer("NoColliderEnemy");
        dragonAttackCheckArea.gameObject.SetActive(true);

        //�h���S���W�����v
        var jumpWidth = 1.0f;
        if (transform.localScale.x > 0f)
        {
            jumpWidth = _dragonJumpingAttackData.DragonJALeftPos.x - transform.position.x;
        }else if (transform.localScale.x < 0f)
        {
            jumpWidth = _dragonJumpingAttackData.DragonJARightPos.x - transform.position.x;
        }
        //���蔲���֌W
        //BoxColthis.isTrigger = true;
        enemyRb.AddForce(new Vector2(jumpWidth * 0.5f, _dragonJumpingAttackData.DragonJAHeight),ForceMode2D.Impulse);

        //�ڒn���鎞�Ɏ��̃A�j���[�V�����𗬂���悤��if���̔��f�v�f�ɂ���
        JumpAttackAnimCtrl = 2;
    }

    public void PlayerInAttackArea(Collider2D col)
    {
        if (!HadAttack)
        {
            if (isFlameBracing)
            {
                //�U���N�[���_�E���^�C��
                HadAttack = true;
                StartCoroutine(HadAttackReset());
                //FlameBracing�̃_���[�W�ƃm�b�N�o�b�N
                col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 30);
                col.gameObject.GetComponent<PlayerController>().Damage(2);
            }

            if (isSlewAttacking)
            {
                //�U���N�[���_�E���^�C��
                HadAttack = true;
                StartCoroutine(HadAttackReset());
                //SlewAttacking�̃_���[�W�ƃm�b�N�o�b�N
                col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 30);
                col.gameObject.GetComponent<PlayerController>().Damage(2);
            }

            if (isJumpingAttacking)
            {
                //�U���N�[���_�E���^�C��
                HadAttack = true;
                StartCoroutine(HadAttackReset());
                //SlewAttacking�̃_���[�W�ƃm�b�N�o�b�N
                col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 30);
                col.gameObject.GetComponent<PlayerController>().Damage(2);
            }
        }
    }

    //�h���S�����S���ɌĂԊ֐�
    virtual public void Boss_Down()
    {
        ComboParam.Instance.ComboStop();
        GameManager.Instance.PlayerExAttack_Start();
        GameManager.Instance.Result_Start(2);
    }



    //�����֐�
    protected override void OnDestroyMode()
    {
        isDestroy = true;
        //�K�E�Z�q�b�g�G�t�F�N�g����
        BossCheckOnCamera = false;
        OnCamera = false;
        GameManager.Instance.AddKillEnemy();
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
    }

    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10));
    }

    void ResetBoxCollider2D()
    {
        EnemyCollider.offset = new Vector2(0, -1.6f);
        EnemyCollider.size = new Vector2(9.6f, 3.18f);
    }

    //Dragon���̑��֘A�̓����֐�
    void ResetAttackCheckArea()
    {
        dragonAttackCheckArea.offset = new Vector2(-6.64f, -2.2f);
        dragonAttackCheckArea.size = new Vector2(4.3f, 2f);
    }
    void AttackCheckArea1()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.1f, -1.5f);
        dragonAttackCheckArea.size = new Vector2(7.4f, 3.4f);
    }
    void AttackCheckArea2()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.3f, -1.3f);
        dragonAttackCheckArea.size = new Vector2(7.8f, 3.8f);
    }
    void AttackCheckArea3()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.5f, -1.1f);
        dragonAttackCheckArea.size = new Vector2(7.5f, 4.2f);
    }
    void AttackCheckArea4()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.4f, -1f);
        dragonAttackCheckArea.size = new Vector2(8f, 4.4f);
    }
    void AttackCheckArea5()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.2f, -1.3f);
        dragonAttackCheckArea.size = new Vector2(7.6f, 3.85f);
    }
    void AttackCheckAreaOver()
    {
        dragonAttackCheckArea.offset = new Vector2(-8f, -1.8f);
        dragonAttackCheckArea.size = new Vector2(6.65f, 2.85f);
    }

    //�ΐ����p
    void CreateStoneAttack()
    {
        var Distance = _dragonJumpingAttackData.StoneMaxRightPos - _dragonJumpingAttackData.StoneMaxLeftPos;
        var subDistance = Distance / (_dragonJumpingAttackData.StoneQuantity);
        //Debug.Log(subDistance+"\n"+Distance);
        for(int i =0; i <= _dragonJumpingAttackData.StoneQuantity - 1; i++)
        {
            if(i == 0)
            {
                subDistanceRdm[i] = Random.Range(0f, subDistance);
            }
            else
            {
                subDistanceRdm[i] = Random.Range(2.5f, subDistance);
            }
        }
        subDistance = 0;
        for (int i = 1; i <= _dragonJumpingAttackData.StoneQuantity; i++)
        {
            subDistance += subDistanceRdm[i-1];
            if(subDistance > Distance) { subDistance = Distance; }
            //DragonFallStone[i - 1] = ObjectPool.Instance.GetObject(JumpAttackStone);
            DragonFallStone[i - 1] = Instantiate(JumpAttackStone);
            DragonFallStone[i - 1].transform.position = new Vector2(_dragonJumpingAttackData.StoneMaxLeftPos + subDistance, gameObject.transform.position.y + _dragonJumpingAttackData.StoneHeight);
            DragonFallStone[i - 1].GetComponent<DragonFallStone>().SetSpeed(_dragonJumpingAttackData.FallSpeed);
        }
    }

    void PlayDragonRoarSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.DragonRoar);
    }
}
