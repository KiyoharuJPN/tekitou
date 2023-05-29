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

    float movingHeight, movingWidth, movingCheck;    //�ړ��Ɋւ�������֐�
    //�`�F�b�N�p�����֐�
    bool /*IsBlowing = false, IsMoving = false, */BossSummon = false, BossTurn = false;
    int MovingAnim = 0;

    //float TestTime = 0f;
    override protected void Start()
    {
        //�ړ��֌W�̓����֐��ɑ��
        movingWidth = moveWidth * -1;
        movingHeight = moveHeight;        
        IsMoving = false;
        ///�G��script�Ɋ�Â�
        base.Start();
    }
    override protected void Update()
    {
        //�G��script�Ɋ�Â�
        base.Update();

        if (enemyRb.velocity.y <-0.1)MovingAnim = 1;
        IsMoving = enemyRb.velocity != Vector2.zero;
        //��ʓ��ɂ���
        if (OnCamera)
        {
            //��΂���ĂȂ�����
            if (!IsBlowing) SlimeMove();
        }
        //TestTime += Time.deltaTime;
        //Debug.Log("moving:" + IsMoving + "\nsecond:" + TestTime);


        //�A�j���[�^�[�̐ݒ�
        animator.SetInteger("MovingAnim", MovingAnim);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //��Ԃ̕ύX
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        
    }

    void SlimeMove()
    {
        //���n�v�Z

        //Debug.Log(MovingAnim);

        if(IsMoving && movingCheck!=0)movingCheck = 0;
        if (!IsMoving)
        {
            movingCheck += Time.deltaTime;
            if(movingCheck > 1)
            {
                movingCheck = 0;
                enemyRb.AddForce(new Vector2(movingWidth, movingHeight), ForceMode2D.Impulse);
            }
        }

        
    }

    //0.1�b�҂��Ĉړ��s�ɂ���
    IEnumerator SetMoveFalse ()
    {
        yield return new WaitForSeconds(0.1f);
        IsMoving = false;
        MovingAnim = 0;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if (!IsMoving && col.gameObject.CompareTag("Stage") && BossSummon)
        {
            if(BossTurn) movingWidth *= -1;
            BossSummon = false;
            enemyRb.AddForce(new Vector2(movingWidth, movingHeight), ForceMode2D.Impulse);
        }
        base.OnCollisionEnter2D(col);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDestroy)
        {
            if (col.CompareTag("Stage") && MovingAnim == 1)
            {
                MovingAnim = 2;
                enemyRb.velocity = Vector2.zero;
                StartCoroutine(SetMoveFalse());
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        Gravity();
    }

    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }
    public void SetIsMoving(bool im)
    {
        IsMoving = im;
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
