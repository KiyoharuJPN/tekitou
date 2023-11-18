using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : Enemy
{
    DemonKing demonKing;
    Renderer spritehand;

    protected override void Start()
    {
        demonKing = GetComponentInParent<DemonKing>();
        //�G�̓_��
        spritehand = GetComponent<Renderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (demonKing != null)
        {
            demonKing.OnHandTriggerEnter2D(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(demonKing != null)
        {
            demonKing.OnHandTriggerStay2D(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (demonKing != null)
        {
            demonKing.OnHandTriggerExit2D(collision);
        }
    }

    private void StopHandAnimation()
    {
        GetComponent<Animator>().speed = 0;
    }

    //protected virtual void Start()
    //{
    //    //id�Ŏw�肵���G�f�[�^�Ǎ�
    //    enemyData = EnemyGeneratar.instance.EnemySet(id);
    //    hp = enemyData.hp;
    //    enemyRb = GetComponent<Rigidbody2D>();
    //    maxReflexNum = enemyData.num;
    //    reflexNum = maxReflexNum;
    //    forceAngle = enemyData.angle;

    //    animator = GetComponent<Animator>();

    //    //������ђ��Ɏg�p
    //    _transform = transform;
    //    _prevPosition = _transform.position;
    //    speed = enemyData.speed;
    //    smokeEffect = EnemyGeneratar.instance.smokeEffect;
    //    effectInterval = EnemyGeneratar.instance.effectInterval;

    //    //���Ŏ��Ɏg�p
    //    deathEffect = EnemyGeneratar.instance.deathEffect;

    //    //�G�̓_��
    //    sprite = GetComponent<SpriteRenderer>();

    //    //Buff�֘A
    //    if (GetComponentInChildren<EnemyBuffSystem>())
    //    {
    //        _EnemyBuff = GetComponentInChildren<EnemyBuffSystem>();
    //        hadEnemyBuff = true;
    //    }

    //    stopState = EnemyGeneratar.instance.stopState;
    //}

    override protected void OnColEnter2D(Collider2D col)
    {

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {

    }

    override protected void OnColStay2D(Collider2D col)
    {

    }

    override protected void Update()
    {

    }

    override protected void FixedUpdate()
    {

    }

    ////�U��
    //protected void Attack(Collider2D col)
    //{
    //    if (!HadAttack)
    //    {
    //        //�ꎞ�g�p��~
    //        //�U���N�[���_�E���^�C��
    //        //HadAttack = true;
    //        //StartCoroutine(HadAttackReset());

    //        //�_���[�W�ƃm�b�N�o�b�N
    //        col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 15 * enemyData.knockBackValue);
    //        col.gameObject.GetComponent<PlayerController>()._Damage((int)enemyData.power);
    //    }
    //}

    //�_���[�W�����ďo��
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        demonKing.Damage(power, skill, isHitStop, exSkill);
        StartCoroutine(DamegeProcess(power, skill, isHitStop, exSkill));
    }

    //�_���[�W�����i�q�b�g�X�g�b�v�̊֌W�ŃR���[�`���ɕύX�j
    protected override IEnumerator DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
    {
        //�q�b�g�G�t�F�N�g����
        if (skill != null)
        {
            HitEfect(this.transform, skill.hitEffectAngle);
        }
        else HitEfect(this.transform, UnityEngine.Random.Range(0, 360));


        //�q�b�g�����o�i�G�_�Łj
        if (!hadDamaged)
        {
            StartCoroutine(HadDamagedHand());
            hadDamaged = true;
        }

        yield return null;
    }

    //protected void EnemyMove()
    //{

    //}

    //���S������
    protected override void OnDestroyMode()
    {

    }

    ////������я���
    //protected void BlownAway()
    //{
    //    //������ъJ�n
    //    CalcForceDirection(); //������ѕ����v�Z
    //    BoostSphere();        //velocity�t�^
    //}

    ////������є���
    //protected void BoostSphere()
    //{
    //    // �����Ɨ͂̌v�Z
    //    Vector2 force = speed * forceDirection;

    //    // �͂������郁�\�b�h
    //    enemyRb.velocity = force;
    //}
    //public void BuffBoostSphere()
    //{
    //    if (_EnemyBuff != null)
    //    {
    //        // �����Ɨ͂̌v�Z
    //        Vector2 force = (speed + BuffBlowingSpeed()) * forceDirection;

    //        // �͂������郁�\�b�h
    //        enemyRb.velocity = force;
    //    }
    //}

    ////������ђ���]
    //private void EnemyRotate()
    //{
    //    //�E�����ɓ����Ă���
    //    if (enemyRb.velocity.x > 0.1)
    //    {
    //        this.transform.Rotate(0, 0, -rotateSpeed);
    //    }
    //    //�������ɓ����Ă���
    //    else if (enemyRb.velocity.x < -0.1)
    //    {
    //        this.transform.Rotate(0, 0, rotateSpeed);
    //    }
    //}

    ////������ђ��̃G�t�F�N�g����
    //private IEnumerator BlowAwayEffect()
    //{
    //    yield return new WaitForSeconds(effectInterval);
    //    GameObject obj = Instantiate(smokeEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);
    //    yield return new WaitForSeconds(0.25f);
    //    Destroy(obj);
    //    StartCoroutine(BlowAwayEffect());
    //}

    //protected void CalcForceDirection()
    //{
    //    // ���͂��ꂽ�p�x�����W�A���ɕϊ�
    //    float rad = forceAngle * Mathf.Deg2Rad;

    //    //�I�u�W�F�N�g���擾
    //    player = serchTag(gameObject, "Player");
    //    if (player == null)
    //    {
    //        player = serchTag(gameObject, "InvinciblePlayer");
    //    }

    //    // ���ꂼ��̎��̐������v�Z
    //    float x = Mathf.Cos(rad);
    //    float y = Mathf.Sin(rad);

    //    //�v���C���[�Ǝ��g�̈ʒu�֌W�𒲍�
    //    if (enemyData.type == EnemyData.EnemyType.FlyEnemy || isDestroy)
    //    {
    //        if (player.transform.position.y + 0.3f < this.transform.position.y)
    //        { y = -y; }
    //    }
    //    if (player.transform.position.x > this.transform.position.x)
    //    { x = -x; }

    //    // Vector3�^�Ɋi�[
    //    forceDirection = new Vector2(x, y);
    //}

    ////�w�肳�ꂽ�^�O�̒��ōł��߂����̂��擾
    //protected GameObject serchTag(GameObject nowObj, string tagName)
    //{
    //    float tmpDis = 0;           //�����p�ꎞ�ϐ�
    //    float nearDis = 0;          //�ł��߂��I�u�W�F�N�g�̋���
    //    //string nearObjName = "";    //�I�u�W�F�N�g����
    //    GameObject targetObj = null; //�I�u�W�F�N�g

    //    //�^�O�w�肳�ꂽ�I�u�W�F�N�g��z��Ŏ擾����
    //    foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
    //    {
    //        //���g�Ǝ擾�����I�u�W�F�N�g�̋������擾
    //        tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

    //        //�I�u�W�F�N�g�̋������߂����A����0�ł���΃I�u�W�F�N�g�����擾
    //        //�ꎞ�ϐ��ɋ������i�[
    //        if (nearDis == 0 || nearDis > tmpDis)
    //        {
    //            nearDis = tmpDis;
    //            //nearObjName = obs.name;
    //            targetObj = obs;
    //        }

    //    }
    //    //�ł��߂������I�u�W�F�N�g��Ԃ�
    //    //return GameObject.Find(nearObjName);
    //    return targetObj;
    //}

    ////��ʂɓ������ǂ������`�F�b�N
    //protected void OnBecameVisible()
    //{
    //    OnCamera = true;
    //}
    //protected void OnBecameInvisible()
    //{
    //    OnCamera = false;
    //}

    ////�ړ������̉�]
    //public override void TurnAround()
    //{
    //    bool InCheck = true;
    //    if (transform.localScale.x == 1f && InCheck)
    //    {
    //        transform.localScale = new Vector3(-1f, 1f, 1f);
    //        InCheck = false;
    //    }
    //    if (transform.localScale.x == -1f && InCheck)
    //    {
    //        transform.localScale = new Vector3(1f, 1f, 1f);
    //        //InCheck = false;
    //    }
    //    moveSpeed *= -1;
    //}

    ////�O���獡�̈ړ���Ԃ��m�F
    //public bool GetIsMoving()
    //{
    //    return IsMoving;
    //}

    ////�O���獡�̐�����΂���Ԋm�F
    //public bool GetIsBlowing()
    //{
    //    return isDestroy;
    //}

    ////�U���͂��O�Ŏ擾����
    //public int GetDamage()
    //{
    //    return enemyData.attackPower;
    //}

    ////�m�b�N�o�b�N�͂��O�Ŏ擾����/
    //public float GetKnockBackForce()
    //{
    //    return enemyData.knockBackValue;
    //}

    //�v���C���[���U���G���A�ɗv�鎞�̓����iAttackCheckArea����Ă΂��j
    public override bool PlayerInAttackArea()
    {
        var InAttack = false;
        ////true�̏C���͊e�X�N���v�g�ŏ����Ă��������B
        //if (IsMoving && AttackChecking)
        //{
        //    AttackChecking = false;
        //    //�R���[�`���֐��������ō��܂�
        //    InAttack = true;
        //}
        return InAttack;
    }
    //�U�����ꂽ�ǂ������`�F�b�N
    public override bool GetPlayerAttacked()
    {
        //true�̏C���͊e�X�N���v�g�ŏ����Ă��������B
        return PlayerNotAttacked;
    }
    public override void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }

    ////�U���N�[���_�E��
    //protected IEnumerator HadAttackReset()
    //{
    //    var n = 100;
    //    while (n > 0)
    //    {
    //        n--;
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //    HadAttack = false;
    //}

    protected IEnumerator HadDamagedHand()
    {
        spritehand.material.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        spritehand.material.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        spritehand.material.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        spritehand.material.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        spritehand.material.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        spritehand.material.color = new Color(1, 1, 1);
        hadDamaged = false;
    }

    //protected void DefaultColor()
    //{
    //    sprite.color = new Color(1, 1, 1);
    //}

    //�d��
    protected override void Gravity()
    {
        
    }

    //�G��~����
    public override void EnemyStop()
    {
        demonKing.EnemyStop();
        if (enemyRb != null)
        {
            isPlayerExAttack = true;
            enemyRb.velocity = Vector2.zero;
        }
        if (animator != null)
        {
            animator.speed = 0;
        }
    }

    //�K�E�Z���������Ă����ꍇ�̃_���[�W�����ďo��
    public override void PlaeyrExAttack_HitEnemyEnd(float power)
    {
        demonKing.PlaeyrExAttack_HitEnemyEnd(power);
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
        Damage(power, null, true, true);
    }

    //��~��������
    public override void Stop_End()
    {
        demonKing.Stop_End();
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
    }






    ////HP��FullHP�̊l��
    //public float GetEnemyHP()
    //{
    //    return hp;
    //}
    //public float GetEnemyFullHP()
    //{
    //    return enemyData.hp;
    //}

    //public void OnColEnter(Collider2D col)
    //{
    //    OnColEnter2D(col);
    //}

    //public void OnColStay(Collider2D col)
    //{
    //    OnColStay2D(col);
    //}

    ////�ʏ�폜
    //public void EnemyNomalDestroy()
    //{
    //    SoundManager.Instance.PlaySE(SESoundData.SE.MonsterDead);
    //    GameObject obj = Instantiate(deathEffect, new Vector2(enemyRb.position.x, enemyRb.position.y), Quaternion.identity);

    //    if (_EnemyBuff) _EnemyBuff._Destroy();

    //    if (tween != null)
    //    {
    //        tween.Kill();
    //        //�A�j���[�V�������I�������玞�Ԃ�߂�
    //        Time.timeScale = 1;
    //    }
    //    Destroy(gameObject);
    //}

    ////�q�b�g�G�t�F�N�g����
    //internal void HitEfect(Transform enemy, int angle)
    //{
    //    GameObject prefab =
    //    Instantiate(GameManager.Instance.hitEffect, new Vector2(enemy.position.x, enemy.position.y), Quaternion.identity);
    //    prefab.transform.Rotate(new Vector3(0, 0, angle));
    //    SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
    //    _EfectDestroy(prefab, 0.2f);
    //}
    ////�G�t�F�N�g�폜
    //void _EfectDestroy(GameObject prefab, float time)
    //{
    //    Destroy(prefab, time);
    //}
}
