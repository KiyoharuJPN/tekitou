using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    internal float jumpTime = 0;
    bool isjump = false;
    internal bool FarstJump;
    internal bool canSecondJump = false;
    internal bool isSecondJump = false;
    float jumpHight;

    //ジャンプした際の位置
    float jumpPos;

    bool isUpAttack = false;
    Skill UpAttackStatus;

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
        UpAttackStatus = SkillGenerater.instance.SkillSet(Skill.Type.UpAttack);
    }

    // Update is called once per frame
    void Update()
    {

        //停止
        if (player.isExAttack || player.isWarpDoor) return;

        player.isFalling = player.rb.velocity.y < -FALL_VELOCITY;

        if(player.isUpAttack && !isSecondJump) canSecondJump = true;

        //ジャンプキー取得
        if (player.canMove || !player.isAttack) JumpBottan();
    }

    private void FixedUpdate()
    {
        if (player.isExAttack) return;
        if (isUpAttack)
        {
            UpAttackMove();
            return;
        }
        Jump();
        Gravity();
    }

    void JumpBottan()
    {
        //ジャンプキー入力
        if (Input.GetButtonDown("Jump"))
        {
            JumpSet();
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

    //ジャンプ使用変数へのセット
    internal void JumpSet()
    {
        //ジャンプ1段目
        if (FarstJump && !canSecondJump)
        {
            player.isSquatting = true;
            FarstJump = false;
            player.animator.SetBool("IsSquatting", player.isSquatting);
            jumpHight = player.jumpData.jumpHeight;
            //ジャンプ前位置格納
            jumpPos = this.transform.position.y;
            isjump = true;
            player.playerSE._JumpSE();
        }
        //ジャンプ2段目
        else if(canSecondJump)
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
        }
    }

    //ジャンプ移動処理
    void Jump()
    {
        if (!isjump) return;

        player.isJumping = true;
        
        if (Input.GetButton("Jump"))
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, player.jumpData.speed) + jumpTime * Time.deltaTime);
        }

        if (Input.GetButtonUp("Jump"))
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x, HeigetLimt(jumpPos, jumpHight, player.jumpData.speed) + jumpTime * Time.deltaTime * 0.5f);
        }
    }

    public async void UpAttack()
    {
        player.isUpAttack = true;
        player.animator.SetBool("IsUpAttack", player.isUpAttack);
        jumpPos = this.transform.position.y;
        jumpHight = 3f;
        await Task.Delay(200);
        isUpAttack = true;
        StartCoroutine(UpAttackEnd());
    }

    void UpAttackMove()
    {
        player.rb.velocity = new Vector2(0, HeigetLimt(jumpPos, jumpHight, UpAttackStatus.distance) + jumpTime * Time.deltaTime);
    }

    private IEnumerator UpAttackEnd()
    {
        var time = 0.4f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        isUpAttack = false;
        player.isUpAttack = false;
        player.animator.SetBool("IsUpAttack", player.isUpAttack);
    }

    //上昇制限処理
    float HeigetLimt(float _jumpPos, float _jumpHeight, float distance)
    {
        if (_jumpPos + _jumpHeight <= player.transform.position.y)
        {
            return distance * 0.3f;
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
