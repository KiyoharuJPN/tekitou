using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class Player_Jump : MonoBehaviour
{
    PlayerController player;

    float jumpTime = 0;
    bool isjump = false;
    bool canSecondJump = false;

    //ジャンプした際の位置
    float jumpPos;

    //カメラ揺れ（突き刺し終了時に使用）
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("揺れ時間")]
        public float Duration;
        [Tooltip("揺れの強さ")]
        public float Strength;
    }

    [SerializeField]
    [Header("画面揺れに関する")]
    public ShakeInfo _shakeInfo;

    public CameraShake shake;

    void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //ジャンプキー取得
        JumpBottan();

        //ジャンプ処理
        if (isjump && player.knockBackCounter <= 0)
        {
            if (jumpPos + player.jumpData.jumpHeight < player.transform.position.y)
            {
                player.rb.velocity = new Vector2(player.rb.velocity.x, player.rb.velocity.y * 0.2f);
                isjump = false;
            }

            Jump();
        }

        
    }

    private void FixedUpdate()
    {
        //重力
        Gravity();
    }

    void JumpBottan()
    {
        //二段ジャンプ
        if (canSecondJump)
        {

            if (Input.GetButtonDown("Jump"))
            {
                canSecondJump = false;
                jumpPos = this.transform.position.y;
                player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
                jumpTime = 0;
                isjump = true;
            }
        }

        //一回目ジャンプ
        if (Input.GetButtonDown("Jump") && !player.isJumping && !player.isFalling && !canSecondJump)
        {
            player.isSquatting = true;
            player.animator.SetBool("IsSquatting", player.isSquatting);
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            isjump = true;
            canSecondJump = true;
        }

        //ジャンプ中の処理
        if (isjump)
        {
            if (!Input.GetButton("Jump") || jumpTime >= player.jumpData.maxJumpTime)
            {
                isjump = false;
                jumpTime = 0;
            }
            else if (Input.GetButton("Jump") && jumpTime <= player.jumpData.maxJumpTime)
            {
                Debug.Log("ジャンプキー長押し中");
                jumpTime += Time.deltaTime;
            }
        }
    }

    void Jump()
    {
        player.isJumping = true;
        //

        if (canSecondJump)
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpData.speed + jumpTime * Time.deltaTime);
            //player.rb.AddForce(transform.up * player.jumpData.speed, ForceMode2D.Impulse);
        }
        else
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpData.speed + jumpTime * Time.deltaTime);
            //player.rb.AddForce(transform.up * ((player.jumpData.speed / 5) * 4), ForceMode2D.Impulse);
        }
    }

    //重力
    void Gravity()
    {
        if (player.knockBackCounter <= 0 || !player.isSideAttack)
        {
            player.rb.AddForce(new Vector2(0, -player.jumpData.gravity));
        }
    }


    //着地の判定
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
        {
            if (player.isUpAttack)
            {
                return;
            }

            if (player.isFalling == true)
            {
                player.isLanding = true;
            }
            player.isSquatting = false;
            player.isJumping = false;
            
            player.rb.velocity = Vector2.zero;
            jumpTime = 0;
            canSecondJump = false;

            //突き刺し攻撃終わり
            if (player.isDropAttack)
            {
                shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength);
                Invoke("DropAttackOff", 0.5f);
            };
            
            Invoke("Landingoff", 0.01f);
        }
    }

    void Landingoff()
    {
        player.isLanding = false;
    }

    private void DropAttackOff()
    {
        
        player.isDropAttack = false;
        player.animator.SetBool("IsDropAttack", player.isDropAttack);
    }
}
