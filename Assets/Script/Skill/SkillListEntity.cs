using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillListEntity : ScriptableObject
{
    public List<Skill> skillList = new List<Skill>();
}
