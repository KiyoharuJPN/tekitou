using System;

[Serializable]
public class EnemyData
{
    public string id;          //登録ID

    public string charName;    //キャラクターの名前

    public enum moveType
    {
        NotMove, //動かない
        Move,    //動く
        FlyMove　//飛ぶ
    }

    public float hp;           //体力
    public float power;     //攻撃力
    public moveType type;      //行動種類
    public float speed;        //素早さ
    public int score;            //入手スコア
    public float knockBackValue; //ノックバック値

    public EnemyData(float hp, float power, moveType type,  float speed,int score, float knockBackValue)
    {
        this.hp = hp;
        this.power = power;
        this.type = type;
        this.speed = speed;
        this.score = score;
        this.knockBackValue = knockBackValue;
    }
}