using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{
    [Tooltip("wizard�������U��")]
    public GameObject Wizard_MagicBall,WMBAttackPos;
    [Tooltip("�U������")]
    public int Distance = 15;
    [Tooltip("�U���X�s�[�h")]
    public float MagicSpeed = 5;

    //�v���C���[�̃I�u�W�F�N�g
    GameObject playerObj;
    Vector2 direction;


    protected override void Start()
    {
        //player�̃I�u�W�F�N�g���擾����
        playerObj = GameObject.Find("Hero");
        direction = new Vector2(MagicSpeed, 0);

        base.Start();
    }

    protected override void Update()
    {
        //�G��script�Ɋ�Â�
        base.Update();

        //��΂���Ă��Ȃ����蓮���𑱂�
        if (!isDestroy)
        {
            Movement();
        }

        //���b�A�j�����X�V����
        animator.SetBool("IsAttacking",IsAttacking);
        animator.SetBool("IsBlowing",IsBlowing);
        //��Ԃ̕ύX
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //�G��script�Ɋ�Â�
        base.Update();
    }

    //�����֘A�̊֐�
    void Movement()
    {
        //�v���C���[�ʒu�𔻒f���āA��]���s��
        LookForPlayer();

        //�v���C���[���͈͓��ɓ�������
        if(Mathf.Abs(gameObject.transform.position.x - playerObj.transform.position.x) < Distance)
        {
            //�U�����[�V�������s��
            if (!IsAttacking) IsAttacking = true;
        }
        else
        {
            //�ҋ@���[�V����
            if (IsAttacking) IsAttacking = false;
        }
        
    }

    //�O���ڑ��p�֐�
    //�������]
    public override void TurnAround()
    {
        //������]����
        gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
    }
    //�O������Ȃ����ǁA������ς��鎞�Ɏg��Ȃ���΂Ȃ�Ȃ��R�[�h�Ȃ̂ŕ���ł݂��B
    //�U�������𔽓]����
    void TurnADAround()
    {
        var check = true;
        //�U�����������ύX����
        if (direction.x > 0 && check)
        {
            direction = new Vector2(-MagicSpeed, 0);
            check = false;
        }
        if (direction.x < 0 && check)
        {
            direction = new Vector2(MagicSpeed, 0);
            //check = false;
        }
    }




    //�����p�֐�
    //�v���C���[�ʒu�𔻒f���āA��]���s��
    void LookForPlayer()
    {
        if (playerObj.transform.position.x < gameObject.transform.position.x && gameObject.transform.localScale.x > 0)
        {
            TurnAround();
            TurnADAround();
        }else if(playerObj.transform.position.x > gameObject.transform.position.x && gameObject.transform.localScale.x < 0)
        {
            TurnAround();
            TurnADAround();
        }
    }
    //�d�͊֘A
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Gravity();
        if(enemyRb.velocity.y > 0 && !isDestroy)
        {
            enemyRb.velocity = Vector3.zero;
        }
    }
    //�U���p�֐�
    void ShotMagicBall()
    {
        GameObject Magic = ObjectPool.Instance.GetObject(Wizard_MagicBall);
        Magic.transform.position = WMBAttackPos.transform.position;
        Magic.GetComponent<Wizard_MagicBall>().AKForce(enemyData.attackPower, enemyData.knockBackValue, direction);
    }
}
