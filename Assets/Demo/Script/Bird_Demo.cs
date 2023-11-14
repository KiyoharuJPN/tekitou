using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird_Demo : Enemy
{
    [Tooltip("�ړ����x")]
    public float movingSpeed = 0.5f;                //�ړ����x
    [Header("�ړ��͈�")]
    public float moveArea = 10f;                    //�ړ��͈�
    [Tooltip("true�͍��Ɉړ��Afalse�͉E�Ɉړ�")]
    public bool LRMove;                             //�ړ�����
    [Tooltip("�҂����Ԃ̐ݒ�")]
    public float idleTime = 2.4f;

    private int cmeraReflexNum = 2;
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
        if (transform.position.x >= MovingArea.x + moveArea / 2 && LRMove)
        {
            TurnAround();
        }
        else if (transform.position.x <= MovingArea.x - moveArea / 2 && !LRMove)
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

    public override bool PlayerInAttackArea()
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
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage") && isDestroy &&
                collision.gameObject.layer == 20)
        {
            Debug.Log("�J�����G���A����");
            cmeraReflexNum--;
            if (cmeraReflexNum <= 0)
            {
                this.gameObject.layer = 27;
            }
        }
        base.OnCollisionEnter2D(collision);
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
    protected override void OnColEnter2D(Collider2D col)
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
            Debug.Log("����");
            if (col.gameObject.layer == 3)
            {
                Debug.Log("�J�����G���A����");
                cmeraReflexNum--;
                if (cmeraReflexNum <= 0)
                {
                    this.gameObject.layer = 27;
                }
            }
            reflexNum--;
            Debug.Log(reflexNum);
            if (reflexNum == 0)
            {
                EnemyNomalDestroy();
            }
        }
    }
}
