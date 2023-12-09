using UnityEngine;

public class SkillGenerater : MonoBehaviour
{
    [SerializeField]
    SkillListEntity skillListEntity;

    public static SkillGenerater instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Skill SkillSet(Skill.Type type)
    {
        foreach (Skill skill in skillListEntity.skillList)
        {
            if (skill.type == type)
            {
                return new Skill(skill.type, skill.damage,skill.activeTime, skill.distance, skill.skillText, skill.hitEffectAngle,skill.coolTime);
            }
        }
        return null;
    }
}
