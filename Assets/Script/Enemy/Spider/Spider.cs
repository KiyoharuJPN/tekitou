using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy
{
    [Tooltip("�ړ����x")]
    public float MoveSpeed = 4;

    protected override void Start()
    {
        base.Start();
        //�ړ����x������֐��ɑ��
        moveSpeed = MoveSpeed;
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
        //animator.SetBool("IsMoving", IsMoving);
        //animator.SetBool("IsBlowing", isDestroy);
    }
    //�G�̓����֐�
    void Movement()
    {
        if (IsMoving)
        {
            Moving();
        }
        else
        {
            Attacking();
        }
    }
    void Moving()
    {
        enemyRb.AddForce(new Vector2(moveSpeed, 0));
        Debug.Log(moveSpeed);
    }
    void Attacking()
    {

    }



    //�A�j���[�V�����p�֐�



    //�O���֐�
    public override void TurnAround()
    {
        base.TurnAround();
    }
    public override void PlayerInAttackArea()
    {
        base.PlayerInAttackArea();
    }



    //�����֐�

}
