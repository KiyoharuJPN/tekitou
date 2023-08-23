using UnityEngine;

public class SkillAttackArea : MonoBehaviour
{
    [SerializeField]
    PlayerController player;

    //�U���͈͂ɓ�������
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
        //�U��
        if (player.isUpAttack)//�㏸�U��
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.UpAttack);
            player._Attack(Enemy, skill.damage, skill);
        }
        if (player.isSideAttack)//���U��
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.SideAttack);
            player._Attack(Enemy, skill.damage, skill);
        }
        if (player.isDropAttack)//���U��
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.DropAttack);
            player._Attack(Enemy, skill.damage, skill);
        }
        ExAttackParam.Instance.AddGauge();
    }
}
