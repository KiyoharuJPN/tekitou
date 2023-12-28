using UnityEngine;
using static PlayerController;

public class Player_Walk : MonoBehaviour
{
    //�v���C���[�R���g���[���[�N���X
    PlayerController player;

    //�ϐ�
    private float xSpeed = 0.0f;
    internal float moveInput; //�ړ��L�[����

    //�A�j���[�V�����p�ϐ�
    

    private void Start()
    {
        player = this.gameObject.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if ((player.playerState == PlayerState.Idle ||
            player.playerState == PlayerState.NomalAttack)
            && Time.timeScale != 0)
        {
            MoveKay();
        }
    }

    private void FixedUpdate()
    {
        if (player.playerState == PlayerState.Idle ||
            player.playerState == PlayerState.NomalAttack)
        {
            if (player.canMovingCounter <= 0)
            {
                //�v���C���[�̍��E�̈ړ�
                player.rb.velocity = new Vector2(xSpeed, player.rb.velocity.y);
            }
        }
    }

    //�L�[���͂��ꂽ��ړ�����
    private void MoveKay()
    {
        //�ړ��L�[�擾
        if (player.canMove) moveInput = player.move.ReadValue<Vector2>().x;
        if (!player.canMove)
        {
            moveInput = 0;
            player.rb.velocity = (new Vector2(0, player.rb.velocity.y));
        }

        if(moveInput <= 0.3&&moveInput>=-0.3)
        {
            xSpeed = 0;
        }
        else if(moveInput > 0.3) 
        {
            transform.localScale = new Vector3(1, 1, 1);
            xSpeed = player.moveData.maxSpeed;
        }
        else if(moveInput < -0.3)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            xSpeed = -player.moveData.maxSpeed;
        }

        player.isMoving = (moveInput <= -0.3) || (moveInput >= 0.3);
    }
}
