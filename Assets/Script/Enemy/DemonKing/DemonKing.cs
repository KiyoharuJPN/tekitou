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




    // �p�^���V�X�e���֘A
    enum EnemyPatternSettings
    {
        IdleAnim,
        MoveAnim,
        SummonAttackAnim,
        crushAttackAnim,
        PincerAttackttackAnim,
    }
    [Header("�p�^�������i�p�^���̓����͓G�̎d�l�����Q�Ƃ��Ă��������j")]
    [SerializeField] List<EnemyPatternSettings> Pattern1, Pattern2, Pattern3;
    public BossHPBar HPBar;

    //�����֐�
    //�U���p�^�����L�^����֐�
    int EnemyAnim = -1, EnemyPattern = -1, EnemyPatternPreb = -1, AnimationController = -1, JumpAttackAnimCtrl = -1;

    //�A�j���`�F�b�N�A�p�^�[���`�F�b�N
    bool NotInAnim = true, PatternOver = true, patternover = false, isFlameBracing = false, isSlewAttacking = false, isJumpingAttacking = false;

    protected override void Start()
    {
        base.Start();



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




    //�������Ȃ��֐�
    //Boss���S���ɌĂԊ֐�
    virtual public void Boss_Down()
    {
        ComboParam.Instance.ComboStop();
        GameManager.Instance.PlayerExAttack_Start();
        GameManager.Instance.Result_Start(3);
    }
}
