using System;
using UnityEngine;

//
[Serializable]
public class Skill
{
    //スキルリスト
    public enum Type
    {
        NormalAttack,//通常攻撃
        Stabbing,    //突き刺し
        IaiCut,      //居合切り（仮称）
        RoundingUp,  //切り上げ（仮称）
        ExAttack     //必殺技
    }

    public Type type;     　 // 種類
    public float damage;　　 // ダメージ(攻撃力)
    public float distance;   // 移動距離
    public String skillText; // スキルの説明文

    public bool coolTime;

    public Skill(Type type, float damage, float distance, String skillTxet, bool coolTime)
    {
        this.type = type;
        this.damage = damage;
        this.distance = distance;
        this.skillText = skillTxet;
        this.coolTime = coolTime;
    }
}