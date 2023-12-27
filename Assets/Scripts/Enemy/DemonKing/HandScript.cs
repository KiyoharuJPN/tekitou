using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : Enemy
{
    DemonKing demonKing;
    Renderer spritehand;
    public bool isLeftHand;

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

    //�����֘A
    void StopHandAnimation()
    {
        GetComponent<Animator>().speed = 0;
    }
    void SetSummonOffset()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + 2.55f);
    }
    void SetPincerAttackOffset()
    {
        if (demonKing.GetSkillAnimationController() == 0)
        {
            if (isLeftHand) transform.position = new Vector2(transform.position.x - 3.1f, transform.position.y);
            else transform.position = new Vector2(transform.position.x + 3.1f, transform.position.y);
        }
    }

    public void DemonDead()
    {
        isDestroy = true;
        //�K�E�Z�q�b�g�G�t�F�N�g����
        BossCheckOnCamera = false;
        OnCamera = false;
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
    }

    //�_���[�W�����ďo��
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DeadBoss")) return;
        if (isDestroy) return;
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

    //�Z�֘A
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


    //�v��Ȃ�����������
    override protected void OnColEnter2D(Collider2D col) { }

    protected override void OnCollisionEnter2D(Collision2D collision) { }

    override protected void OnColStay2D(Collider2D col) { }

    override protected void Update() { }

    override protected void FixedUpdate() { }

    //���S������
    protected override void OnDestroyMode() { }

    //�d��
    protected override void Gravity() { }
}
