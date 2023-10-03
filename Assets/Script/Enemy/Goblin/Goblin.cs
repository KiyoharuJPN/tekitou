using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    [Tooltip("�ړ����x")]
    public float movingSpeed;
    [Tooltip("�҂����Ԃ̐ݒ�")]
    public float idleTime = 2.4f;
    //[SerializeField]
    //[Tooltip("�S�u�����U��")]
    //public GameObject GoblinAttack;

    //float x = 1,y = 1;

    //�`�F�b�N�p�����֐�
    //bool AttackChecking = true, PlayerNotAttacked = true;


    protected override void Start()
    {
        moveSpeed = movingSpeed * -1;           //�ړ����x�̕����C��
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

    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        Gravity();
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

    void Attacking()
    {
        //�A�j������
        IsMoving = false;
        IsAttacking = true;

        //�U������Ƃ��̓���
        //int i = 0;
        //while (i < 25)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}
        //SoundManager.Instance.PlaySE(SESoundData.SE.SwingDownClub);
        //GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-.55f, 1.05f);
        //GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.4f, 1f);
        //GoblinAttack.SetActive(true);
        //while (i < 32)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}
        //GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-.8f, .3f);
        //GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.7f, 2.3f);
        //while (i < 37)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}
        //GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-1.05f, -0.1f);
        //GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.1f, 1.6f);
        //while (i < 38)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}
        //GoblinAttack.SetActive(false);
        //while (i < 57)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}

        
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
        if (IsMoving&&AttackChecking)
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
