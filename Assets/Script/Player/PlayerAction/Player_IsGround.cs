using System;
using System.Collections;
using UnityEngine;

public class Player_IsGround : MonoBehaviour
{
    //�e�̃I�u�W�F�N�g
    [SerializeField]
    GameObject HERO;

    [Header("���蔲�������肷�邩")]
    public bool checkPlatformGroud = true;

    PlayerController player;
    Player_Jump jumpData;

    //�n�ʂ̃^�O
    private string groundTag = "Stage";
    private string platformTag = "PlatFormStage";
    //�ڒn����t���O
    public bool isGround = false;
    private bool isGroundEnter, isGroundStay, isGroundExit, stageLandingCheck = false;

    void Start()
    {
        player = HERO.GetComponent<PlayerController>();
        jumpData = HERO.GetComponent<Player_Jump>();
    }

    //�ڒn�����Ԃ����\�b�h
    //��������̍X�V���ɌĂԕK�v������
    public bool IsGround()
    {
        if (isGroundExit)
        {
            isGround = false;
        }
        else if (isGroundEnter || isGroundStay)
        {
            isGround = true;
        }

        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;
        return isGround;
    }

    //���n�̔���
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == groundTag)
        {
            isGroundEnter = true;
            StageLanding(collision);
        }
        else if(checkPlatformGroud && collision.tag == platformTag)
        {
            isGroundEnter = true;
            StageLanding(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundStay = true;
        }
        else if (checkPlatformGroud && collision.tag == platformTag)
        {
            isGroundStay = true;
        }

        if (!player.isUpAttack && stageLandingCheck && !player.isGround) { Landingoff(); }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //�X�e�[�W���痣�ꂽ
        if (collision.tag == groundTag)
        {
            isGroundExit = true;
            player.isGround = false;
            jumpData.FarstJump = false;
            jumpData.canSecondJump = true;
            //�n�ʂɍ~��Ȃ��o�O�p�`�F�b�N�֐�
            stageLandingCheck = false;
        }
        else if (checkPlatformGroud && collision.tag == platformTag)
        {
            isGroundExit = true;
            player.isGround = false;
            jumpData.FarstJump = false;
            jumpData.canSecondJump = true;
            //�n�ʂɍ~��Ȃ��o�O�p�`�F�b�N�֐�
            stageLandingCheck = false;
        }
    }

    //stage���n����
    void StageLanding(Collider2D collision)
    {
        if (!player.isUpAttack)
        {
            player.isSquatting = false;
            player.isJumping = false;

            jumpData.jumpTime = 0;
            player.canUpAttack = true;

            jumpData.canSecondJump = false;
            jumpData.isSecondJump = false;

            //�˂��h���U���I���
            if (player.isDropAttack)
            {
                jumpData.shake.Shake(jumpData._shakeInfo.Duration, jumpData._shakeInfo.Strength, false, true);
                Landingoff();
                Debug.Log("dropattack");
                //player.isGround = true;
                //Invoke("Landingoff", 0.18f);
            }
            else
            {
                Landingoff();
                Debug.Log("Nomalland");
            }
        }
        stageLandingCheck = true;
    }

    void Landingoff()
    {
        player.isGround = true;
        player.isNomalAttack = false;
        player.animator.SetBool("IsNomalAttack_1", player.isNomalAttack);
        jumpData.FarstJump = true;
        player.isLanding = false;
        
        player.isAttack = false;
    }
}
