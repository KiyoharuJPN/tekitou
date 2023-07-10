using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Enemy
{
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
    [SerializeField] List<EnemyPatternSettings> Pattern1,Pattern2,Pattern3;
    public BossHPBar HPBar;
    //�U���p�^�����L�^����֐�
    int EnemyAnim = -1, EnemyPattern = -1, EnemyPatternPreb = -1, AnimationController = -1;

    //�A�j���`�F�b�N�A�p�^�[���`�F�b�N
    bool NotInAnim = true, PatternOver = true, patternover = false, isFlameBracing =false;

    BoxCollider2D EnemyCollider;
    protected override void Start()
    {
        base.Start();
        moveSpeed = MoveSpeed * -1;
        EnemyCollider = GetComponent<BoxCollider2D>();
    }
    //�h���S���̓���
    protected override void Update()
    {
        base.Update();
        //animator�̐ݒ�
        animator.SetBool("IsBlowing", isDestroy);

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
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

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
        Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");

        animator.SetInteger("AnimationController", -1);
        NotInAnim = true;
        if (patternover)
        {
            PatternOver = patternover;
            patternover = false;
            //Debug.Log("Pattern Over");
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
            Debug.Log("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
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
        ResetAttackCheckArea();
        dragonAttackCheckArea.gameObject.SetActive(true);
        while (animcheck < 79)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea1();
        while (animcheck < 86)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea2();
        while (animcheck < 93)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea3();
        while (animcheck < 100)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea4();
        while (animcheck < 107)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea5();
        while (animcheck < 114)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea1();
        while (animcheck < 121)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea2();
        while (animcheck < 128)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea3();
        while (animcheck < 135)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea4();
        while (animcheck < 142)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea5();
        while (animcheck < 149)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea1();
        while (animcheck < 156)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea2();
        while (animcheck < 163)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea3();
        while (animcheck < 170)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea4();
        while (animcheck < 177)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckArea5();
        while (animcheck < 177)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        AttackCheckAreaOver();
        while (animcheck < 184)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        dragonAttackCheckArea.gameObject.SetActive(false);
        while (animcheck < 210)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }

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

        //�U���͂ƃm�b�N�o�b�N��ς���
        var attackdam = enemyData.power;
        enemyData.power = 2;
        var knockbackval = enemyData.knockBackValue;
        enemyData.knockBackValue = 5;

        float animcheck = 0;
        while(animcheck < 15)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        EnemyCollider.offset = new Vector2(1.45f, -1.2f);
        EnemyCollider.size = new Vector2(9.6f, 4);
        while(animcheck < 23)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        EnemyCollider.offset = new Vector2(-3.9f, -1.2f);
        EnemyCollider.size = new Vector2(10, 4);
        while (animcheck < 31)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        EnemyCollider.size = new Vector2(11.3f, 4);
        while (animcheck < 39)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        ResetBoxCollider2D();
        while (animcheck < 65)
        {
            animcheck++;
            yield return new WaitForSeconds(0.01f);
        }
        //�U���͂ƃm�b�N�o�b�N��߂�
        enemyData.power = attackdam;
        enemyData.knockBackValue = knockbackval;
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


    //�O���֐�
    public override void Damage(float power)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
        hp -= power;
        //HP�Q�[�W���g�p���Ă��邩�ǂ���
        if (HPBar != null)
        {
            HPBar.ReductionHP();
        }
        else
        {
            Debug.Log("HPBar�͂܂�����ĂȂ��ł��B����HPBar�t���Ŏ��������ꍇ��HPBar��t���Ă��玎���Ă��������B");
        }

        ComboParam.Instance.ResetTime();
        if (!hadDamaged)
        {
            StartCoroutine(HadDamaged());
            hadDamaged = true;
        }
        if (hp <= 0)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            Destroy();
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
                col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * 3);
                col.gameObject.GetComponent<PlayerController>()._Damage(2);
            }
            
            
        }
    }



    //�����֐�
    protected override void Destroy()
    {
        GameManager.Instance.AddKillEnemy();
        this.GetComponent<BoxCollider2D>().enabled = false;
        isDestroy = true;
        //���̏o��������Ŏ�������Ƃ��ɂ����ɏ����Ύ����ł���
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
        dragonAttackCheckArea.offset = new Vector2(-8, -1.25f);
        dragonAttackCheckArea.size = new Vector2(7.1f, 3.9f);
    }
    void AttackCheckArea1()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.6f, -0.85f);
        dragonAttackCheckArea.size = new Vector2(8.2f, 4.7f);
    }
    void AttackCheckArea2()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.6f, -0.7f);
        dragonAttackCheckArea.size = new Vector2(8.2f, 5f);
    }
    void AttackCheckArea3()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.6f, -0.35f);
        dragonAttackCheckArea.size = new Vector2(8.5f, 5.7f);
    }
    void AttackCheckArea4()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.6f, -0.1f);
        dragonAttackCheckArea.size = new Vector2(8.5f, 6.2f);
    }
    void AttackCheckArea5()
    {
        dragonAttackCheckArea.offset = new Vector2(-8.5f, -0.1f);
        dragonAttackCheckArea.size = new Vector2(8.25f, 6.2f);
    }
    void AttackCheckAreaOver()
    {
        dragonAttackCheckArea.offset = new Vector2(-9f, -1f);
        dragonAttackCheckArea.size = new Vector2(6.75f, 4.45f);
    }


}
