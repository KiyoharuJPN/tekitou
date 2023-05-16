using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    Animator animator;

    [Tooltip("�ړ����x")]
    public float movingSpeed;
    [Tooltip("�҂����Ԃ̐ݒ�")]
    public float idleTime = 2.4f;
    [SerializeField]
    [Tooltip("�S�u�����U��")]
    public GameObject GoblinAttack;

    //float x = 1,y = 1;

    //�`�F�b�N�p�����֐�
    bool IsBlowing = false, IsMoving = true, IsAttacking = false, AttackChecking = true, PlayerNotAttacked = true;

    //�ړ����x�����֐�
    float moveSpeed;        

    protected override void Start()
    {
        moveSpeed = movingSpeed * -1;           //�ړ����x�̕����C��
        animator = GetComponent<Animator>();    //�����p�A�j���[�^�[�̑��
        base.Start();                           //�G��script�Ɋ�Â�
    }
    protected override void Update()
    {
        //��ʓ��ɂ���
        if (OnCamera)
        {
            //��΂���ĂȂ�����
            if (!isDestroy)
            {
                Movement();
            }
            
        }
        //�A�j��
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //��Ԃ̕ύX
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //�G��script�Ɋ�Â�
        base.Update();
    }

    //�S�u�����̓���
    void Movement()
    {
        //�R���f�O���ɂ��R�̔��f���s���Ă��܂��B
        /*if (!Physics.Linecast(new Vector2(transform.position.x - x + 0.01f, transform.position.y - y + 0.01f), new Vector2(transform.position.x - x, transform.position.y - y))) Debug.Log("1111111111111111111111111111111111111111111111111111"); //moveSpeed *= -1;
        Debug.Log(new Vector2(transform.position.x - x + 0.01f, transform.position.y - y + 0.01f));
        Debug.Log("\n"+new Vector2(transform.position.x - x, transform.position.y - y));*/

        //�ړ�
        if (IsMoving)
        {
            enemyRb.velocity = new Vector2(moveSpeed, enemyRb.velocity.y);
        }
    }

    IEnumerator Attacking()
    {
        //�A�j������
        IsMoving = false;
        IsAttacking = true;

        //�U������Ƃ��̓���
        int i = 0;
        while (i < 10)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-.55f, 1.05f);
        GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.4f, 1f);
        GoblinAttack.SetActive(true);
        while (i < 15)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-.8f, .3f);
        GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.7f, 2.3f);
        while (i < 20)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-1.05f, -0.1f);
        GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.1f, 1.6f);
        while (i < 21)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        GoblinAttack.SetActive(false);
        while (i < 30)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }

        //�A�j������
        IsAttacking = false;
        StartCoroutine(Idling());
    }

    IEnumerator Idling()
    {
        yield return new WaitForSeconds(idleTime);
        IsMoving = true;
        AttackChecking = true;
        PlayerNotAttacked = true;
    }

    //�v���C���[���U���G���A�ɗv�鎞�̓����iAttackCheckArea����Ă΂��j
    public void PlayerInAttackArea()
    {
        if (IsMoving&&AttackChecking)
        {
            AttackChecking = false;
            StartCoroutine(Attacking());
        }
    }

    //�O���獡�̈ړ���Ԃ��m�F
    public bool GetIsMoving()
    {
        return IsMoving;
    }

    //�O���獡�̐�����΂���Ԋm�F
    public bool GetIsBlowing()
    {
        return IsBlowing;
    }

    //�S�u�����̈ړ�������ς���
    public void TurnAround()
    {
        bool InCheck = true;
        if(transform.localScale.x == 1f && InCheck)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            InCheck = false;
        }
        if(transform.localScale.x == -1f && InCheck)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            //InCheck = false;
        }
        moveSpeed *= -1;
    }

    //�U���͂��O�Ŏ擾����
    public int GetGoblinDamage()
    {
        return enemyData.attackPower;
    }
    
    //�m�b�N�o�b�N�͂��O�Ŏ擾����/
    public float GetGoblinKnockBackForce()
    {
        return enemyData.knockBackValue;
    }

    public bool GetPlayerAttacked()
    {
        return PlayerNotAttacked;
    }
    public void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }
}
