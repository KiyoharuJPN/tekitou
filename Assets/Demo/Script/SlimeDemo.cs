using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class SlimeDemo : Enemy
{
    [Header("�ړ����鎞�̍����Ƌ���")]
    public float moveHeight, moveWidth;
    [Header("�\������")]
    public float StanceTime = 1;

    float movingHeight, movingWidth, movingCheck;    //�ړ��Ɋւ�������֐�
    //�`�F�b�N�p�����֐�
    bool BossSummon = false, BossTurn = false;
    int MovingAnim = 0;

    private int cmeraReflexNum = 2;

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
        if (enemyRb.velocity.y < -1) MovingAnim = 1;
        IsMoving = MovingAnim == 3 ? true : enemyRb.velocity != Vector2.zero;
        //��ʓ��ɂ���
        if (OnCamera)
        {
            //Debug.Log(moveWidth);
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

        if (IsMoving && movingCheck != 0) movingCheck = 0;
        if (!IsMoving && MovingAnim == 0)
        {
            movingCheck += Time.deltaTime;
            if (movingCheck > StanceTime)
            {
                movingCheck = 0;
                MovingAnim = 3;
            }
        }

    }

    //0.1�b�҂��Ĉړ��s�ɂ���
    IEnumerator SetMoveFalse()
    {
        yield return new WaitForSeconds(0.05f);
        MovingAnim = 0;
    }

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

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (/*!IsMoving && */collision.gameObject.CompareTag("Stage") && BossSummon)
        {
            BossSummon = false;
            if (BossTurn) movingWidth *= -1;
        }
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDestroy)
        {
            if (col.CompareTag("Stage") && MovingAnim == 1 && enemyRb.velocity.y == 0)
            {
                MovingAnim = 2;
                if (enemyRb.velocity.y <= 0) enemyRb.velocity = Vector2.zero;
                StartCoroutine(SetMoveFalse());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!isDestroy)
        {
            if (col.CompareTag("Stage") && MovingAnim == 1 && enemyRb.velocity.y == 0)
            {
                MovingAnim = 2;
                if (enemyRb.velocity.y <= 0) enemyRb.velocity = Vector2.zero;
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
        var eveloX = enemyRb.velocity.x * -1;
        enemyRb.velocity = new Vector2(eveloX, enemyRb.velocity.y);
        base.TurnAround();
    }

    public float GetBossHP()
    {
        return hp;
    }

    public float GetBossFullHP()
    {
        return enemyData.hp;
    }

}