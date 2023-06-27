using System.Collections;
using UnityEngine;

public class NomalAttackArea : MonoBehaviour
{
    [SerializeField]
    PlayerController player;
    Skill skill;


    private void Start()
    {
        skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
    }

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
        player._Attack(Enemy, skill.damage);
        player._HitEfect(Enemy.transform, skill.hitEffectAngle);
    }
}
