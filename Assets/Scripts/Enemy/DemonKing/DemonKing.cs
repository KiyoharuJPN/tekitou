using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing : Enemy
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
    Animator LHanimator, RHanimator;

    //Idle��
    [System.Serializable]
    struct IdleStatus
    {
        [Tooltip("�ړ�����A����")]
        public float upLimit, downLimit;
        [Tooltip("����ƉE��̈ړ��X�s�[�h")]
        public float handSpeed;
        [Tooltip("�ړ���")]
        public int handFrequency;
    }
    [SerializeField] IdleStatus idleStatus;
    float LHSpeed, RHSpeed;
    int HandMoveFrequency;

    //CrushAttack��
    [System.Serializable]
    struct CrushAttckStatus
    {
        [Tooltip("���~���x�ƒǐՎ�����(0�Ńf�t�H���g)")]
        public float dropSpeed, attackHeight, furthermoreAttackHeight;
        [Tooltip("���̏���A�E�̏��")]
        public float leftCrushMaxPosX, rightCrushMaxPosX;
        [Tooltip("��̏��")]
        public float upCrushMaxPosY;
    }
    [SerializeField] CrushAttckStatus crushAttckStatus = new CrushAttckStatus { dropSpeed = 30, attackHeight = 8, furthermoreAttackHeight = 2 };

    //SommonAttack��
    [System.Serializable]
    struct SummonAttackStatus
    {
        [Tooltip("��������G�I�u�W�F�N�g(0�Ńf�t�H���g)")]
        public GameObject summonMonster;
        [Tooltip("�U�����̏㏸����Ƒ��x�Ɖ��~���x(0�Ńf�t�H���g)")]
        public float summonHeightLimit, summonHandSpeed, dropSpeed;
        [Tooltip("�����͈́i����̈ʒu�ƉE���̈ʒu0�Ńf�t�H���g�j")]
        public Vector2 LeftUpPoint, RightDownPoint;
    }
    [SerializeField] SummonAttackStatus summonAttackStatus;

    //PincerAttack��
    [System.Serializable]
    struct PincerAttackStatus
    {
        [Tooltip("�U���J�n�����Ɖ��ɉ�����}�X��")]
        public float downMass, BackMass;
        [Tooltip("�ǐՃX�s�[�h")]
        public float maxFollowSpeed, bottomLimit;
        [Tooltip("�v���C���[�֍U�����X�s�[�h")]
        public float attackSpeed;
        [Tooltip("������G�t�F�N�g�I�u�W�F�N�g")]
        public GameObject effectObject;
    }
    [SerializeField] PincerAttackStatus pincerAttackStatus = new PincerAttackStatus { downMass = 0, BackMass = 3, maxFollowSpeed = 2, bottomLimit = 0, attackSpeed = 10};
    Vector2 pincerReservePosLH, pincerReservePosRH;


    Rigidbody2D enemyLHRb, enemyRHRb;
    Vector2 LHOriginalPos, RHOriginalPos;

    // �p�^���V�X�e���֘A
    enum EnemyPatternSettings
    {
        IdleAnim,
        //MoveAnim,
        CrushAttackAnim,
        SummonAttackAnim,
        PincerAttackAnim,
    }
    [Header("�p�^�������i�p�^���̓����͓G�̎d�l�����Q�Ƃ��Ă��������j")]
    [SerializeField] List<EnemyPatternSettings> Pattern1, Pattern2, Pattern3;
    public BossHPBar HPBar;
    public GameObject LeftHand, RightHand;
    public RuntimeAnimatorController animControllerL, animControllerR;

    

    //�����֐�
    //�v���C���[�̃I�u�W�F�N�g
    GameObject Player;
    //�U���p�^�����L�^����֐�
    int EnemyAnim = -1, EnemyPattern = -1, EnemyPatternPreb = -1, AnimationController = -1, SkillAnimationController = -1, AnimationControllerPreb = -1/*�V�A�j���[�V�����p*/;
    int BossLayer;

    //�A�j���`�F�b�N�A�p�^�[���`�F�b�N
    bool NotInAnim = true, PatternOver = true, patternover = false, isSummonAttack = false, isCrushAttack = false, isPincerAttack = false;

    //�K�E�Z�p�R���[�`��
    Coroutine MovementCoroutine;


    private void OnEnable()
    {
        if(animator != null)
        {
            animator.SetBool("InAdanim", false);
            //LHanimator.SetBool("InAdanim", false);
            //RHanimator.SetBool("InAdanim", false);
            LHanimator.runtimeAnimatorController = animControllerL;
            RHanimator.runtimeAnimatorController = animControllerR;
        }
        if(HPBar != null && !HPBar.gameObject.activeSelf)
        {
            HPBar.gameObject.SetActive(true);
        }

    }

    protected override void Start()
    {
        base.Start();
        BossLayer = LayerMask.NameToLayer("BossEnemy");

        //idle�֌W
        if (idleStatus.handSpeed == 0) idleStatus.handSpeed = 1;
        if (idleStatus.handFrequency == 0) idleStatus.handFrequency = 1;
        if (idleStatus.upLimit == 0) { idleStatus.upLimit = LeftHand.transform.position.y + 5; } else { idleStatus.upLimit += LeftHand.transform.position.y; }
        if (idleStatus.downLimit == 0) { idleStatus.downLimit = LeftHand.transform.position.y - 5; } else { idleStatus.downLimit = LeftHand.transform.position.y - idleStatus.downLimit; }
        LHSpeed = idleStatus.handSpeed * 0.01f;
        RHSpeed = LHSpeed * -1;
        HandMoveFrequency = idleStatus.handFrequency * 2;

        //crushAttack�֌W
        if (crushAttckStatus.leftCrushMaxPosX == 0) { crushAttckStatus.leftCrushMaxPosX = transform.position.x - 10; } else { crushAttckStatus.leftCrushMaxPosX = transform.position.x - crushAttckStatus.leftCrushMaxPosX; }
        if (crushAttckStatus.rightCrushMaxPosX == 0) { crushAttckStatus.rightCrushMaxPosX = transform.position.x + 10; } else { crushAttckStatus.leftCrushMaxPosX = transform.position.x + crushAttckStatus.rightCrushMaxPosX; }
        if (crushAttckStatus.upCrushMaxPosY == 0) { crushAttckStatus.upCrushMaxPosY = transform.position.y + 10; } else { crushAttckStatus.upCrushMaxPosY += transform.position.y; }
        //sommonAttack�֌W
        if (summonAttackStatus.summonHandSpeed == 0) { summonAttackStatus.summonHandSpeed = 0.01f; } else { summonAttackStatus.summonHandSpeed *= 0.01f; }
        if (summonAttackStatus.summonHeightLimit == 0) { summonAttackStatus.summonHeightLimit = LeftHand.transform.position.y + 5; } else { summonAttackStatus.summonHeightLimit += LeftHand.transform.position.y; }
        if (summonAttackStatus.dropSpeed == 0) { summonAttackStatus.dropSpeed = 30; }
        if (summonAttackStatus.LeftUpPoint == Vector2.zero) { summonAttackStatus.LeftUpPoint = new Vector2(transform.position.x - 7, transform.position.y + 8); }
        if (summonAttackStatus.RightDownPoint == Vector2.zero) { summonAttackStatus.RightDownPoint = new Vector2(transform.position.x +7,transform.position.y+7); }

        //pinverAttack�֌W
        pincerReservePosLH = new Vector2(LeftHand.transform.position.x - pincerAttackStatus.BackMass, LeftHand.transform.position.y - pincerAttackStatus.downMass);
        pincerReservePosRH = new Vector2(RightHand.transform.position.x + pincerAttackStatus.BackMass, RightHand.transform.position.y - pincerAttackStatus.downMass);
        pincerAttackStatus.maxFollowSpeed *= 0.01f;

        //animator���
        LHanimator = LeftHand.GetComponent<Animator>();
        RHanimator = RightHand.GetComponent<Animator>();
        //�A�j���[�^�[���
        LHanimator.runtimeAnimatorController = animControllerL;
        RHanimator.runtimeAnimatorController = animControllerR;
        //�v���C���[�̃I�u�W�F�N�g
        Player = GameObject.Find("Hero");
        //����̃��W�b�h�{�f�B
        enemyLHRb = LeftHand.GetComponent<Rigidbody2D>();
        enemyRHRb = RightHand.GetComponent<Rigidbody2D>();
        //����̍ŏ��̈ʒu���o����
        LHOriginalPos = LeftHand.transform.position;
        RHOriginalPos = RightHand.transform.position;
        //�J�����h��
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        //�g�p���@
        //shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
    }

    //�����̓���
    protected override void Update()
    {
        base.Update();

        if (isDestroy) return;

        //�G�̃p�^�[���������_���őI��
        if (PatternOver && NotInAnim)
        {
            //�����_���œG�̃p�^�[����I��
            while (EnemyPattern == EnemyPatternPreb)
            {
                EnemyPattern = Random.Range(0, 999) % 3;
            }

            //���񓯂����̂�I�΂�Ȃ��悤�ɐ�ɑ��������
            EnemyPatternPreb = EnemyPattern;
            //�I�񂾂��K�I�ׂȂ��悤�ɂ���
            PatternOver = false;        //�p�^�[���̍Ō�Ƀ��Z�b�g����
            EnemyAnim = 0;              //Pattern�̃[������A�j���𗬂��Ă���
        }

        //�G�̃p�^�[���������ē��������s
        if (!PatternOver)
        {
            if (isPlayerExAttack) return;
            switch (EnemyPattern)
            {
                case 0:
                    //�A�j��������Ă��Ȃ���Ύ��̃A�j���𗬂��(�R���[�`����true�C��)
                    if (EnemyAnim < Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        //�A�j���𗬂��
                        MovementCoroutine = StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //���̃A�j��
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //�p�^�[�������Z�b�g�i�R���[�`���̍Ō�Ŏ��s�j
                        patternover = true;
                    }
                    break;
                case 1:
                    if (EnemyAnim < Pattern2.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern2[EnemyAnim].ToString());
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern2.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern2[EnemyAnim].ToString());
                        patternover = true;
                    }
                    break;
                case 2:
                    if (EnemyAnim < Pattern3.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern3[EnemyAnim].ToString());
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern3.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        MovementCoroutine = StartCoroutine(Pattern3[EnemyAnim].ToString());
                        patternover = true;
                    }
                    break;
                default:
                    Debug.Log("�ݒ肳��Ă��Ȃ��p�^�[�����ǂݍ��܂�܂����B");
                    break;
            }
        }

    }








    //��������
    IEnumerator IdleAnim()
    {
        SkillAnimationController = 0;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        AnimationController = 0;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        AnimationControllerPreb = 0;

        var speed = idleStatus.handSpeed * 0.01f;
        int Frequency = 1;
        LeftHand.transform.position = LHOriginalPos;
        RightHand.transform.position = RHOriginalPos;

        while (Frequency < HandMoveFrequency)
        {
            LeftHand.transform.position = new Vector2(LeftHand.transform.position.x, LeftHand.transform.position.y + LHSpeed);
            RightHand.transform.position = new Vector2(RightHand.transform.position.x, RightHand.transform.position.y + RHSpeed);

            if (LeftHand.transform.position.y >= LHOriginalPos.y - speed / 2&& LeftHand.transform.position.y <= LHOriginalPos.y + speed / 2) Frequency++;
            if (LHSpeed > 0 && LeftHand.transform.position.y > idleStatus.upLimit) LHSpeed *= -1;
            if (LHSpeed < 0 && LeftHand.transform.position.y < idleStatus.downLimit) LHSpeed *= -1;
            if (RHSpeed > 0 && RightHand.transform.position.y > idleStatus.upLimit) RHSpeed *= -1;
            if (RHSpeed < 0 && RightHand.transform.position.y < idleStatus.downLimit) RHSpeed *= -1;
            yield return new WaitForSeconds(0.01f);
        }

        LeftHand.transform.position = LHOriginalPos;
        RightHand.transform.position = RHOriginalPos;

        //�A�j���̏I���i�K�{�j
        //�p�^�[���A�j���[�V�����֘A
        AnimationController = -1;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //�p�^�[�����[�v�֘A
        NotInAnim = true;                   //���̓����Ɉڂ���悤�ɂ���
        if (patternover)                    //�p�^�[�����I��鎞�ɌĂ΂��֐�
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    IEnumerator CrushAttackAnim()
    {
        Physics2D.IgnoreLayerCollision(BossLayer, BossLayer);
        SkillAnimationController = 0;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        AnimationController = 1;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //�����V�A�j���[�V�����I�t�Z�b�g�p
        AnimationControllerPreb = 1;

        //�ꎟ�ړ��p�v���C���[�̈ʒu�Ƒ��x�̎Z�o
        float CheckMaxY = crushAttckStatus.upCrushMaxPosY - crushAttckStatus.attackHeight;
        Vector2 HandPos;
        if (Player.transform.position.y > CheckMaxY)
            HandPos = new Vector3(Player.transform.position.x, crushAttckStatus.upCrushMaxPosY, Player.transform.position.z);
        else
            HandPos = Player.transform.position + new Vector3(0, crushAttckStatus.attackHeight, 0);

        var attackMoveSpeed = Vector2.Distance(HandPos, RightHand.transform.position);
        attackMoveSpeed /= 25;

        //��U����v���C���[�̏�Ɉړ�����
        var i = 0;
        while(i < 25)
        {
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, HandPos, attackMoveSpeed);
            i++;
            yield return new WaitForEndOfFrame();
        }

        //���3�b�ԃv���C���[�̓��̏�Ɏc��
        i = 0;
        while (i < 120)
        {
            if(Player.transform.position.x < crushAttckStatus.leftCrushMaxPosX )
            {
                if (Player.transform.position.y > CheckMaxY)
                    RightHand.transform.position = new Vector2(crushAttckStatus.leftCrushMaxPosX, crushAttckStatus.upCrushMaxPosY);
                else
                    RightHand.transform.position = new Vector2(crushAttckStatus.leftCrushMaxPosX, Player.transform.position.y + crushAttckStatus.attackHeight);
            }
            else if(Player.transform.position.x > crushAttckStatus.rightCrushMaxPosX)
            {
                if (Player.transform.position.y > CheckMaxY)
                    RightHand.transform.position = new Vector2(crushAttckStatus.rightCrushMaxPosX, crushAttckStatus.upCrushMaxPosY);
                else
                    RightHand.transform.position = new Vector2(crushAttckStatus.rightCrushMaxPosX, Player.transform.position.y + crushAttckStatus.attackHeight);
            }
            else
            {
                if (Player.transform.position.y > CheckMaxY)
                    RightHand.transform.position = new Vector2(Player.transform.position.x, crushAttckStatus.upCrushMaxPosY);
                else
                    RightHand.transform.position = new Vector2(Player.transform.position.x, Player.transform.position.y + crushAttckStatus.attackHeight);
            }
            i++;
            yield return new WaitForSeconds(0.01f);
        }

        //��̈ʒu����Ɏw��̃}�X�ړ�
        i = 0;
        attackMoveSpeed = crushAttckStatus.furthermoreAttackHeight / 25;
        while(i < 30)
        {
            if (i > 5)
            {
                if (RightHand.transform.position.y > crushAttckStatus.upCrushMaxPosY)
                    RightHand.transform.position = new Vector2(RightHand.transform.position.x, crushAttckStatus.upCrushMaxPosY);
                else
                    RightHand.transform.position = new Vector2(RightHand.transform.position.x, RightHand.transform.position.y + attackMoveSpeed);
            }
            i++;
            yield return new WaitForSeconds(0.01f);
        }

        //���~�U��
        isCrushAttack = true;
        enemyRHRb.AddForce(new Vector2(0, -crushAttckStatus.dropSpeed),ForceMode2D.Impulse);
        SkillAnimationController = 1;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
    }
    IEnumerator CrushAttackAnim2()
    {
        SkillAnimationController = 2;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        yield return new WaitForSeconds(2);

        //���߂�
        var attackMoveSpeed = Vector2.Distance(RHOriginalPos, RightHand.transform.position);
        attackMoveSpeed /= 25;
        var i = 0;
        while (i < 25)
        {
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, RHOriginalPos, attackMoveSpeed);
            i++;
            if (i == 5)
            {
                AnimationController = 0;        //animator�����i�K�{�j
                LHanimator.SetInteger("AnimationController", AnimationController);
                RHanimator.SetInteger("AnimationController", AnimationController);
            }
            yield return new WaitForSeconds(0.01f);
        }

        RightHand.transform.position = RHOriginalPos;
        Physics2D.IgnoreLayerCollision(BossLayer, BossLayer, false);

        //�A�j���̏I���i�K�{�j
        //�p�^�[���A�j���[�V�����֘A
        SkillAnimationController = -1;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        AnimationController = -1;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //�����V�A�j���[�V�����I�t�Z�b�g�p
        AnimationControllerPreb = -1;
        //�p�^�[�����[�v�֘A
        NotInAnim = true;                   //���̓����Ɉڂ���悤�ɂ���
        if (patternover)                    //�p�^�[�����I��鎞�ɌĂ΂��֐�
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    IEnumerator SummonAttackAnim()
    {
        SkillAnimationController = 0;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        AnimationController = 2;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //�����V�A�j���[�V�����I�t�Z�b�g�p
        AnimationControllerPreb = 2;

        //�㏸����܂ŏ㏸����
        while (LeftHand.transform.position.y < summonAttackStatus.summonHeightLimit)
        {
            LeftHand.transform.position = new Vector2(LeftHand.transform.position.x, LeftHand.transform.position.y + summonAttackStatus.summonHandSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        enemyLHRb.velocity = Vector3.zero;
        isSummonAttack = true;
        enemyLHRb.AddForce(new Vector2(0,-summonAttackStatus.dropSpeed),ForceMode2D.Impulse);
        SkillAnimationController = 1;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
    }
    IEnumerator SummonAttackAnim2()
    {
        SkillAnimationController = 2;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        yield return new WaitForSeconds(1.5f);
        //��l��
        Vector2 summonPos = new Vector2(Random.Range(summonAttackStatus.LeftUpPoint.x,summonAttackStatus.RightDownPoint.x),Random.Range(summonAttackStatus.LeftUpPoint.y,summonAttackStatus.RightDownPoint.y));
        var newEnemy = Instantiate(summonAttackStatus.summonMonster, summonPos, Quaternion.identity);
        newEnemy.GetComponentInChildren<EnemyBuffSystem>().SetBuffTypeByScript(GetSummonProbability());
        //��l��
        summonPos = new Vector2(Random.Range(summonAttackStatus.LeftUpPoint.x, summonAttackStatus.RightDownPoint.x), Random.Range(summonAttackStatus.LeftUpPoint.y, summonAttackStatus.RightDownPoint.y));
        newEnemy = Instantiate(summonAttackStatus.summonMonster, summonPos, Quaternion.identity);
        newEnemy.GetComponentInChildren<EnemyBuffSystem>().SetBuffTypeByScript(GetSummonProbability());
        //�O�l��
        summonPos = new Vector2(Random.Range(summonAttackStatus.LeftUpPoint.x, summonAttackStatus.RightDownPoint.x), Random.Range(summonAttackStatus.LeftUpPoint.y, summonAttackStatus.RightDownPoint.y));
        newEnemy = Instantiate(summonAttackStatus.summonMonster, summonPos, Quaternion.identity);
        newEnemy.GetComponentInChildren<EnemyBuffSystem>().SetBuffTypeByScript(GetSummonProbability());

        yield return new WaitForSeconds(1.0f);

        //���߂�
        var backMoveSpeed = Vector2.Distance(LHOriginalPos, LeftHand.transform.position);
        backMoveSpeed /= 25;
        var i = 0;
        while (i < 25)
        {
            LeftHand.transform.position = Vector2.MoveTowards(LeftHand.transform.position, LHOriginalPos, backMoveSpeed);
            i++;
            if (i == 5)
            {
                AnimationController = 0;        //animator�����i�K�{�j
                LHanimator.SetInteger("AnimationController", AnimationController);
                RHanimator.SetInteger("AnimationController", AnimationController);
            }
            yield return new WaitForSeconds(0.01f);
        }

        LeftHand.transform.position = LHOriginalPos;

        //�A�j���̏I���i�K�{�j
        //�p�^�[���A�j���[�V�����֘A
        SkillAnimationController = -1;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        AnimationController = -1;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //�����V�A�j���[�V�����I�t�Z�b�g�p
        AnimationControllerPreb = -1;
        //�p�^�[�����[�v�֘A
        NotInAnim = true;                   //���̓����Ɉڂ���悤�ɂ���
        if (patternover)                    //�p�^�[�����I��鎞�ɌĂ΂��֐�
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    IEnumerator PincerAttackAnim()
    {
        SkillAnimationController = 0;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        AnimationController = 3;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //�V�A�j���[�V�����I�t�Z�b�g�p
        AnimationControllerPreb = 3;
        yield return new WaitForEndOfFrame();

        //������̉������ֈړ�
        var attackMoveSpeed = Vector2.Distance(LeftHand.transform.position, pincerReservePosLH);
        attackMoveSpeed /= 25;
        var i = 0;
        while (i < 25)
        {
            LeftHand.transform.position = Vector2.MoveTowards(LeftHand.transform.position, pincerReservePosLH, attackMoveSpeed);
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, pincerReservePosRH, attackMoveSpeed);
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        LeftHand.transform.position = pincerReservePosLH;
        RightHand.transform.position = pincerReservePosRH;

        float timer = 0;
        while (timer < 2)
        {
            timer += Time.deltaTime;
            float PlayerPosY = Player.transform.position.y;
            if(Mathf.Abs(LeftHand.transform.position.y - PlayerPosY) > pincerAttackStatus.maxFollowSpeed)
            {
                if(LeftHand.transform.position.y - PlayerPosY > 0)
                {
                    PlayerPosY = LeftHand.transform.position.y - pincerAttackStatus.maxFollowSpeed;
                }
                else
                {
                    PlayerPosY = LeftHand.transform.position.y +pincerAttackStatus.maxFollowSpeed;
                }
            }
            if(PlayerPosY > pincerAttackStatus.bottomLimit)
            {
                LeftHand.transform.position = new Vector2(LeftHand.transform.position.x, PlayerPosY);
                RightHand.transform.position = new Vector2(RightHand.transform.position.x, PlayerPosY);
            }
            else
            {
                LeftHand.transform.position = new Vector2(LeftHand.transform.position.x, pincerAttackStatus.bottomLimit);
                RightHand.transform.position = new Vector2(RightHand.transform.position.x, pincerAttackStatus.bottomLimit);
            }
            yield return new WaitForEndOfFrame();
        }

        isPincerAttack = true;
        enemyLHRb.velocity = new Vector2 (pincerAttackStatus.attackSpeed, 0);
        enemyRHRb.velocity = new Vector2(-pincerAttackStatus.attackSpeed, 0);
        SkillAnimationController = 1;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
    }
    IEnumerator PincerAttackAnim2()
    {
        SkillAnimationController = 2;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        var effectPos = new Vector2(LeftHand.transform.position.x + 5.1f, LeftHand.transform.position.y);
        Instantiate(pincerAttackStatus.effectObject, effectPos, Quaternion.identity);
        yield return new WaitForSeconds(3);

        //���߂�
        var backMoveSpeedLH = Vector2.Distance(LHOriginalPos, LeftHand.transform.position);
        var backMoveSpeedRH = Vector2.Distance(RHOriginalPos, RightHand.transform.position);
        backMoveSpeedLH /= 25;
        backMoveSpeedRH /= 25;
        var i = 0;
        while (i < 25)
        {
            LeftHand.transform.position = Vector2.MoveTowards(LeftHand.transform.position, LHOriginalPos, backMoveSpeedLH);
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, RHOriginalPos, backMoveSpeedRH);
            i++;
            if (i == 5)
            {
                AnimationController = 0;        //animator�����i�K�{�j
                LHanimator.SetInteger("AnimationController", AnimationController);
                RHanimator.SetInteger("AnimationController", AnimationController);
            }
            yield return new WaitForSeconds(0.01f);
        }

        LeftHand.transform.position = LHOriginalPos;
        RightHand.transform.position = RHOriginalPos;

        //�A�j���̏I���i�K�{�j
        //�p�^�[���A�j���[�V�����֘A
        SkillAnimationController = -1;
        LHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        RHanimator.SetInteger("SkillAnimationController", SkillAnimationController);
        AnimationController = -1;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //�����V�A�j���[�V�����I�t�Z�b�g�p
        AnimationControllerPreb = -1;
        //�p�^�[�����[�v�֘A
        NotInAnim = true;                   //���̓����Ɉڂ���悤�ɂ���
        if (patternover)                    //�p�^�[�����I��鎞�ɌĂ΂��֐�
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    IEnumerator ExBackIdleAnim()
    {
        AnimationController = 0;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        yield return new WaitForSeconds(1f);

        //��̈ʒu�Ɩ߂邽�߂̑��x���v�Z����
        var backMoveSpeedLH = Vector2.Distance(LHOriginalPos, LeftHand.transform.position);
        var backMoveSpeedRH = Vector2.Distance(RHOriginalPos, RightHand.transform.position);
        backMoveSpeedLH /= 25;
        backMoveSpeedRH /= 25;

        //����f�t�H���g�ʒu�ɖ߂�
        var i = 0;
        while (i < 25)
        {
            LeftHand.transform.position = Vector2.MoveTowards(LeftHand.transform.position, LHOriginalPos, backMoveSpeedLH);
            RightHand.transform.position = Vector2.MoveTowards(RightHand.transform.position, RHOriginalPos, backMoveSpeedRH);
            i++;
            if (i == 5)
            {
                AnimationController = 0;        //animator�����i�K�{�j
                LHanimator.SetInteger("AnimationController", AnimationController);
                RHanimator.SetInteger("AnimationController", AnimationController);
            }
            yield return new WaitForSeconds(0.01f);
        }

        LeftHand.transform.position = LHOriginalPos;
        RightHand.transform.position = RHOriginalPos;

        Physics2D.IgnoreLayerCollision(BossLayer, BossLayer, false);
        yield return new WaitForSeconds(1f);

        //�A�j���̏I���i�K�{�j
        //�p�^�[���A�j���[�V�����֘A
        AnimationController = -1;        //animator�����i�K�{�j
        LHanimator.SetInteger("AnimationController", AnimationController);
        RHanimator.SetInteger("AnimationController", AnimationController);
        //�����V�A�j���[�V�����I�t�Z�b�g�p
        AnimationControllerPreb = -1;
        //�p�^�[�����[�v�֘A
        NotInAnim = true;                   //���̓����Ɉڂ���悤�ɂ���
        if (patternover)                    //�p�^�[�����I��鎞�ɌĂ΂��֐�
        {
            PatternOver = patternover;
            patternover = false;
        }
    }

    //�����֐�
    protected override void FixedUpdate()
    {
        if (isCrushAttack)  enemyRHRb.AddForce(new Vector2(0, -5));
        if (isSummonAttack) enemyLHRb.AddForce(new Vector2(0, -5));
    }





    //�U���֐�
    public void PlayerInAttackArea(Collider2D collider,bool isLH)
    {
        if (!HadAttack)
        {
            if (isCrushAttack && !isLH)
            {
                //�U���N�[���_�E���^�C��
                HadAttack = true;
                StartCoroutine(HadAttackReset());

                //CrushAttack�̃_���[�W�ƃm�b�N�o�b�N
                collider.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 50);
                collider.gameObject.GetComponent<PlayerController>().Damage(2);
            }

            if(isSummonAttack && isLH)
            {
                //�U���N�[���_�E���^�C��
                HadAttack = true;
                StartCoroutine(HadAttackReset());

                //isSummonAttack�̃_���[�W�ƃm�b�N�o�b�N
                collider.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 40);
                collider.gameObject.GetComponent<PlayerController>().Damage(1);
            }

            if(isPincerAttack)
            {
                //�U���N�[���_�E���^�C��
                HadAttack = true;
                StartCoroutine(HadAttackReset());

                //isPincerAttack�̃_���[�W�ƃm�b�N�o�b�N
                collider.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 50);
                collider.gameObject.GetComponent<PlayerController>().Damage(2);
            }
        }
    }

    //�O���֐�
    public int GetAnimCtrlPreb()
    {
        return AnimationControllerPreb;
    }
    public int GetSkillAnimationController()
    {
        return SkillAnimationController;
    }
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DeadBoss")) return;
        //�q�b�g�X�g�b�v
        DamegeProcess(power, skill, isHitStop, exSkill);
    }

    protected override async void DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
    {
        //�q�b�g��SE�E�R���{���ԃ��Z�b�g
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
        ComboParam.Instance.ResetTime();

        //�q�b�g�X�g�b�v����
        if (isHitStop)
        {
            await EnemyGeneratar.instance.HitStopProcess(power, this.transform);
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

    //�K�E�Z�֘A
    //�G��~����
    public override void EnemyStop()
    {
        if (isPlayerExAttack) return;
        if (enemyRb != null)
        {
            isPlayerExAttack = true;
            if (MovementCoroutine != null)
            {
                StopAllCoroutines();

                //��~�p����
                NotInAnim = false;
                if (isCrushAttack) isCrushAttack = false;
                if (isPincerAttack) isPincerAttack = false;
                if (isSummonAttack) isSummonAttack = false;
            }
            enemyRb.velocity = enemyLHRb.velocity = enemyRHRb.velocity = Vector2.zero;
        }
        if (animator != null)
        {
            animator.speed = 0;
        }
    }
    //�K�E�Z���������Ă����ꍇ�̃_���[�W�����ďo��
    public override void PlaeyrExAttack_HitEnemyEnd(float power)
    {
        if (!isPlayerExAttack) return;
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        MovementCoroutine = StartCoroutine(ExBackIdleAnim());
    }
    //��~��������
    public override void Stop_End()
    {
        if (!isPlayerExAttack) return;
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        MovementCoroutine = StartCoroutine(ExBackIdleAnim());
    }


    public void OnHandTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<HandScript>() != null && isPincerAttack)
        {
            isPincerAttack = false;
            enemyLHRb.velocity = Vector2.zero;
            enemyRHRb.velocity = Vector2.zero;
            SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
            StartCoroutine(PincerAttackAnim2());
        }
    }
    public void OnHandTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage") && enemyRHRb.velocity.y == 0 && isCrushAttack)
        {
            isCrushAttack = false;
            SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
            StartCoroutine(CrushAttackAnim2());
        }
        if (collision.CompareTag("Stage") && enemyLHRb.velocity.y == 0 && isSummonAttack)
        {
            isSummonAttack = false;
            SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
            shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
            StartCoroutine(SummonAttackAnim2());
        }
    }
    public void OnHandTriggerExit2D(Collider2D collision)
    {
        
    }


    //�������Ȃ��֐�
    //Boss���S���ɌĂԊ֐�
    virtual public void Boss_Down()
    {
        ComboParam.Instance.ComboStop();
        GameManager.Instance.PlayerExAttack_Start();
        GameManager.Instance.Result_Start(3);
    }

    protected override void OnDestroyMode()
    {
        //�K�E�Z�q�b�g�G�t�F�N�g����
        BossCheckOnCamera = false;
        OnCamera = false;

        isDestroy = true;
        IsBlowing = true;
        //����̓����蔻�������
        var children = GetComponentsInChildren<HandScript>();
        foreach (var child in children)
        {
            child.DemonDead();
        }

        GameManager.Instance.PlayerStop();
        GameManager.Instance.AddKillEnemy();
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
        animator.SetBool("IsDestroy", isDestroy);
        LHanimator.SetBool("IsDestroy", isDestroy);
        RHanimator.SetBool("IsDestroy", isDestroy);
    }
    int GetSummonProbability()
    {
        var probability = (int)UnityEngine.Random.Range(0, 99) % 9;

        switch (probability)
        {
            case 0:
            case 1:
            case 2:
            default:
                return 0;
            case 3:
            case 4:
            case 5:
                return 1;
            case 6:
            case 7:
            case 8:
                return 2;
            case 9:
                return 3;
        }
    }

    protected void StopAnimatorRH()
    {
        RHanimator.speed = 0;
    }
    protected void StopAnimatorLH()
    {
        LHanimator.speed = 0;
    }
    protected void StartAnimatorRH()
    {
        RHanimator.speed = 1;
    }
    protected void StartAnimatorLH()
    {
        LHanimator.speed = 1;
    }
    protected void StopAnimatorLRH()
    {
        RHanimator.speed = LHanimator.speed = 0;
    }
    protected void StartAnimatorLRH()
    {
        RHanimator.speed = LHanimator.speed = 1;
    }
    protected void StopAllAnimator()
    {
        RHanimator.speed = LHanimator.speed = animator.speed = 0;
    }
    protected void StartAllAnimator()
    {
        RHanimator.speed = LHanimator.speed = animator.speed = 1;
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {

    }
}
