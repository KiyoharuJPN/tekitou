using UnityEngine;
using static PlayerController;

public class Player_Jump : MonoBehaviour
{
    internal PlayerController player;
    public Player_IsGround ground;
    public Player_IsGround head;
    internal bool isGround = false;            //�ڒn�t���O
    bool isHead = false;            //�����Ԃ�������
    const float FALL_VELOCITY = 0.4f;   //����������p�萔�icharacter��Vilocity��������傫���ꍇtrue�j

    internal float jumpTime = 0;
    internal bool isjump = false;
    internal bool FarstJump;
    internal bool canSecondJump = false;
    internal bool isSecondJump = false;
    internal float jumpHight;

    //�W�����v�����ۂ̈ʒu
    internal float jumpPos;

    internal const float upAttackHight = 2f;
    internal bool isUpAttack = false;

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

    protected virtual void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //������Ԏ擾
        player.isFalling = player.rb.velocity.y < -FALL_VELOCITY;
        //�ڒn��Ԃ𓾂�
        isGround = ground.IsGround();

        if(player.isUpAttack && !isSecondJump) canSecondJump = true;

        //�v���C���[���C�x���g�E�U�����ȊO�̏���
        if (player.playerState == PlayerState.Idle && Time.timeScale != 0)
        {
            //�W�����v�L�[�擾
            JumpBottan();
        }
    }

    private void FixedUpdate()
    {
        //�v���C���[���C�x���g�E�U�����ȊO�̏���
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

        //�W�����v���̏���
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

        //�W�����v�L�[����
        if (player.jumpKay.WasPressedThisFrame())
        {
            JumpSet();
        }
    }

    //�W�����v�g�p�ϐ��ւ̃Z�b�g
    internal void JumpSet()
    {
        //�W�����v1�i��
        if (FarstJump && !canSecondJump && isGround)
        {
            player.isSquatting = true;
            FarstJump = false;
            player.animator.SetBool("IsSquatting", player.isSquatting);
            jumpHight = player.jumpData.jumpHeight;
            //�W�����v�O�ʒu�i�[
            jumpPos = this.transform.position.y;
            isjump = true;
            player.playerSE._JumpSE();

            player.animator.Play("Hero_anim_Jump_1");
        }
        //�W�����v2�i��
        else if(!FarstJump && canSecondJump)
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
            player._JumpEffect();
        }
    }

    //�W�����v�ړ�����
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

    //�㏸��������
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

    //�d��
    void Gravity()
    {
        Vector2 myGravity = new Vector2(0, -player.jumpData.gravity * 2);
        player.rb.AddForce(myGravity);
    }
}