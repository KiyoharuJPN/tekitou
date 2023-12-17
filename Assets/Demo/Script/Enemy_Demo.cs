using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Demo : Enemy
{
    [SerializeField]
    private GameObject damageUI;

    private int cmeraReflexNum = 2;

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

    protected override void OnColEnter2D(Collider2D col)
    {
        if (!isDestroy && HadContactDamage)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Attack(col);
            }
        }
        if (col.gameObject.CompareTag("Stage") && isDestroy)
        {
            if(col.gameObject.layer == 20)
            {
                cmeraReflexNum--;
                if (cmeraReflexNum <= 0)
                {
                    this.gameObject.layer = 27;
                }
            }
            reflexNum--;
            Debug.Log(reflexNum);
            if (reflexNum == 0)
            {
                EnemyNomalDestroy();
            }
        }
    }

    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DeadBoss")) return;
        var obj = Instantiate<GameObject>(damageUI, this.transform.position + new Vector3(0,1.5f,0), Quaternion.identity);
        string SpriteText = power.ToString();
        obj.GetComponent<DamageUI>().damageText.text = SpriteText;
        base.Damage(power, skill, isHitStop, exSkill);
    }
}
