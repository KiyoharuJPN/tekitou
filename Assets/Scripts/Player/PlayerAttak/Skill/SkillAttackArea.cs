using UnityEngine;

public class SkillAttackArea : MonoBehaviour
{
    [SerializeField]
    PlayerController player;

    Skill upAttack;
    Skill downAttack;
    Skill sideAttack;

    Skill setSkill;
    float damage;

    private void Start()
    {
        upAttack = SkillGenerater.instance.SkillSet(Skill.Type.UpAttack);
        downAttack = SkillGenerater.instance.SkillSet(Skill.Type.DropAttack);
        sideAttack = SkillGenerater.instance.SkillSet(Skill.Type.DropAttack);
    }

    //çUåÇîÕàÕÇ…ì¸Ç¡ÇΩéû
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
        switch (player.playerState)
        {
            case PlayerController.PlayerState.UpAttack:
                damage = upAttack.damage;
                setSkill = upAttack;
                break;
            case PlayerController.PlayerState.DownAttack:
                damage = downAttack.damage;
                setSkill = downAttack;
                break;
            case PlayerController.PlayerState.SideAttack:
                damage = sideAttack.damage;
                setSkill = sideAttack;
                break;
        }

        player.Attack(Enemy, damage, setSkill, true);
        ExAttackParam.Instance.AddGauge();
    }
}
