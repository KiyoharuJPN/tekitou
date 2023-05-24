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
    bool /*IsBlowing = false, IsMoving = false, */moveHideFlag = false, BossSummon = false, BossTurn = false;

    //float TestTime = 0f;
    override protected void Start()
    {
        //�ړ��֌W�̓����֐��ɑ��
        movingWidth = moveWidth * -1;
        movingHeight = moveHeight;
        //�����p�A�j���[�^�[�̑��
        animator = GetComponent<Animator>();
        IsMoving = false;
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



        //��b�̑҂�����
        if (IsMoving && !moveHideFlag)
        {
            moveHideFlag = true;
            StartCoroutine(SetMoveFalse());
            //�ړ������B
            enemyRb.AddForce(new Vector2(movingWidth, movingHeight), ForceMode2D.Impulse);
        }
        if (!IsMoving && !moveHideFlag)
        {
            moveHideFlag = true;
            StartCoroutine(SetMoveTrue());
        }

        
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

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsMoving && col.gameObject.CompareTag("Stage") && BossSummon)
        {
            if(BossTurn) movingWidth *= -1;
            BossSummon = false;
            moveHideFlag = false;
            IsMoving = true;
        }
        if (col.gameObject.CompareTag("Stage")) enemyRb.velocity = Vector2.zero;
        base.OnCollisionEnter2D(col);
    }
    private void FixedUpdate()
    {
        Gravity();
    }
    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -5f));
    }
    public void SetIsMoving(bool im)
    {
        IsMoving = im;
        moveHideFlag = true;
        BossSummon = true;
    }
    public void SummonSlimeTurn()
    {
        Vector2 scale = this.transform.localScale;
        scale.x *= -1;
        this.transform.localScale = scale;
        BossTurn = true;
    }
    public override void TurnAround()
    {
        movingWidth *= -1;
        base.TurnAround();
    }
}
