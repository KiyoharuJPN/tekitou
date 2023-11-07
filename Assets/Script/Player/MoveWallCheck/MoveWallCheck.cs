using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWallCheck : MonoBehaviour
{

    [SerializeField, Header("プレイヤー")]
    PlayerController player;
    [SerializeField, Header("壁チェック_左")]
    WallCheck_coll wallCheck_Left;
    [SerializeField, Header("壁チェック_右")]
    WallCheck_coll wallCheck_Right;

    private void Update()
    {
        if (wallCheck_Left.isWall && wallCheck_Right.isWall) 
        {
            player.PlayerDead();
        }
    }
}
