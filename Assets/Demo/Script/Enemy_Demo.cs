using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Demo : Enemy
{
    [SerializeField]
    private GameObject damageUI;

    override protected void Start()
    {
        ///“G‚Ìscript‚ÉŠî‚Ã‚­
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Gravity();
    }

    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        var obj = Instantiate<GameObject>(damageUI, this.transform.position + new Vector3(0,1.5f,0), Quaternion.identity);
        string SpriteText = power.ToString();
        obj.GetComponent<DamageUI>().damageText.text = SpriteText;
        base.Damage(power, skill, isHitStop, exSkill);
    }
}
