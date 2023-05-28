using System;
using UnityEngine;

[Serializable]
public class Skill
{
    //スキルリスト
    public enum Type
    {
        NormalAttack,//通常攻撃
        DropAttack,  //下攻撃
        SideAttack,  //横移動攻撃
        UpAttack,    //上昇攻撃
        ExAttack     //必殺技
    }

    public Type type;     　 // 種類
    public float damage;　　 // ダメージ(攻撃力)
    public float distance;   // 移動距離
    public String skillText; // スキルの説明文
    [SerializeField]
    public int hitEffectAngle;//ヒットエフェクト発生角度

    public float coolTime;

    public Skill(Type type, float damage, float distance, String skillTxet, int hitEffectAngle, float coolTime)
    {
        this.type = type;
        this.damage = damage;
        this.distance = distance;
        this.skillText = skillTxet;
        this.hitEffectAngle = hitEffectAngle;
        this.coolTime = coolTime;
    }
}