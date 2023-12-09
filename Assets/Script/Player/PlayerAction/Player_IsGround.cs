using System;
using System.Collections;
using UnityEngine;
using static PlayerController.PlayerState;

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
    private bool isGroundEnter, isGroundStay, isGroundExit;

    void Start()
    {
        player = HERO.GetComponent<PlayerController>();
        jumpData = HERO.GetComponent<Player_Jump>();
    }

    //�ڒn�����Ԃ����\�b�h
    //��������̍X�V���ɌĂԕK�v������
    public bool IsGround()
    {
        if (isGroundEnter || isGroundStay)
        {
            isGround = true;
        }
        else if (isGroundExit)
        {
            isGround = false;
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
        }
        else if (checkPlatformGroud && collision.tag == platformTag)
        {
            isGroundExit = true;
            player.isGround = false;
            jumpData.FarstJump = false;
            jumpData.canSecondJump = true;
        }
    }

    //stage���n����
    void StageLanding(Collider2D collision)
    {
        player.isSquatting = false;
        player.isJumping = false;

        jumpData.jumpTime = 0;

        jumpData.canSecondJump = false;
        jumpData.isSecondJump = false;
        player.canUpAttack = true;

        switch (player.playerState)
        {
            case PlayerController.PlayerState.Event:
            case Idle:
                player.canUpAttack = true;
                Landingoff();
                break;

            case DownAttack:
                jumpData.shake.Shake(jumpData._shakeInfo.Duration, jumpData._shakeInfo.Strength, false, true);
                player.AttackEnd();
                player.isGround = true;
                jumpData.FarstJump = true;
                player.isLanding = false;
                player.isFalling = false;

                player.canDropAttack = true;
                break;

            case PlayerController.PlayerState.NomalAttack:
                player.AttackEnd();
                player.isGround = true;
                jumpData.FarstJump = true;
                player.isLanding = false;
                player.isFalling = false;
                break;
        }
    }

    void Landingoff()
    {
        player.isGround = true;
        jumpData.FarstJump = true;
        player.isLanding = false;
    }
}
