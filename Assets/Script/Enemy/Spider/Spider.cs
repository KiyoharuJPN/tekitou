using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
{
    [Tooltip("�ړ����x")]
    public float MoveSpeed = 4;

    public GameObject SpiderAttackCheckArea, SpiderAttack;

    Coroutine attack;

    protected override void Start()
    {
        base.Start();
        //�ړ����x������֐��ɑ��
        moveSpeed = -MoveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        //��ʓ�������ł��Ȃ�����
        if (OnCamera && !isDestroy)
        {
            Movement();
        }

        //�A�j���[�^�[�̐ݒ�
        animator.SetBool("IsWalking", IsMoving);
        animator.SetBool("IsBlowing", isDestroy);
    }
    //�G�̓����֐�
    void Movement()
    {
        if (IsMoving)
        {
            Moving();
        }
    }
    void Moving()
    {
        if(enemyRb.velocity == Vector2.zero)
        {
            enemyRb.velocity = new Vector2(moveSpeed, 0);
        }
    }



    //�R���[�`���p�֐�
    IEnumerator SpiderAttacking()
    {
        var i = 0;
        while (i < 25)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        SpiderAttack.SetActive(true);
        SoundManager.Instance.PlaySE(SESoundData.SE.ForefootHeavyAttack);
        while(i < 32)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        SpiderAttack.SetActive(false);
        while (i < 70)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        SpiderAttackCheckArea.SetActive(true);
        IsMoving = true;
        AttackChecking = true;
    }


    //�O���֐�
    public override void TurnAround()
    {
        base.TurnAround();
        enemyRb.velocity = Vector2.zero;
    }
    public override bool PlayerInAttackArea()
    {
        var InAttack = false;
        //true�̏C���͊e�X�N���v�g�ŏ����Ă��������B
        if (IsMoving && AttackChecking)
        {
            AttackChecking = false;
            //Spider
            PlayerNotAttacked = true;
            IsMoving = false;
            attack = StartCoroutine(SpiderAttacking());
            enemyRb.velocity = Vector2.zero;
            SpiderAttackCheckArea.SetActive(false);
            //Spider
            InAttack = true;
        }
        return InAttack;
    }



    //�����֐�

}
