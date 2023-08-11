using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuff : MonoBehaviour
{
    public PlayerController player;

    [SerializeField, Header("速度上昇バフ:初期上昇量")]
    float speedUpNum = 5;
    [SerializeField, Header("速度上昇バフ:減少量")]
    float speedBuffDown = 1;
    //スピードアップバフ獲得回数
    float speedBuffCount = 0;

    //プレイヤーステータス初期値格納変数
    float firstSpeedUpNum;
    float moveFirstSpeed;
    float moveDashSpeed;
    float moveMaxSpeed;
    float jumpSpeed;

    public static PlayerBuff Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        firstSpeedUpNum = speedUpNum;
        moveFirstSpeed = player.moveData.firstSpeed;
        moveDashSpeed = player.moveData.dashSpeed;
        moveMaxSpeed = player.moveData.maxSpeed;
        jumpSpeed = player.jumpData.speed;
    }

    /// <summary>
    /// 速度上昇バフ
    /// </summary>
    public void SpeedUp()
    {
        speedBuffCount++;
        if(speedBuffCount % 2 == 0 && speedUpNum > 1) 
        {
            speedUpNum -= speedBuffDown;
        }

        player.moveData.firstSpeed += speedUpNum;
        player.moveData.dashSpeed += speedUpNum;
        player.moveData.maxSpeed += speedUpNum;
        player.jumpData.speed += speedUpNum;
    }

    /// <summary>
    /// バフリセット
    /// </summary>
    public void BuffRest()
    {
        speedBuffCount = 0;

        speedUpNum = firstSpeedUpNum;
        player.moveData.firstSpeed = moveFirstSpeed;
        player.moveData.dashSpeed = moveDashSpeed;
        player.moveData.maxSpeed = moveMaxSpeed;
        player.jumpData.speed = jumpSpeed;
    }
}
