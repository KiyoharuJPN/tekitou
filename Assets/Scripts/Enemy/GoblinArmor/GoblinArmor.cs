using System.Collections;
using UnityEngine;

public class GoblinArmor : Enemy
{
    [Tooltip("�ړ����x")]
    public float movingSpeed;
    [Tooltip("�҂����Ԃ̐ݒ�")]
    public float idleTime = 2.4f;


    protected override void Start()
    {
        moveSpeed = movingSpeed * -1;           //�ړ����x�̕����C��
        base.Start();                           //�G��script�Ɋ�Â�
    }
    protected override void Update()
    {
        //�A�j��
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //��Ԃ̕ύX
        IsBlowing = isDestroy;
        //�G��script�Ɋ�Â�
        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        if (isDestroy)
        {
            //������ђ��̉��G�t�F�N�g
            if (effectTime > effectInterval)
            {
                BlowAwayEffect();
                effectTime = 0;
            }
            else effectTime += Time.deltaTime;
            return;
        }


        //��ʓ��ɂ���
        if (OnCamera)
        {
            //��΂���ĂȂ�����
            if (!isDestroy)
            {
                Movement();
            }
        }
        Gravity();
    }

    //�S�u�����̓���
    void Movement()
    {
        //�ړ�
        if (IsMoving)
        {
            enemyRb.velocity = new Vector2(moveSpeed, enemyRb.velocity.y);
        }
    }

    void Attacking()
    {
        //�A�j������
        IsMoving = false;
        IsAttacking = true;
    }

    public void EndAttack()
    {
        //�A�j������
        IsAttacking = false;
        StartCoroutine(Idling());
    }

    public void AttackSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.SwingDownClub);
    }

    IEnumerator Idling()
    {
        yield return new WaitForSeconds(idleTime);
        IsMoving = true;
        AttackChecking = true;
        PlayerNotAttacked = true;
    }

    //�v���C���[���U���G���A�ɂ��鎞�̓����iAttackCheckArea����Ă΂��j
    public override bool PlayerInAttackArea()
    {
        var InAttack = false;
        if (IsMoving && AttackChecking)
        {
            AttackChecking = false;
            Attacking();
            InAttack = true;
        }
        return InAttack;
    }

    public override bool GetPlayerAttacked()
    {
        return PlayerNotAttacked;
    }
    public override void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }
}
