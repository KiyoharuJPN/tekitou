using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class SideAttack
{
    //射程
    const float range = 8f;

    private static float firstPos_x;
    private static float movePos_x;

    private static Vector2 moveVec;

    public static void SideAttackStart(PlayerController player, bool sideJudge, MonoBehaviour p_behaviour)
    {
        //アニメーションセット
        player.animator.SetBool("IsSideAttack", true);
        player.animator.Play("Hero_SideAttack_Start");

        //originalGravity = player.rb.gravityScale;
        //player.rb.gravityScale = 0f;
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.SideAttack);

        p_behaviour.StartCoroutine(SideAttackMove(player, skill, sideJudge));
    }

    static IEnumerator SideAttackMove(PlayerController player, Skill skill, bool sideJudge)
    {
        firstPos_x = player.transform.position.x;


        Vector2 moveVec = SideJudge(skill, player, sideJudge);
        Slashing(player, sideJudge);

        float activeTime = skill.activeTime;

        while (activeTime > 0)
        {
            player.rb.velocity = moveVec;
            activeTime -= Time.deltaTime;

            if (player.transform.position.x > movePos_x && sideJudge)
            {
                break;
            }
            if (player.transform.position.x < movePos_x && !sideJudge)
            {   
                break;
            }

            //イベント時は終了
            if (player.playerState == PlayerController.PlayerState.Event)
            {
                player.animator.SetBool("IsSideAttack", false);
                break;
            }

            yield return null;
        };

        player.rb.velocity = Vector2.zero;
        activeTime = skill.coolTime;

        player.AttackEnd();

        while (activeTime > 0)
        {
            activeTime -= Time.deltaTime;
            yield return null;
        }

        player.canSideAttack = true;
    }

    //横攻撃左右判定
    static Vector2 SideJudge(Skill skill, PlayerController player, bool sideJudge)
    {
        Vector3 localScale = player.transform.localScale;

        if (sideJudge)
        {
            movePos_x = firstPos_x + range;
            localScale.x = 1f;
            player.transform.localScale = localScale;
            
        }
        else if (!sideJudge)
        {
            movePos_x = firstPos_x - range;
            localScale.x = -1f;
            player.transform.localScale = localScale;
        }
        return new Vector2((player.transform.localScale.x * skill.distance), 0f);
    }

    //追加斬撃
    private static void Slashing(PlayerController player, bool sideJudge)
    {
        if (!player.gameObject.GetComponent<SlashingBuff>()) return;

        if (sideJudge)
        {
            player.gameObject.GetComponent<SlashingBuff>().Slashing(SlashingBuff.SlashingType.sideAttack_Right, player.gameObject);
        }
        else if(!sideJudge) 
        {
            player.gameObject.GetComponent<SlashingBuff>().Slashing(SlashingBuff.SlashingType.sideAttack_Left, player.gameObject);
        }
    } 
}
