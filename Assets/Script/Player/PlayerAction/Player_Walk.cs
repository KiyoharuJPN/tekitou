using UnityEngine;

public class Player_Walk : MonoBehaviour
{
    //�v���C���[�R���g���[���[�N���X
    PlayerController player;

    //�ϐ�
    private float xSpeed = 0.0f;

    private float dashSpeed;
    private float dashTime; //�_�b�V�����Ă��鎞��
    internal float moveInput; //�ړ��L�[����
    
    float timer;

    bool isDash = false;

    //�A�j���[�V�����p�ϐ�
    

    private void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player.isSideAttack || player.isDropAttack 
            || player.isExAttack || player.isWarpDoor || player.isUpAttack)
        {
            return;
        }

        MoveKay();
        Dash();

        if (!player.isMoving)
        {
            isDash = false;
            dashTime = 0;
            dashSpeed = 0.0f;
            player.isRun = false;
        }
    }

    private void FixedUpdate()
    {
        if (player.isSideAttack || player.isDropAttack || player.isExAttack || player.isWarpDoor)
        {
            return;
        }

        if (player.canMovingCounter <= 0) 
        {
            //�v���C���[�̍��E�̈ړ�
            player.rb.velocity = new Vector2(xSpeed + dashSpeed, player.rb.velocity.y);
            if(player.parallaxBackground != null)
            {
                player.parallaxBackground.StartScroll(player.transform.position);
            }
        }
    }

    //�L�[���͂��ꂽ��ړ�����
    private void MoveKay()
    {
        //�ړ��L�[�擾
        if (player.canMove) moveInput = Input.GetAxis("Horizontal");
        if (!player.canMove)
        {
            moveInput = 0;
            player.rb.velocity = (new Vector2(0, player.rb.velocity.y));
        }


        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            xSpeed = JumpCheck();

        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            xSpeed = -JumpCheck();
        }
        else
        {
            xSpeed = 0.0f;
        }

        player.isMoving = moveInput != 0;
    }

    //�W�����v�����ǂ���
    float JumpCheck()
    {
        if(player.isJumping || player.isFalling)
        {
            return player.moveData.jumpFirstSpeed;
        }
        else { return player.moveData.firstSpeed; }
    }

    //�ړ����̉�����
    void Dash()
    {
        //�_�b�V������
        if (player.isMoving 
            && ((player.moveData.maxSpeed >= (xSpeed + dashSpeed)) 
            && (-player.moveData.maxSpeed <= (xSpeed + dashSpeed))))
        {
            dashTime += Time.deltaTime;
            
            if (dashTime > player.moveData.acceleTime)
            {
                
                dashSpeed += DirectionChack();
                dashTime = 0;
                if ((xSpeed + dashSpeed) >= player.moveData.dashSpeed)
                {
                    player.isRun = true;
                    if (!isDash)
                    {
                        //player.playerSE._DashSE();
                        isDash = true;
                    }
                }
            }
        }
        
    }

    float DirectionChack()
    {
        if (moveInput > 0)
        {
            if ((xSpeed + dashSpeed) >= player.moveData.dashSpeed)
            {
                player.isRun = true;
            }
            return player.moveData.accele;
        }
        else if (moveInput < 0)
        {
            if ((xSpeed + dashSpeed) <= -player.moveData.dashSpeed)
            {
                player.isRun = true;
            }
            return -player.moveData.accele;
        }
        else return 0.0f;
    }
}
