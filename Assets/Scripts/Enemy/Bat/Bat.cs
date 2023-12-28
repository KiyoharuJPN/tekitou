using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    [Tooltip("�ړ��͈�")]
    public float MoveDistance = 10;
    [Tooltip("�ړ����x")]
    public float MoveSpeed = 5;

    Vector2 BatStartPos, BatEndPos, LeftSpeed, RightSpeed;

    //�ړ������m�F�p�֐� 0��null�A�P�͍��A�Q�͉E
    int Direction = 0;
    //�ŏ��̈ړ��������L�^����֐�
    enum FirstDir
    {
        none,
        Left,
        Right,
    }
    FirstDir firstDir = FirstDir.none;

    protected override void Start()
    {
        base.Start();
        //�����ʒu�ƍŏI�̈ʒu���L�^���āA�ړ��͈͂����߂�
        BatStartPos = transform.position;
        BatEndPos = new Vector2(BatStartPos.x + MoveDistance, BatStartPos.y);
        //�ړ����x��������
        LeftSpeed = new Vector2(-Mathf.Abs(MoveSpeed), 0f);
        RightSpeed = new Vector2(Mathf.Abs(MoveSpeed), 0f);
        //�����ŏ�����E�֍s�����Ƃ�����E�Ɍ�������
        if (BatEndPos.x > transform.position.x) transform.localScale = new Vector2(-1, 1);
    }

    protected override void Update()
    {
        base.Update();
        
        // �A�j���[�^�[�̐ݒ�
        animator.SetBool("IsBlowing", isDestroy);
    }


    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;

        //��΂���Ă��Ȃ�����
        if (!isDestroy)
        {
            //��ʓ��ɂ���Ƃ��͓�������
            if (OnCamera)
            {
                Movement();
            }
            else
            {
                //��ʊO�ɂ���Ƃ��͎~�܂点��
                enemyRb.velocity = Vector2.zero;
                //Debug.Log(3);
            }
        }

        //������ђ��̉��G�t�F�N�g
        if (isDestroy)
        {
            if (effectTime > effectInterval)
            {
                BlowAwayEffect();
                effectTime = 0;
            }
            else effectTime += Time.deltaTime;
        }
    }

    //�G�̓�����֐�
    void Movement()
    {
        if (isPlayerExAttack) return;
        //�ړ����������������ē�����
        if (Direction == 0)
        {
            SetFirstDir();
        }

        //�ړ��֐�
        if(Direction == 1 && enemyRb.velocity == Vector2.zero)
        {
            enemyRb.velocity = LeftSpeed;
        }
        else if(Direction == 2 && enemyRb.velocity == Vector2.zero)
        {
            enemyRb.velocity = RightSpeed;
        }

        //�]���֐�
        switch (firstDir)
        {
            case FirstDir.Left:
                if (BatEndPos.x >= transform.position.x && Direction == 1)
                {
                    TurnAround();
                }
                if(BatStartPos.x <= transform.position.x && Direction == 2)
                {
                    TurnAround();
                }
                break;
            case FirstDir.Right:
                if (BatEndPos.x <= transform.position.x && Direction == 2)
                {
                    TurnAround();
                }
                if (BatStartPos.x >= transform.position.x && Direction == 1)
                {
                    TurnAround();
                }
                break;
            case 0:
                Debug.Log("�������������������Ă��Ȃ��悤�ł�");
                break;
        }
    }


    //�O���֐�
    //�]���֐�
    public override void TurnAround()
    {
        var check = true;
        //���֍s��
        if (Direction == 2 && check)
        {
            Direction = 1;
            transform.localScale = transform.localScale = new Vector2(1, 1);
            enemyRb.velocity = LeftSpeed;
            check = false;
        }
        //�E�֍s��
        if (Direction == 1 && check)
        {
            Direction = 2;
            transform.localScale = transform.localScale = new Vector2(-1, 1);
            enemyRb.velocity = RightSpeed;
            //check= false;
        }
    }



    //�����֐�
    //����������
    void SetFirstDir()
    {
        //���������Ď��ۂɈړ�������
        if (BatEndPos.x < transform.position.x)
        {
            enemyRb.velocity = LeftSpeed;
            Direction = 1;
            firstDir = FirstDir.Left;
        }
        else if (BatEndPos.x > transform.position.x)
        {
            if (transform.localScale.x != -1) transform.localScale = new Vector2(-1, 1);
            enemyRb.velocity = RightSpeed;
            Direction = 2;
            firstDir = FirstDir.Right;
        }
        else
        {
            Debug.Log("�ړ��͈͕͂s�\�̒l�ɂȂ��Ă��܂��B");
        }
    }
}
