using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dragon;

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




    // �p�^���V�X�e���֘A
    enum EnemyPatternSettings
    {
        IdleAnim,
        MoveAnim,
        SummonAttackAnim,
        CrushAttackAnim,
        PincerAttackAnim,
    }
    [Header("�p�^�������i�p�^���̓����͓G�̎d�l�����Q�Ƃ��Ă��������j")]
    [SerializeField] List<EnemyPatternSettings> Pattern1, Pattern2, Pattern3;
    public BossHPBar HPBar;
    public GameObject LeftHand, RightHand;

    //�����֐�
    //�v���C���[�̃I�u�W�F�N�g
    GameObject Player;
    //�U���p�^�����L�^����֐�
    int EnemyAnim = -1, EnemyPattern = -1, EnemyPatternPreb = -1, AnimationController = -1, JumpAttackAnimCtrl = -1;

    //�A�j���`�F�b�N�A�p�^�[���`�F�b�N
    bool NotInAnim = true, PatternOver = true, patternover = false, isSummonAttack = false, isCrushAttack = false, isPincerAttack = false;

    private void OnEnable()
    {
        if(animator != null)
        {
            animator.SetBool("InAdanim", false);
            LHanimator.SetBool("InAdanim", false);
            RHanimator.SetBool("InAdanim", false);
        }

    }

    protected override void Start()
    {
        base.Start();

        //animator���
        LHanimator = LeftHand.GetComponent<Animator>();
        RHanimator = RightHand.GetComponent<Animator>();
        //�v���C���[�̃I�u�W�F�N�g
        Player = GameObject.Find("Hero");
        //�J�����h��
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        //�g�p���@
        //shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
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
            while (EnemyPattern == EnemyPatternPreb)
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
                    if (EnemyAnim < Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
                        //�A�j���𗬂��
                        StartCoroutine(Pattern1[EnemyAnim].ToString());
                        //���̃A�j��
                        EnemyAnim++;
                    }
                    if (EnemyAnim == Pattern1.Count - 1 && NotInAnim)
                    {
                        NotInAnim = false;
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
        
    }



    //�����֐�




    //�U���֐�




    //�O���֐�
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
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

            //�q�b�g�X�g�b�v�����J�n
            tween = transform.DOShakePosition(power * stopState.shakTime, stopState.shakPowar, stopState.shakNum, stopState.shakRand)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    //�A�j���[�V�������I�������玞�Ԃ�߂�
                    Time.timeScale = 1;
                    //�����ʒu�ɖ߂�
                    this.transform.position = initialPos;

                });
            yield return new WaitForSeconds(power * stopState.shakTime + 0.01f);
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

        if (hp <= 0)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            OnDestroyMode();
        }
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
        GameManager.Instance.AddKillEnemy();
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
        isDestroy = true;
        IsBlowing = true;
    }
}
