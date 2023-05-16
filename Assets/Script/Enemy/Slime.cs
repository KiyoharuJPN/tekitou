using System.Dynamic;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Slime : Enemy
{
    [Header("�ړ����鎞�̍����Ƌ���")]
    public float moveHeight, moveWidth;

    Animator animator;      //�G�̃A�j���֐�
    float movingHeight, movingWidth;    //�ړ��Ɋւ�������֐�
    //�`�F�b�N�p�����֐�
    bool IsBlowing = false, IsMoving = false, moveHideFlag = false, LRmove = false;

    //float TestTime = 0f;
    override protected void Start()
    {
        //�ړ��֌W�̓����֐��ɑ��
        movingWidth = moveWidth * -1;
        movingHeight = moveHeight;
        //�����p�A�j���[�^�[�̑��
        animator = GetComponent<Animator>();
        ///�G��script�Ɋ�Â�
        base.Start();
    }
    override protected void Update()
    {
        //��ʓ��ɂ���
        if (OnCamera)
        {
            //��΂���ĂȂ�����
            if (!IsBlowing) SlimeMove();
        }
        //TestTime += Time.deltaTime;
        //Debug.Log("moving:" + IsMoving + "\nsecond:" + TestTime);


        //�A�j���[�^�[�̐ݒ�
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //��Ԃ̕ύX
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //�G��script�Ɋ�Â�
        base.Update();
    }

    void SlimeMove()
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
    IEnumerator SetMoveFalse ()
    {
        yield return new WaitForSeconds(1f);
        IsMoving = false;
        moveHideFlag = false;
    }


}
