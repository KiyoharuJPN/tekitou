using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Enemy : MonoBehaviour
{
    protected Animator animator;

    [SerializeField]
    protected GameObject EnemyColliderArea;
    [SerializeField]
    protected string id;
    protected EnemyData enemyData;
    protected Rigidbody2D enemyRb;

    //�ړ����x�����֐�
    protected float moveSpeed;
    //�`�F�b�N�p�����֐�
    protected bool IsBlowing = false, IsMoving = true, IsAttacking = false, hadDamaged = false, PlayerNotAttacked = true, HadAttack = false, AttackChecking = true;

    //�v���C���[�K�E�Z�����ǂ���
    [System.NonSerialized]
    public bool isPlayerExAttack;
    public bool HadContactDamage = true;

    protected float hp;

    //������ъp�x
    protected float forceAngle;
    protected Vector2 forceDirection = new Vector3(1.0f, 1.0f);
    [SerializeField, Header("������ё��x")]
    protected float speed = 15f;     //������ё��x
    //������ђ��̉��G�t�F�N�g
    private GameObject smokeEffect;
    //���Ŏ��G�t�F�N�g
    private GameObject deathEffect;
    private float effectInterval = 0.5f;
    protected float rotateSpeed = 10f;//������щ�]���x

    //���ˉ�
    int maxReflexNum;
    internal int reflexNum;

    protected GameObject player;

    //�G�̓_��
    SpriteRenderer sprite;
    protected enum moveType
    {
        NotMove, //�����Ȃ�
        Move,    //����
        FlyMove�@//���
    }
    protected moveType type;

    internal bool isDestroy = false;
    internal bool OnCamera = false;

    protected Transform _transform;

    // �O�t���[���̃��[���h�ʒu
    protected Vector2 _prevPosition;


    //Buff�֘A
    EnemyBuffSystem _EnemyBuff;

    protected virtual void Start()
    {
        //id�Ŏw�肵���G�f�[�^�Ǎ�
        enemyData = EnemyGeneratar.instance.EnemySet(id);
        hp = enemyData.hp;
        enemyRb = GetComponent<Rigidbody2D>();
        maxReflexNum = enemyData.num;
        reflexNum = maxReflexNum;
        forceAngle = enemyData.angle;

        animator = GetComponent<Animator>();

        //������ђ��Ɏg�p
        _transform = transform;
        _prevPosition = _transform.position;
        speed = EnemyGeneratar.instance.speed;
        smokeEffect = EnemyGeneratar.instance.smokeEffect;
        effectInterval = EnemyGeneratar.instance.effectInterval;

        //���Ŏ��Ɏg�p
        deathEffect = EnemyGeneratar.instance.deathEffect;

        //�G�̓_��
        sprite = GetComponent<SpriteRenderer>();

        //Buff�֘A
        if (GetComponentInChildren<EnemyBuffSystem>())
        {
            _EnemyBuff = GetComponentInChildren<EnemyBuffSystem>();
        }
    }

    virtual protected void OnColEnter2D(Collider2D col)
    {

        if (!isDestroy && HadContactDamage)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Attack(col);
            }
        }
        if (col.gameObject.CompareTag("Stage") && isDestroy)
        {
            reflexNum--;
            Debug.Log(reflexNum);
            if (reflexNum == 0)
            {
                SoundManager.Instance.PlaySE(SESoundData.SE.MonsterDead);
                GameObject obj = Instantiate(deathEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);

                if (_EnemyBuff) _EnemyBuff._Destroy();
                Destroy(gameObject);
            }
        }
    }

    virtual protected void OnColStay2D(Collider2D col)
    {
        if (!isDestroy)
        {
            if (col.gameObject.CompareTag("Player")&&!HadAttack)
            {
                Attack(col);
            }
            //if (col.gameObject.CompareTag("Player"))
            //{
            //    enemyRb.velocity = new Vector2(0, enemyRb.velocity.y);
            //}
        }
    }

    virtual protected void Update()
    {
        if (isPlayerExAttack)
        {
            this.transform.Rotate(0, 0, 0);
            if (enemyRb.bodyType != RigidbodyType2D.Static)
            {
                this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            return;
        }

        //������ђ��ȊO�͍s��Ȃ�
        if (!isDestroy)
            return;

        //������ђ��̏���
        if (isDestroy)
        {
            
            EnemyRotate();
        }
    }

    virtual protected void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        Gravity();
    }

    //�U��
    protected void Attack(Collider2D col)
    {
        if (!HadAttack)
        {
            //�ꎞ�g�p��~
            //�U���N�[���_�E���^�C��
            //HadAttack = true;
            //StartCoroutine(HadAttackReset());

            //�_���[�W�ƃm�b�N�o�b�N
            col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 15 * enemyData.knockBackValue);
            col.gameObject.GetComponent<PlayerController>()._Damage((int)enemyData.power);
        }
    }

    public virtual void Damage(float power, Skill skill)
    {
        //���Ɏ��S��Ԃ̏ꍇ
        if (isDestroy)
        {
            if (_EnemyBuff != null)
            {
                if(_EnemyBuff.GetBuffAttackCheckCount() > 0)
                {
                    _EnemyBuff.ShowAttackChecking();
                    SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
                    ComboParam.Instance.ResetTime();
                    reflexNum = maxReflexNum;
                    CalcForceDirection();
                    BuffBoostSphere();
                }
                else
                {
                    SoundManager.Instance.PlaySE(SESoundData.SE.MonsterDead);
                    GameObject obj = Instantiate(_EnemyBuff.GetBuffEffect(), new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);
                    obj.GetComponent<SpriteRenderer>().color = _EnemyBuff.GetColorByType();
                    GameManager.Instance.SetBuff((int)_EnemyBuff.GetBuffType());
                    _EnemyBuff.ShowAttackChecking();

                    Destroy(gameObject);
                }
            }
            else
            {
                SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
                ComboParam.Instance.ResetTime();
                reflexNum = maxReflexNum;
                CalcForceDirection();
                BoostSphere();

            }
            return;
        }

        //�q�b�g�X�g�b�v�Ƀ_���[�W�������s���Ă��炤
        StartCoroutine(HitStop(power, skill));
    }

    IEnumerator HitStop(float power, Skill skill)
    {
        
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
        ComboParam.Instance.ResetTime();

        if(skill != null)
        {
            HitEfect(this.transform, skill.hitEffectAngle);
        }
        else HitEfect(this.transform, UnityEngine.Random.Range(0, 360));

        //�q�b�g�X�g�b�v����
        Vector3 initialPos = this.transform.position;//�����ʒu�ۑ�
        Time.timeScale = 0;

        yield return transform.DOShakePosition(power * 0.1f, 0.8f, 30, 90)
            .SetUpdate(true)
            .OnComplete(() => 
            {
                //�A�j���[�V�������I�������玞�Ԃ�߂�
                Time.timeScale = 1;
                //�����ʒu�ɖ߂�
                this.transform.position = initialPos;
            });


        hp -= power;
        
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

    protected void EnemyMove()
    {
        
    }

    protected virtual void Destroy()
    {
        GameManager.Instance.AddKillEnemy();
        //���˗p�̃R���C�_�[�ɕύX
        this.GetComponent<BoxCollider2D>().enabled = false;
        this.GetComponent<CircleCollider2D>().enabled = true;
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        enemyRb.gravityScale = 0;
        enemyRb.constraints = RigidbodyConstraints2D.None;
        CalcForceDirection();
        //������ъJ�n
        BoostSphere();
        isDestroy = true;
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterKnock);
        gameObject.layer = LayerMask.NameToLayer("PinBallEnemy");
        
        if(_EnemyBuff != null)
            _EnemyBuff.ShowAttackChecking();

        if(smokeEffect != null)
            StartCoroutine(BlowAwayEffect());
    }

    //������є���
    protected void BoostSphere()
    {
        // �����Ɨ͂̌v�Z
        Vector2 force = speed * forceDirection;

        // �͂������郁�\�b�h
        enemyRb.velocity = force;
    }
    public void BuffBoostSphere()
    {
        if(_EnemyBuff != null)
        {
            // �����Ɨ͂̌v�Z
            Vector2 force = (speed + _EnemyBuff.GetBuffBlowingSpeed()) * forceDirection;
            Debug.Log(_EnemyBuff.GetBuffBlowingSpeed());
            // �͂������郁�\�b�h
            enemyRb.velocity = force;
        }
    }

    //������ђ���]
    private void EnemyRotate()
    {
        //�E�����ɓ����Ă���
        if (enemyRb.velocity.x > 0.1)
        {
            this.transform.Rotate(0, 0, -rotateSpeed);
        }
        //�������ɓ����Ă���
        else if (enemyRb.velocity.x < -0.1)
        {
            this.transform.Rotate(0, 0, rotateSpeed);
        }
    }

    //������ђ��̃G�t�F�N�g����
    private IEnumerator BlowAwayEffect()
    {
        yield return new WaitForSeconds(effectInterval);
        GameObject obj =  Instantiate(smokeEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);
        StartCoroutine(BlowAwayEffect());
        yield return new WaitForSeconds(0.25f);
        Destroy(obj);
    }

    protected void CalcForceDirection()
    {
        // ���͂��ꂽ�p�x�����W�A���ɕϊ�
        float rad = forceAngle * Mathf.Deg2Rad;

        //�I�u�W�F�N�g���擾
        player = serchTag(gameObject, "Player");

        // ���ꂼ��̎��̐������v�Z
        float x = Mathf.Cos(rad);
        float y = Mathf.Sin(rad);

        //�v���C���[�Ǝ��g�̈ʒu�֌W�𒲍�
        if (enemyData.type == EnemyData.EnemyType.FlyEnemy || isDestroy)
        {
            if (player.transform.position.y + 0.3f < this.transform.position.y)
            { y = -y; }
        }
        if(player.transform.position.x > this.transform.position.x) 
        { x = -x; }
        
        // Vector3�^�Ɋi�[
        forceDirection = new Vector2(x, y);
    }

    //�w�肳�ꂽ�^�O�̒��ōł��߂����̂��擾
    protected GameObject serchTag(GameObject nowObj, string tagName)
    {
        float tmpDis = 0;           //�����p�ꎞ�ϐ�
        float nearDis = 0;          //�ł��߂��I�u�W�F�N�g�̋���
        //string nearObjName = "";    //�I�u�W�F�N�g����
        GameObject targetObj = null; //�I�u�W�F�N�g

        //�^�O�w�肳�ꂽ�I�u�W�F�N�g��z��Ŏ擾����
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //���g�Ǝ擾�����I�u�W�F�N�g�̋������擾
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //�I�u�W�F�N�g�̋������߂����A����0�ł���΃I�u�W�F�N�g�����擾
            //�ꎞ�ϐ��ɋ������i�[
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                //nearObjName = obs.name;
                targetObj = obs;
            }

        }
        //�ł��߂������I�u�W�F�N�g��Ԃ�
        //return GameObject.Find(nearObjName);
        return targetObj;
    }
    //��ʂɓ������ǂ������`�F�b�N
    protected void OnBecameVisible()
    {
        OnCamera = true;
    }
    protected void OnBecameInvisible()
    {
        OnCamera = false;
    }

    //�ړ������̉�]
    public virtual void TurnAround()
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

    //�O���獡�̈ړ���Ԃ��m�F
    public bool GetIsMoving()
    {
        return IsMoving;
    }

    //�O���獡�̐�����΂���Ԋm�F
    public bool GetIsBlowing()
    {
        return isDestroy;
    }

    //�U���͂��O�Ŏ擾����
    public int GetDamage()
    {
        return enemyData.attackPower;
    }

    //�m�b�N�o�b�N�͂��O�Ŏ擾����/
    public float GetKnockBackForce()
    {
        return enemyData.knockBackValue;
    }

    //�v���C���[���U���G���A�ɗv�鎞�̓����iAttackCheckArea����Ă΂��j
    public virtual bool PlayerInAttackArea()
    {
        var InAttack = false;
        //true�̏C���͊e�X�N���v�g�ŏ����Ă��������B
        if (IsMoving && AttackChecking)
        {
            AttackChecking = false;
            //�R���[�`���֐��������ō��܂�
            InAttack = true;
        }
        return InAttack;
    }
    //�U�����ꂽ�ǂ������`�F�b�N
    public virtual bool GetPlayerAttacked()
    {
        //true�̏C���͊e�X�N���v�g�ŏ����Ă��������B
        return PlayerNotAttacked;
    }
    public virtual void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }

    //�U���N�[���_�E��
    protected IEnumerator HadAttackReset()
    {
        var n = 100;
        while(n > 0)
        {
            n--;
            yield return new WaitForSeconds(0.01f);
        }
        HadAttack = false;
    }

    protected IEnumerator HadDamaged()
    {
        sprite.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        sprite.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        sprite.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        sprite.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        sprite.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        sprite.color = new Color(1, 1, 1);
        hadDamaged = false;
    }


    //�d��
    protected virtual void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -5));
    }

    public virtual void EnemyStop() 
    {
        isPlayerExAttack = true;
        enemyRb.velocity = Vector2.zero;
        if(animator != null)
        {
            animator.speed = 0;
        }
    }

    //�K�E�Z���������Ă����ꍇ
    public virtual void PlaeyrExAttack_HitEnemyEnd(float powar)
    {
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
        Damage(powar, null);
    }

    //��~��������
    public virtual void Stop_End()
    {
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
    }

    //HP��FullHP�̊l��
    public float GetEnemyHP()
    {
        return hp;
    }
    public float GetEnemyFullHP()
    {
        return enemyData.hp;
    }

    public void OnColEnter(Collider2D col)
    {
        OnColEnter2D(col);
    }

    public void OnColStay(Collider2D col)
    {
        OnColStay2D(col);
    }

    //�q�b�g�G�t�F�N�g����
    internal void HitEfect(Transform enemy, int angle)
    {
        GameObject prefab =
        Instantiate(GameManager.Instance.hitEffect, new Vector2(enemy.position.x, enemy.position.y), Quaternion.identity);
        prefab.transform.Rotate(new Vector3(0, 0, angle));
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
        _EfectDestroy(prefab, 0.2f);
    }
    //�G�t�F�N�g�폜
    void _EfectDestroy(GameObject prefab, float time)
    {
        Destroy(prefab, time);
    }

}
