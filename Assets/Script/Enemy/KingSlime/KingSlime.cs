using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime : Enemy
{
    [Header("�ړ����鎞�̍����Ƌ���")]
    public float moveHeight, moveWidth;

    Animator animator;                          //�G�̃A�j���֐�
    float movingHeight, movingWidth;            //�ړ��Ɋւ�������֐�
    bool moveHideFlag = false, LRmove = false;  //���f�p�����֐�
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
    }

    protected override void Update()
    {
        KingSlimeMoving();



        animator.SetBool("IsMoving", IsMoving);
    }

    void KingSlimeMoving()
    {
        //���n�v�Z


        //�ړ��� �ǂɓ����������̔��]����
        if (IsMoving)
        {
            if (enemyRb.velocity.x == 0 && LRmove)
            {
                Vector2 scale = this.transform.localScale;
                scale.x *= -1;
                this.transform.localScale = scale;
                movingWidth *= -1;
                LRmove = false;
            }
        }

        //��b�̑҂�����
        if (IsMoving && !moveHideFlag)
        {
            moveHideFlag = true;
            StartCoroutine(SetMoveFalse());
            //�ړ������B
            enemyRb.AddForce(new Vector2(movingWidth, movingHeight), ForceMode2D.Impulse);
            LRmove = true;
            StartCoroutine(Landing());
        }
        if (!IsMoving && !moveHideFlag)
        {
            moveHideFlag = true;
            StartCoroutine(SetMoveTrue());
        }

    }

    //���n�܂ł̎��ԑ҂��ƈړ����̊m�F
    IEnumerator Landing()
    {
        yield return new WaitForSeconds(0.5f);
        LRmove = false;
    }
    //��b�҂��Ĉړ��\�ɂ���
    IEnumerator SetMoveTrue()
    {
        yield return new WaitForSeconds(1f);
        IsMoving = true;
        moveHideFlag = false;
    }
    //��b�҂��Ĉړ��s�ɂ���
    IEnumerator SetMoveFalse()
    {
        yield return new WaitForSeconds(1f);
        IsMoving = false;
        moveHideFlag = false;
    }

    private void FixedUpdate()
    {
        Gravity();
    }
    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }
}
