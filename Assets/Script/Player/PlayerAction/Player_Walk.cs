using UnityEngine;

public class Player_Walk : MonoBehaviour
{
    //プレイヤーコントローラークラス
    PlayerController player;

    //変数
    private float xSpeed = 0.0f;

    private float dashSpeed;
    private float dashTime; //ダッシュしている時間
    internal float moveInput; //移動キー入力
    
    float timer;

    bool isDash = false;

    //アニメーション用変数
    

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
            //プレイヤーの左右の移動
            player.rb.velocity = new Vector2(xSpeed + dashSpeed, player.rb.velocity.y);
            if(player.parallaxBackground != null)
            {
                player.parallaxBackground.StartScroll(player.transform.position);
            }
        }
    }

    //キー入力されたら移動する
    private void MoveKay()
    {
        //移動キー取得
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

    //ジャンプ中かどうか
    float JumpCheck()
    {
        if(player.isJumping || player.isFalling)
        {
            return player.moveData.jumpFirstSpeed;
        }
        else { return player.moveData.firstSpeed; }
    }

    //移動中の加速等
    void Dash()
    {
        //ダッシュ加速
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
