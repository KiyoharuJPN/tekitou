using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static PlayerController;

public class Player_Jump : MonoBehaviour
{
    PlayerController player;
    bool isGrounded = false;            //�ڒn�t���O
    const float FALL_VELOCITY = 0.4f;   //����������p�萔�icharacter��Vilocity��������傫���ꍇtrue�j

    [Header("���蔲�������肷�邩")]
    public bool checkPlatformGroud = true;
    private string platformTag = "GroundPlatform";

    internal float jumpTime = 0;
    bool isjump = false;
    internal bool FarstJump;
    internal bool canSecondJump = false;
    internal bool isSecondJump = false;
    float jumpHight;

    //�W�����v�����ۂ̈ʒu
    float jumpPos;

    bool isUpAttack = false;
    Skill UpAttackStatus;

    //�J�����h��i�˂��h���I�����Ɏg�p�j
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("�h�ꎞ��")]
        public float Duration;
        [Tooltip("�h��̋���")]
        public float Strength;
    }

    [SerializeField]
    [Header("��ʗh��Ɋւ���")]
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

        //��~
        if (player.isExAttack || player.isWarpDoor) return;

        player.isFalling = player.rb.velocity.y < -FALL_VELOCITY;

        if(player.isUpAttack && !isSecondJump) canSecondJump = true;

        //�W�����v�L�[�擾
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
        //�W�����v�L�[����
        if (Input.GetButtonDown("Jump"))
        {
            JumpSet();
        }

        //�W�����v���̏���
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

    //�W�����v�g�p�ϐ��ւ̃Z�b�g
    internal void JumpSet()
    {
        //�W�����v1�i��
        if (FarstJump && !canSecondJump)
        {
            player.isSquatting = true;
            FarstJump = false;
            player.animator.SetBool("IsSquatting", player.isSquatting);
            jumpHight = player.jumpData.jumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            isjump = true;
            player.playerSE._JumpSE();
        }
        //�W�����v2�i��
        else if(canSecondJump)
        {
            player.animator.SetTrigger("IsSecondJump");
            canSecondJump = false;
            isSecondJump = true;
            jumpHight = player.jumpData.secondJumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            player.rb.velocity = new Vector2(player.rb.velocity.x, 0);
            jumpTime = 0;
            isjump = true;
        }
    }

    //�W�����v�ړ�����
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

    //�㏸��������
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

    //�d��
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -player.jumpData.gravity * 2);
        player.rb.AddForce(myGravity);
    }
}
