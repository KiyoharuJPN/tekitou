using System;
using System.Collections;
using UnityEngine;

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
    private bool isGroundEnter, isGroundStay, isGroundExit, stageLandingCheck = false;

    void Start()
    {
        player = HERO.GetComponent<PlayerController>();
        jumpData = HERO.GetComponent<Player_Jump>();
    }

    //接地判定を返すメソッド
    //物理判定の更新毎に呼ぶ必要がある
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

        if (!player.isUpAttack && stageLandingCheck && !player.isGround) { Landingoff(); }
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
            //地面に降りないバグ用チェック関数
            stageLandingCheck = false;
        }
        else if (checkPlatformGroud && collision.tag == platformTag)
        {
            isGroundExit = true;
            player.isGround = false;
            jumpData.FarstJump = false;
            jumpData.canSecondJump = true;
            //地面に降りないバグ用チェック関数
            stageLandingCheck = false;
        }
    }

    //stage着地処理
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

            //突き刺し攻撃終わり
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
