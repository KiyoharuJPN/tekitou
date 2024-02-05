using UnityEngine;
using static PlayerController;

public class Player_Jump : MonoBehaviour
{
    internal PlayerController player;
    public Player_IsGround ground;
    public Player_IsGround head;
    internal bool isGround = false;            //接地フラグ
    bool isHead = false;            //頭をぶつけた判定
    const float FALL_VELOCITY = 0.4f;   //落下中判定用定数（characterのVilocityがこれより大きい場合true）

    internal float jumpTime = 0;
    internal bool isjump = false;
    internal bool FarstJump;
    internal bool canSecondJump = false;
    internal bool isSecondJump = false;
    internal float jumpHight;

    //ジャンプした際の位置
    internal float jumpPos;

    internal const float upAttackHight = 2f;
    internal bool isUpAttack = false;

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

    protected virtual void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //落下状態取得
        player.isFalling = player.rb.velocity.y < -FALL_VELOCITY;
        //接地状態を得る
        isGround = ground.IsGround();

        if(player.isUpAttack && !isSecondJump) canSecondJump = true;

        //プレイヤーがイベント・攻撃中以外の処理
        if (player.playerState == PlayerState.Idle && Time.timeScale != 0)
        {
            //ジャンプキー取得
            JumpBottan();
        }
    }

    private void FixedUpdate()
    {
        //プレイヤーがイベント・攻撃中以外の処理
        if (player.playerState == PlayerState.Idle || 
            player.playerState == PlayerState.Event ||
            player.playerState == PlayerState.NomalAttack)
        {
            Jump();
            Gravity();
        }
        if(isjump &&
            player.playerState != PlayerState.Idle)
        {
            isjump = false;
            jumpTime = 0;
        }
    }

    void JumpBottan()
    {

        //ジャンプ中の処理
        if (isjump && !isHead)
        {
            if (!player.jumpKay.IsPressed() || jumpTime >= player.jumpData.maxJumpTime)
            {
                isjump = false;
                jumpTime = 0;
            }
            else if(player.jumpKay.IsPressed() && jumpTime <= player.jumpData.maxJumpTime)
            {
                jumpTime += Time.deltaTime;
            }
        }
        else if (isHead)
        {
            isjump = false;
            jumpTime = 0;
        }

        //ジャンプキー入力
        if (player.jumpKay.WasPressedThisFrame())
        {
            JumpSet();
        }
    }

    //ジャンプ使用変数へのセット
    internal void JumpSet()
    {
        //ジャンプ1段目
        if (FarstJump && !canSecondJump && isGround)
        {
            player.isSquatting = true;
            FarstJump = false;
            player.animator.SetBool("IsSquatting", player.isSquatting);
            jumpHight = player.jumpData.jumpHeight;
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            isjump = true;
            player.playerSE._JumpSE();

            player.animator.Play("Hero_anim_Jump_1");
        }
        //ジャンプ2段目
        else if(!FarstJump && canSecondJump)
        {
            player.animator.SetTrigger("IsSecondJump");
            canSecondJump = false;
            isSecondJump = true;
            jumpHight = player.jumpData.secondJumpHeight;
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
            jumpTime = 0;
            isjump = true;
            player._JumpEffect();
        }
    }

    //ジャンプ移動処理
    void Jump()
    {
        if (!isjump) return;
        player.isJumping = true;
        
        if (player.jumpKay.IsPressed())
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, player.jumpData.speed) + jumpTime * Time.deltaTime);
        }

        if (player.jumpKay.WasReleasedThisFrame())
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, player.jumpData.speed) + jumpTime * Time.deltaTime * 0.5f);
        }
    }

    //上昇制限処理
    internal float HeigetLimt(float _jumpPos, float _jumpHeight, float distance)
    {
        if (_jumpPos + _jumpHeight <= player.transform.position.y)
        {
            if (isUpAttack)
            {
                return distance * 0.3f;
            }
            else
            {
                return distance * 0.3f;
            }
        }
        else
        {
            return distance;
        }
    }

    //重力
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -player.jumpData.gravity * 2);
        player.rb.AddForce(myGravity);
    }
}