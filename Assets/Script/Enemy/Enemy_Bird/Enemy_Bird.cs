using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bird : Enemy
{
    [Tooltip("�ړ����x")]
    public float movingSpeed = 0.5f;                //�ړ����x
    [Header("�ړ��͈�")]
    public float moveArea = 10f;                    //�ړ��͈�
    [Tooltip("true�͍��Ɉړ��Afalse�͉E�Ɉړ�")]
    public bool LRMove;                             //�ړ�����
    [Tooltip("�҂����Ԃ̐ݒ�")]
    public float idleTime = 2.4f;


    //�����֐�
    Vector2 MovingArea;

    protected override void Start()
    {
        MovingArea = gameObject.transform.position;
        moveSpeed = movingSpeed * -1;
        base.Start();
    }

    protected override void Update()
    {
        if (OnCamera)      //��ʓ��ɂ���Ƃ�
        {
            Movement();
        }


        //�A�j���[�^�[�̐ݒ�
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //��΂���Ă��邩�ǂ����̔���
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //Enemy�̌p��
        base.Update();
    }

    void Movement()
    {
        //�ړ��͈͊O�Ɉړ������������ς���
        if(transform.position.x >= MovingArea.x + moveArea / 2 && LRMove)
        {
            TurnAround();
        }else if (transform.position.x <= MovingArea.x - moveArea / 2 && !LRMove)
        {
            TurnAround();
        }

        //�ړ�
        if (IsMoving)
        {
            enemyRb.velocity = new Vector2(moveSpeed, enemyRb.velocity.y);
        }
    }

    void Attacking()
    {
        //�A�j������
        IsAttacking = true;
        PlayerNotAttacked = true;
        IsMoving = false;

        //�U������Ƃ��̓���
        SoundManager.Instance.PlaySE(SESoundData.SE.BirdChirping);

        //55
        //AttackChecking = true;
        //BirdAttackArea.SetActive(true);

        //66
        //IsAttacking = false;
        //IsMoving = true;
    }
    
    public void StartAttack()
    {
        AttackChecking = true;
        
    }
    public void EndAttack()
    {
        IsAttacking = false;
        IsMoving = true;
    }


    public override void TurnAround()
    {
        LRMove = !LRMove;
        base.TurnAround();
    }

    public override bool  PlayerInAttackArea()
    {
        var InAttack = false;
        if (IsMoving && AttackChecking)
        {
            //���U��
            AttackChecking = false;
            Attacking();
            InAttack = true;
        }
        return InAttack;
    }

    public bool HadAttacked()
    {
        if (!HadAttack)
        {
            //�ڐG�_���[�W����������Ȃ��ɂ���
            HadAttack = true;
            StartCoroutine(HadAttackReset());
            return true;
        }
        return false;
    }


    /*    //��΂����Ƃ��ɃR���C�_�[��ς���Ƃ��̑���
        protected override void _Destroy()
        {
            //���˗p�̃R���C�_�[�ɕύX
            this.GetComponent<BoxCollider2D>().enabled = false;
            this.GetComponent<CircleCollider2D>().enabled = true;
            enemyRb.bodyType = RigidbodyType2D.Dynamic;
            enemyRb.constraints = RigidbodyConstraints2D.None;
            CalcForceDirection();
            //������ъJ�n
            BoostSphere();
            isDestroy = true;
            gameObject.layer = LayerMask.NameToLayer("PinBallEnemy");
        }*/

}
