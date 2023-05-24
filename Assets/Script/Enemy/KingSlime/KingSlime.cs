using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Enemy
{
    [Header("�ړ����鎞�̍����Ƌ���")]
    public float moveHeightForce, moveWidthForce;
    public GameObject summonSlime;

    Animator animator;                                          //�G�̃A�j���֐�
    float movingHeight, movingWidth;                            //�ړ��Ɋւ�������֐�
    bool KSmovingCheck = true, KSattackingCheck = true;        //���f�p�����֐�
    int movingCheck = 0, AttackMode = 0;
    protected override void Start()
    {
        animator = GetComponent<Animator>();    //�A�j���[�^�[���

        movingHeight = moveHeightForce;
        movingWidth = -moveWidthForce;
        base.Start();
    }

    protected override void Update()
    {
        if (!IsBlowing)
        {
            //�ړ��֘A
            if(IsMoving)KingSlimeMoving();
            //�U���֘A
            if(IsAttacking)KingSlimeAttack();
        }


        //�|����邱�Ƃ��m�F���Ă���̂�Enemy�̃��C���֐��ōs���Ă��܂�
        if (isDestroy && !IsBlowing)
        {   //�|���ꂽ�瑼�̃��C���[�̉e�����󂯂Ȃ��悤�ɂ���DeadBossLayer
            IsBlowing = true;
            gameObject.layer = LayerMask.NameToLayer("DeadBoss");
            //KingSlimeBlowing();   //���ʂ̓��������邽�߂ɗp�ӂ����֐� 
        }

        //�A�j���[�V�����֐��̑��
        animator.SetBool("IsMoving", IsMoving);
        animator.SetInteger("AttackMode", AttackMode);
        animator.SetBool("IsBlowing", isDestroy);
        animator.SetBool("IsAnimation",false);
    }

    //�L���O�X���C���̍U��
    void KingSlimeAttack()
    {
        if (KSattackingCheck)
        {
            KSattackingCheck = false;
            switch (AttackMode)
            {
                case 0:
                    //KSBossAtack();
                    KSBossSummon();
                    break;
                case 1:
                    KSBossSummon();
                    break;
            }
        }
    }
    IEnumerator KSBossAtack()
    {
        yield return null;
        IsMoving = true;
        AttackMode = 1;
    }
    IEnumerator KSBossSummon()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeSummon);
        if (GameObject.Find("Hero").transform.position.x > gameObject.transform.position.x && movingWidth < 0) TurnAround();
        if (GameObject.Find("Hero").transform.position.x < gameObject.transform.position.x && movingWidth > 0) TurnAround();
        yield return new WaitForSeconds(0.333f);
        var newSlime1 = Instantiate(summonSlime);
        newSlime1.GetComponent<Rigidbody2D>().AddForce(new Vector2(3, 5),ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);
        var newSlime2 = Instantiate(summonSlime);
        newSlime1.GetComponent<Rigidbody2D>().AddForce(new Vector2(4, 6), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.335f);
        var newSlime3 = Instantiate(summonSlime);
        newSlime1.GetComponent<Rigidbody2D>().AddForce(new Vector2(5, 7), ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.76f);
        IsMoving = true;
        AttackMode = 0;
    }



    //�L���O�X���C���̈ړ�
    void KingSlimeMoving()
    {
        if (KSmovingCheck)
        {
            KSmovingCheck = false;
            StartCoroutine(KSMovingAnim());
        }
    }
    IEnumerator KSMovingAnim()
    {
        movingCheck++;
        yield return new WaitForSeconds(0.5f);
        enemyRb.AddForce(new Vector2(movingWidth, movingHeight),ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.75f);
        if (movingCheck == 4)
        {
            movingCheck = 0;
            IsMoving = false;
            IsAttacking = true;
        }
        KSmovingCheck = true;
    }

    



    //�ǂɓ���������ړ��ʂ�ۂ����܂܉�]���s��
    public override void TurnAround()
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
        enemyRb.velocity = new Vector2(enemyRb.velocity.x * -1,enemyRb.velocity.y);
        movingWidth *= -1;
    }


    //�d�͊֘A
    private void FixedUpdate()
    {
        Gravity();
    }
    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }
}
