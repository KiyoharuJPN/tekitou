using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{
    [Tooltip("wizard�������U��")]
    public GameObject Wizard_MagicBall;

    //�v���C���[�̃I�u�W�F�N�g
    GameObject playerObj;



    protected override void Start()
    {
        //player�̃I�u�W�F�N�g���擾����
        playerObj = GameObject.Find("Hero");

        base.Start();
    }

    protected override void Update()
    {
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


    }






    //�O���ڑ��p�֐�
    //�������]
    public override void TurnAround()
    {
        //������]����
        gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);

    }




    //�����p�֐�
    //�v���C���[�ʒu�𔻒f���āA��]���s��
    void LookForPlayer()
    {
        if (playerObj.transform.position.x < gameObject.transform.position.x && gameObject.transform.localScale.x > 0)
        {
            TurnAround();
        }else if(playerObj.transform.position.x > gameObject.transform.position.x && gameObject.transform.localScale.x < 0)
        {
            TurnAround();
        }
    }
    //�d�͊֘A
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Gravity();
        if(enemyRb.velocity.y > 0)
        {
            enemyRb.velocity = Vector3.zero;
        }
    }
}
