using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class SideAttack:MonoBehaviour
{
    //横攻撃発生必要時間
    const float dashingTime = 0.2f;

    public static void SideAttackStart(PlayerController player, bool sideJudge, MonoBehaviour p_behaviour)
    {
        p_behaviour.StartCoroutine(SideAttackMove(player, sideJudge));
    } 

    //横攻撃処理
    static IEnumerator SideAttackMove(PlayerController player, bool sideJudge)
    {
        //アニメーションセット
        player.canSideAttack = false;
        player.isAttack = true;
        player.isSideAttack = true;
        player.animator.SetBool("IsSideAttack", player.isSideAttack);
        player.animator.Play("Hero_SideAttack_Start");

        float originalGravity = player.rb.gravityScale;
        player.rb.gravityScale = 0f;
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.SideAttack);
        SideJudge(skill, player, sideJudge);

        yield return new WaitForSeconds(dashingTime);
        player.rb.gravityScale = originalGravity;
        player.isSideAttack = false;
        player.animator.SetBool("IsSideAttack", player.isSideAttack);
        player.enemylist.Clear();
        yield return new WaitForSeconds(skill.coolTime);
        player.isAttack = false;
        player.canSideAttack = true;
    }

    //横攻撃左右判定
    public static void SideJudge(Skill skill, PlayerController player, bool sideJudge)
    {
        Vector3 localScale = player.transform.localScale;
        if (player.transform.localScale.x > 0)
        {
            if (sideJudge)
            {
                player.rb.velocity = new Vector2(player.transform.localScale.x * skill.distance, 0f);
                
            }
            else if (!sideJudge)
            {
                player.rb.velocity = new Vector2(-player.transform.localScale.x * skill.distance, 0f);
                localScale.x *= -1f;
                player.transform.localScale = localScale;
            }
        }
        else if (player.transform.localScale.x < 0)
        {
            if (sideJudge)
            {
                player.rb.velocity = new Vector2(-player.transform.localScale.x * skill.distance, 0f);
                localScale.x *= -1f;
                player.transform.localScale = localScale;
            }
            else if (!sideJudge)
            {
                player.rb.velocity = new Vector2(player.transform.localScale.x * skill.distance, 0f);
            }
        }
        Slashing(player, sideJudge);
    }

    //追加斬撃
    private static void Slashing(PlayerController player, bool sideJudge)
    {
        if (sideJudge)
        {
            if (player.gameObject.GetComponent<SlashingBuff>())
                player.gameObject.GetComponent<SlashingBuff>().Slashing(SlashingBuff.SlashingType.sideAttack_Right, player.gameObject);
        }
        else if(!sideJudge) 
        {
            if (player.gameObject.GetComponent<SlashingBuff>())
                player.gameObject.GetComponent<SlashingBuff>().Slashing(SlashingBuff.SlashingType.sideAttack_Left, player.gameObject);
        }
    } 
}
