using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public string id;          //登録ID

    public string charName;    //キャラクターの名前

    public enum EnemyType
    {
        GrundEnemy,    //動く
        FlyEnemy　//飛ぶ
    }

    public float hp;                //体力
    public int score;               //入手スコア
    public int attackPower;       //攻撃力
    public float power;             //接触ダメージ
    public float knockBackValue;    //ノックバック値

    [SerializeField]
    [Header("吹っ飛び角度")]
    public float angle;
    [SerializeField]
    [Header("吹っ飛び回数")]
    public int num;

    public EnemyType type;           //行動種類
    public float speed;             //素早さ


    public EnemyData(float hp, int score, int attackPower, float power,
                     float knockBackValue, float angle, int num, EnemyType type,  
                     float speed)
    {
        this.hp = hp;
        this.score = score;
        this.attackPower = attackPower;
        this.power = power;
        this.knockBackValue = knockBackValue;
        this.angle = angle;
        this.num = num;
        this.type = type;
        this.speed = speed;
    }
}