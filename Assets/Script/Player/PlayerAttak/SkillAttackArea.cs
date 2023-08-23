using UnityEngine;

public class SkillAttackArea : MonoBehaviour
{
    [SerializeField]
    PlayerController player;

    //UŒ‚”ÍˆÍ‚É“ü‚Á‚½
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !player.enemylist.Contains(other.gameObject))
        {
            player.enemylist.Add(other.gameObject);
            HitDamage(other);
        }
    }

    void HitDamage(Collider2D Enemy)
    {
        //UŒ‚
        if (player.isUpAttack)//ã¸UŒ‚
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.UpAttack);
            player._Attack(Enemy, skill.damage, skill);
        }
        if (player.isSideAttack)//‰¡UŒ‚
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.SideAttack);
            player._Attack(Enemy, skill.damage, skill);
        }
        if (player.isDropAttack)//‰ºUŒ‚
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.DropAttack);
            player._Attack(Enemy, skill.damage, skill);
        }
        ExAttackParam.Instance.AddGauge();
    }
}
