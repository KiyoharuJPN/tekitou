using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsEnemy : MonoBehaviour
{
    Enemy enemyParents;

    private void Start()
    {
        enemyParents = GetComponentInParent<Enemy>();
    }

    //ダメージ処理呼出し
    public virtual void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        enemyParents.Damage(power, skill, isHitStop, exSkill);
    }
}
