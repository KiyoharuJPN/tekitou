using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class SlimeArmor : Enemy
{
    [Header("�ړ����鎞�̍����Ƌ���")]
    public float moveHeight, moveWidth;
    [Header("�\������")]
    public float StanceTime = 1;

    float movingHeight, movingWidth, movingCheck;    //�ړ��Ɋւ�������֐�
    int MovingAnim = 0;

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
        if (enemyRb.velocity.y < -1)
        {
            MovingAnim = 1;
        }
        IsMoving = MovingAnim == 3 ? true : enemyRb.velocity != Vector2.zero;
        //��ʓ��ɂ���
        if (OnCamera)
        {
            //��΂���ĂȂ�����
            if (!IsBlowing) SlimeMove();
        }


        //�A�j���[�^�[�̐ݒ�
        animator.SetInteger("MovingAnim", MovingAnim);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //��Ԃ̕ύX
        IsBlowing = isDestroy;
    }

    void SlimeMove()
    {
        //���n�v�Z

        //Debug.Log(MovingAnim);

        if (IsMoving && movingCheck != 0) movingCheck = 0;
        if (!IsMoving && MovingAnim != 3)
        {
            movingCheck += Time.deltaTime;
            if (movingCheck > StanceTime)
            {
                movingCheck = 0;
                MovingAnim = 3;

                Debug.Log("in 3");
            }
        }

    }

    IEnumerator SetMoveFalse()
    {
        yield return new WaitForSeconds(0.05f);
        MovingAnim = 0;

        Debug.Log("in 0");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Ontrigger");
        if (!isDestroy)
        {
            if (col.CompareTag("Stage") && MovingAnim == 1)
            {
                MovingAnim = 2;

                Debug.Log("in 2");
                if (enemyRb.velocity.y! < 0) enemyRb.velocity = Vector2.zero;
                StartCoroutine(SetMoveFalse());
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        Gravity();
    }

    protected void SlimeJump()
    {
        enemyRb.AddForce(new Vector2(movingWidth, movingHeight), ForceMode2D.Impulse);
    }

    protected override void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10f));
    }
    public override void TurnAround()
    {
        movingWidth *= -1;
        var eveloX = enemyRb.velocity.x * -1;
        enemyRb.velocity = new Vector2(eveloX, enemyRb.velocity.y);
        base.TurnAround();
    }

}
