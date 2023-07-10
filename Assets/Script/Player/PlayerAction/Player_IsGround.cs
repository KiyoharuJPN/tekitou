using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        if (collision.CompareTag("Stage") && !player.isUpAttack)
        {
            if (player.isFalling == true)
            {
                player.isLanding = true;
            }
            player.isSquatting = false;
            player.isJumping = false;

            player.rb.velocity = Vector2.zero;

            jumpData.jumpTime = 0;
            player.canUpAttack = true;

            jumpData.canSecondJump = false;
            jumpData.isSecondJump = false;

            //�˂��h���U���I���
            if (player.isDropAttack)
            {
                jumpData.shake.Shake(jumpData._shakeInfo.Duration, jumpData._shakeInfo.Strength, false, true);
                player.isDropAttack = false;
                player.animator.SetBool("IsDropAttack", player.isDropAttack);
            }
            StartCoroutine(Interval(0.1f));
        }
    }

    IEnumerator Interval(float time)
    {
        if (!player.canDropAttack)
        {
            time += AnimationCipsTime.GetAnimationTime(player.animator, AnimationCipsTime.ClipType.Hero_DropAttack_End);
        }
        yield return new WaitForSeconds(time);

        Landingoff();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
        {
            jumpData.FarstJump = false;
            jumpData.canSecondJump = true;
        }
    }

    void Landingoff()
    {
        jumpData.FarstJump = true;
        player.isLanding = false;
        player.isAttack = false;
        player.canDropAttack = true;
    }
}
