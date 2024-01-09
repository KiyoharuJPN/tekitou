using System.Collections;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class KingSlime : Enemy
{
    [Header("�ړ����鎞�̍����Ƌ���")]
    public float moveHeightForce, moveWidthForce, AttackHeight = 8;
    public GameObject[] summonSlime;
    public GameObject wallCheck;
    
    public BoxCollider2D attackCheckArea;
    public CircleCollider2D knockbackAttackCircle;
    public BossHPBar HPBar;

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

    [System.Serializable]
    public struct BossDownShake
    {
        [Tooltip("�h��̎���")]
        public float ShakeTime;
        [Tooltip("�h��̋���")]
        public float ShakePower;
        [Tooltip("�X�g�b�v�����i�K���h�ꎞ�Ԃ�菬�����悤�ɐݒ肵�Ă��������j")]
        public float StopTime;
        [Tooltip("�񕜑��x�i0.5��菬�����悤�ɐݒ肵�Ă��������j")]
        public float RecoverySpeed;
        //[Tooltip("�U����")]
        //public int ShakeNum;
        //[Tooltip("�h��̃����_����")]
        //public int ShakeRand;
    }
    [SerializeField]
    [Header("�{�X���|���ꂽ�Ƃ��̗h��")]
    public BossDownShake bossDownShake = new BossDownShake() { ShakeTime = 1f, ShakePower = 1.2f, StopTime = 0.3f, RecoverySpeed = 0.01f/*ShakeNum = 40, ShakeRand = 90*/ };

    float movingHeight, movingWidth, summonPosX, summonPosY;            //�ړ��Ɋւ�������֐�
    bool KSmovingCheck = true, KSattackingCheck = true, KSNormalAttackLanding = false
        , NoGravity = false, ExSkillCheck = false, inExSkillCheck = false
        , ExSkillFalling = false, SkillTurnAround = false;                                            //���f�p�����֐�
    int movingCheck = 0, AttackMode = 1, NormalAttackAnimation;         //�`�F�b�N�pint�֐�
    GameObject playerObj;                                               //�v���C���[�I�u�W�F�N�g�錾

    //�R�[���`�[���悤���f�֐�
    bool inKSBossAtack1, inKSBossAtack2, inKSBossAtack3, inKSBossSummon, inKSMovingAnim;

    protected override void Start()
    {
        playerObj = GameObject.Find("Hero");    //�v���C���[�I�u�W�F�N�g
        movingHeight = moveHeightForce;
        movingWidth = -moveWidthForce;
        summonPosX = -5f;
        summonPosY = 1;
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        base.Start();
    }

    protected override void Update()
    {
        //Ex�֌W�̃A�j���[�V����
        animator.SetBool("ExSkillFalling", ExSkillFalling);
        animator.SetBool("ExSkillCheck", ExSkillCheck);
        if (ExSkillCheck)
        {
            if (!inExSkillCheck)
            {
                inExSkillCheck = true;
                StopAllCoroutines();
                ClearCoroutines();
            }
            enemyRb.velocity = Vector2.zero;
            if (!isPlayerExAttack)
            {
                ExSkillCheck = false;
                StartCoroutine(SkillWait());
            }
            return;
        }
        if (isPlayerExAttack)
        {
            ExSkillCheck = true;
            enemyRb.velocity = Vector2.zero;
            return;
        }
        if (inExSkillCheck)
        {
            ExSkillFalling = enemyRb.velocity != Vector2.zero;
            if (ExSkillFalling)
            {
                return;
            }
            else
            {
                StartCoroutine(SkillWait());
            }
            
        }
        if (!IsBlowing)
        {
            //�U���֘A
            if (IsAttacking) KingSlimeAttack();
            //�ړ��֘A
            if (IsMoving && !inExSkillCheck)KingSlimeMoving();
        }
        
        //�A�j���[�V�����֐��̑��
        animator.SetBool("IsMoving", IsMoving);
        animator.SetInteger("AttackMode", AttackMode);
        animator.SetInteger("NormalAttackAnimation", NormalAttackAnimation);
        animator.SetBool("IsBlowing", isDestroy);
        if(IsMoving)animator.SetBool("IsAnimation",false);
    }

    //���������֘A�̊֐���������
    IEnumerator SkillWait()
    {
        yield return new WaitForSeconds(0.1f);
        inExSkillCheck = false;
    }
    //�L���O�X���C���̍U���p�֐�
    //�m�[�}���U���֐�
    void KingSlimeAttack()
    {
        if (KSattackingCheck)
        {
            KSattackingCheck = false;
            switch (AttackMode)
            {
                case 0:
                    StartCoroutine(KSBossAtack1());
                    break;
                case 1:
                    StartCoroutine(KSBossSummon());
                    break;
            }
        }
    }
    IEnumerator KSBossAtack1()
    {
        wallCheck.SetActive(false);
        inKSBossAtack1 = true;
        NormalAttackAnimation = 0;
        Vector3 PlayerPos = playerObj.transform.position + new Vector3(0, AttackHeight, 0);
        var AttackMoveSpeed = (float)Math.Sqrt(((PlayerPos.x - transform.position.x) * (PlayerPos.x - transform.position.x)) + (PlayerPos.y - transform.position.y) * (PlayerPos.y - transform.position.y));
        AttackMoveSpeed /= 25;
        if (playerObj.transform.position.x > gameObject.transform.position.x && movingWidth < 0) TurnAround();
        if (playerObj.transform.position.x < gameObject.transform.position.x && movingWidth > 0) TurnAround();
        yield return new WaitForSeconds(1f);
        //attackCheckArea.offset = new Vector2(0, 0f);
        //attackCheckArea.size = new Vector2(5.36f, 4.1f);
        //attackCheckArea.enabled = true;

        NoGravity = true;
        var i = 0;
        while(i < 25)
        {
            this.transform.position = Vector2.MoveTowards(transform.position, PlayerPos, AttackMoveSpeed);
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        enemyRb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.8f);
        NoGravity = false;
        gameObject.layer = LayerMask.NameToLayer("NoColliderEnemy");
        NormalAttackAnimation++;
        enemyRb.AddForce(new Vector2(0, -40),ForceMode2D.Impulse);
        knockbackAttackCircle.enabled = true;
        KSNormalAttackLanding = true;
        inKSBossAtack1 = false;
    }
    IEnumerator KSBossAtack2()
    {
        inKSBossAtack2 = true;
        SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
        shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        knockbackAttackCircle.enabled = false;
        NormalAttackAnimation++;
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        int i = 0;
        while (i < 35)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine(KSBossAtack3());
        inKSBossAtack2 = false;
    }
    IEnumerator KSBossAtack3()
    {
        SkillTurnAround = true;
        inKSBossAtack3 = true;
        NormalAttackAnimation++;

        yield return new WaitForSeconds(0.75f);

        IsAttacking = false;
        IsMoving = true;
        KSattackingCheck = true;
        AttackMode = 1;
        NormalAttackAnimation = 0;
        inKSBossAtack3 = false;
        SkillTurnAround = false;
        
    }
    //�����U���֐�
    IEnumerator KSBossSummon()
    {
        inKSBossSummon = true;
        SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeSummon);
        if (playerObj.transform.position.x > gameObject.transform.position.x && movingWidth < 0) TurnAround();
        if (playerObj.transform.position.x < gameObject.transform.position.x && movingWidth > 0) TurnAround();
        var summonPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2.5f, gameObject.transform.position.z);
        yield return new WaitForSeconds(0.333f);

        var Summonslm = GetSummonProbability();
        var newSlime1 = Instantiate(summonSlime[Summonslm], summonPos, Quaternion.identity);
        if (transform.localScale.x < 0) newSlime1.GetComponent<Slime>().SummonSlimeTurn();
        newSlime1.GetComponent<Slime>().SetIsMoving(false);
        newSlime1.GetComponent<Rigidbody2D>().AddForce(new Vector2(summonPosX, summonPosY),ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);

        Summonslm = GetSummonProbability();
        var newSlime2 = Instantiate(summonSlime[Summonslm], summonPos, Quaternion.identity);
        if (transform.localScale.x < 0) newSlime2.GetComponent<Slime>().SummonSlimeTurn();
        newSlime2.GetComponent<Slime>().SetIsMoving(false);
        newSlime2.GetComponent<Rigidbody2D>().AddForce(new Vector2(summonPosX + 1, summonPosY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);

        Summonslm = GetSummonProbability();
        var newSlime3 = Instantiate(summonSlime[Summonslm], summonPos, Quaternion.identity);
        if (transform.localScale.x < 0) newSlime3.GetComponent<Slime>().SummonSlimeTurn();
        newSlime3.GetComponent<Slime>().SetIsMoving(false);
        newSlime3.GetComponent<Rigidbody2D>().AddForce(new Vector2(summonPosX + 2, summonPosY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.76f);
        IsAttacking = false;
        IsMoving = true;
        KSattackingCheck = true;
        AttackMode = 0;
        inKSBossSummon = false;
    }


    //�ړ��p�֐�
    //�L���O�X���C���̈ړ�
    void KingSlimeMoving()
    {
        if (KSmovingCheck)
        {
            wallCheck.SetActive(true);
            KSmovingCheck = false;
            StartCoroutine(KSMovingAnim());
        }
    }
    IEnumerator KSMovingAnim()
    {
        inKSMovingAnim = true;
        movingCheck++;
        yield return new WaitForSeconds(0.5f);
        enemyRb.AddForce(new Vector2(movingWidth, movingHeight),ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.75f);
        if (movingCheck == 4)
        {
            movingCheck = 0;
            IsMoving = false;
            IsAttacking = true;
        }
        KSmovingCheck = true;
        inKSMovingAnim = false;
    }

    //Boss���S���ɌĂԊ֐�
    virtual public void Boss_Down()
    {
        ComboParam.Instance.ComboStop();
        GameManager.Instance.PlayerExAttack_Start();
        GameManager.Instance.Result_Start(1);
    }

    //�R���C�_�[��g���K�[�Ȃǂ̃`�F�b�N�֐�
    protected override void OnColEnter2D(Collider2D col)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            //if (col.gameObject.CompareTag("Stage") && KSNormalAttackLanding)
            //{
            //    KSNormalAttackLanding = false;
            //    StartCoroutine(KSBossAtack2());
            //}
            base.OnColEnter2D(col);
        }
    }
    new private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            if (collision.gameObject.CompareTag("Stage") && KSNormalAttackLanding)
            {
                KSNormalAttackLanding = false;
                StartCoroutine(KSBossAtack2());
            }
        }
    }

    protected override void OnColStay2D(Collider2D col)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            //if (col.gameObject.CompareTag("Stage") && KSNormalAttackLanding && enemyRb.velocity == Vector2.zero)
            //{
            //    KSNormalAttackLanding = false;
            //    StartCoroutine(KSBossAtack2());
            //}
            base.OnColStay2D(col);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (GetComponent<KingSlime>().enabled)
        {
            if (collision.gameObject.CompareTag("Stage") && KSNormalAttackLanding && enemyRb.velocity == Vector2.zero)
            {
                KSNormalAttackLanding = false;
                StartCoroutine(KSBossAtack2());
            }
        }
    }


    //�O���C���p�ϐ���������
    //�ǂɓ���������ړ��ʂ�ۂ����܂܉�]���s��s
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
        enemyRb.velocity = new Vector2(enemyRb.velocity.x * -1, enemyRb.velocity.y);
        movingWidth *= -1;
        summonPosX *= -1;
    }

    public void PlayerInAttackArea(Collider2D col)
    {
        if (!HadAttack)
        {
            //�U���N�[���_�E���^�C��
            HadAttack = true;
            StartCoroutine(HadAttackReset());
            //�_���[�W�ƃm�b�N�o�b�N
            col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 30 * enemyData.knockBackValue);
            col.gameObject.GetComponent<PlayerController>().Damage(enemyData.attackPower);
        }
    }
    public bool GetSkillTurnAround()
    {
        return SkillTurnAround;
    }
    public void SetSkillTurnAround(bool sta)
    {
        SkillTurnAround = sta;
    }


    //��ɓ������ǐG��Ȃ��Ă������R�[�h��������
    //��~��������
    public override void Stop_End()
    {
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
    }
    //�d�͊֘A
    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        if(!NoGravity)Gravity();
    }
    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }

    //�_���[�W�֘A
    protected override async void OnDestroyMode()
    {
        //�K�E�Z�q�b�g�G�t�F�N�g����
        BossCheckOnCamera = false;
        OnCamera = false;


        //OnDestroy���̗���
        isDestroy = true;
        IsBlowing = true;
        GameManager.Instance.PlayerStop();
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
        //BossDown���ʉ�
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);

        //GameManager.Instance.PlayerExAttack_Start();
        Time.timeScale = 0;
        await BossDownProcess();
    }
    public async UniTask BossDownProcess()
    {
        //BossDown��ʗh��
        shake.BossShake(bossDownShake.ShakeTime, bossDownShake.ShakePower, true, true);
        await UniTask.Delay(TimeSpan.FromSeconds(bossDownShake.StopTime), ignoreTimeScale: true);
        int i = (int)((bossDownShake.ShakeTime - bossDownShake.StopTime) / bossDownShake.RecoverySpeed);
        Debug.Log(i);
        while (i > 0)
        {
            Time.timeScale += 1/i;
            i--;
            await UniTask.Delay(TimeSpan.FromSeconds(bossDownShake.RecoverySpeed), ignoreTimeScale: true);
        }
        if (Time.timeScale != 1) Time.timeScale = 1;
        //GameManager.Instance.PlayerExAttack_End();


        //    Vector3 initialPos = transform.position;//�����ʒu�ۑ�
        //    Time.timeScale = 0;

        //    //BossDown���ʉ�
        //    SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
        //    //�q�b�g�X�g�b�v�����J�n
        //    await transform.DOShakePosition(bossDownShake.ShakeTime, bossDownShake.ShakePower, bossDownShake.ShakeNum, bossDownShake.ShakeRand)
        //        .SetUpdate(true)
        //        .SetLink(gameObject)
        //        .OnComplete(() =>
        //        {
        //            //�����̈ʒu�ɖ߂�
        //            transform.position = initialPos;
        //        });
        //    //�A�j���[�V�������I�������玞�Ԃ�߂�
        //    if (!GameManager.Instance.PauseCheck)
        //    {
        //        Time.timeScale = 1;
        //    }

        //    //OnDestroy���̗���
        //    isDestroy = true;
        //    IsBlowing = true;
        //    GameManager.Instance.PlayerStop();
        //    gameObject.layer = LayerMask.NameToLayer("DeadBoss");
        //    gameObject.GetComponent<BoxCollider2D>().enabled = false;
        //    gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DeadBoss")) return;
        //�_���[�W����
        DamegeProcess(power, skill, isHitStop, exSkill);
    }

    protected override async void DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
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
            await EnemyGeneratar.instance.HitStopProcess(power, this.transform);
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

        if (hp <= 0 && !isDestroy)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            OnDestroyMode();
        }
    }

    //�C�����Ȃ��֐�
    void ClearCoroutines()
    {
        if (inKSBossAtack1)
        {
            inKSBossAtack1 = false;
            NoGravity = false;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            knockbackAttackCircle.enabled = false;
            attackCheckArea.enabled = false;
            KSNormalAttackLanding = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 1;
            wallCheck.SetActive(true);
        }
        if(NormalAttackAnimation == 1)
        {
            inKSBossAtack1 = false;
            NoGravity = false;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            knockbackAttackCircle.enabled = false;
            attackCheckArea.enabled = false;
            KSNormalAttackLanding = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 1;
            wallCheck.SetActive(true);
        }
        if (inKSBossAtack2)
        {
            inKSBossAtack2 = false;
            NormalAttackAnimation = 0;
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            knockbackAttackCircle.enabled = false;
            KSNormalAttackLanding = false;
            attackCheckArea.enabled = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 1;
            wallCheck.SetActive(true);
        }
        if (inKSBossAtack3)
        {
            inKSBossAtack3 = false;
            NormalAttackAnimation = 0;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            knockbackAttackCircle.enabled = false;
            attackCheckArea.enabled = false;
            AttackMode = 1;
            wallCheck.SetActive(true);
            SkillTurnAround = false;
        }
        if (inKSBossSummon)
        {
            inKSBossSummon = false;
            IsAttacking = false;
            IsMoving = true;
            KSattackingCheck = true;
            AttackMode = 1;
        }
        if (inKSMovingAnim)
        {
            movingCheck = 0;
            KSmovingCheck = true;
        }
        DefaultColor();
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
}
