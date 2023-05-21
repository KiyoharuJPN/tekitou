using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class Player_Jump : MonoBehaviour
{
    PlayerController player;
    bool isGrounded = false;            //接地フラグ
    const float FALL_VELOCITY = 0.4f;   //落下中判定用定数（characterのVilocityがこれより大きい場合true）

    [Header("すり抜床か判定するか")]
    public bool checkPlatformGroud = true;
    private string platformTag = "GroundPlatform";

    float jumpTime = 0;
    bool isjump = false;
    bool canSecondJump = false;
    float jumpHight;

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
        player.isFalling = player.rb.velocity.y < -FALL_VELOCITY;



        //ジャンプキー取得
        if (player.canMove) JumpBottan();
    }

    private void FixedUpdate()
    {
        Jump();
        Gravity();
    }

    void JumpBottan()
    {
        //二段ジャンプ
        if (canSecondJump)
        {

            if (Input.GetButtonDown("Jump"))
            {
                player.animator.SetTrigger("IsSecondJump");
                canSecondJump = false;
                jumpHight = player.jumpData.secondJumpHeight;
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
            jumpHight = player.jumpData.jumpHeight;
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
                jumpTime += Time.deltaTime;
            }
        }
    }

    void Jump()
    {
        if (!isjump) return;
        player.isJumping = true;

        //ジャンプ高さ制限処理
        if (player.isJumping && player.knockBackCounter <= 0 && !player.isUpAttack)
        {
            if (jumpPos + jumpHight <= player.transform.position.y)
            {
                player.isJumping = false;
                player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
                isjump = false;
            }
        }
        
        if (Input.GetButton("Jump"))
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpData.speed + jumpTime * Time.deltaTime);
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpData.speed + jumpTime * Time.deltaTime * 0.5f);
        }
    }

    //重力
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -player.jumpData.gravity * 2);
        player.rb.AddForce(myGravity);
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
                Invoke(nameof(DropAttackOff), 0.5f);
            };
            
            Invoke("Landingoff", 0.1f);
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
