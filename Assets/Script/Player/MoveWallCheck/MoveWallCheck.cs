using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWallCheck : MonoBehaviour
{

    [SerializeField, Header("�v���C���[")]
    PlayerController player;
    [SerializeField, Header("�ǃ`�F�b�N_��")]
    WallCheck_coll wallCheck_Left;
    [SerializeField, Header("�ǃ`�F�b�N_�E")]
    WallCheck_coll wallCheck_Right;

    private void Update()
    {
        if (wallCheck_Left.isWall && wallCheck_Right.isWall) 
        {
            player.PlayerDead();
        }
    }
}
