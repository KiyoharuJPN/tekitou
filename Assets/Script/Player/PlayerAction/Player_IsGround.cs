using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_IsGround : MonoBehaviour
{
    //�e�̃I�u�W�F�N�g
    [SerializeField]
    GameObject HERO;

    PlayerController player;
    Player_Jump jumpData;

    void Start()
    {
        player = HERO.GetComponent<PlayerController>();
        jumpData = HERO.GetComponent<Player_Jump>();
    }

    //���n�̔���
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
        {
            if (player.isUpAttack)
            {
                return;
            }
            if (player.isFalling == true)
            {
                player.isLanding = true;
            }
            player.isSquatting = false;
            player.isJumping = false;

            player.rb.velocity = Vector2.zero;
            jumpData.jumpTime = 0;
            jumpData.canSecondJump = false;

            //�˂��h���U���I���
            if (player.isDropAttack)
            {
                jumpData.shake.Shake(jumpData._shakeInfo.Duration, jumpData._shakeInfo.Strength, false, true);
                Invoke(nameof(DropAttackOff), 0.5f);
            };

            Invoke(nameof(Landingoff), 0.1f);
        }
    }

    void Landingoff()
    {
        player.isLanding = false;
    }

    void DropAttackOff()
    {
        player.isDropAttack = false;
        player.animator.SetBool("IsDropAttack", player.isDropAttack);
    }
}
