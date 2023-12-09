using System;
using System.Collections;
using UnityEngine;
using static PlayerController.PlayerState;

public class Player_IsGround : MonoBehaviour
{
    //親のオブジェクト
    [SerializeField]
    GameObject HERO;

    [Header("すり抜床か判定するか")]
    public bool checkPlatformGroud = true;

    PlayerController player;
    Player_Jump jumpData;

    //地面のタグ
    private string groundTag = "Stage";
    private string platformTag = "PlatFormStage";
    //接地判定フラグ
    public bool isGround = false;
    private bool isGroundEnter, isGroundStay, isGroundExit;

    void Start()
    {
        player = HERO.GetComponent<PlayerController>();
        jumpData = HERO.GetComponent<Player_Jump>();
    }

    //接地判定を返すメソッド
    //物理判定の更新毎に呼ぶ必要がある
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

    //着地の判定
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
        //ステージから離れた
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

    //stage着地処理
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
