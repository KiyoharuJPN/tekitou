using System.Drawing;
using UnityEngine;

public class EnemyGeneratar : MonoBehaviour
{
    [SerializeField]
    EnemyListEntity EnemyListEntity;

    [SerializeField, Header("吹っ飛び速度")]
    public float speed = 15f;
    //吹っ飛び中の煙エフェクト
    [SerializeField, Header("吹き飛び中の煙エフェクト")]
    public GameObject smokeEffect;
    [SerializeField, Header("煙エフェクト発生間隔")]
    public float effectInterval = 0.5f;
    [SerializeField, Header("消滅時エフェクト")]
    public GameObject deathEffect;
    public static EnemyGeneratar instance;

    [System.Serializable]
    public struct HitStopState
    {
        [SerializeField, Header("揺れの時間")]
        public float shakTime;
        [SerializeField, Header("時間条件")]
        public float shakTimeMax;
        [SerializeField, Header("揺れの強さ")]
        public float shakPowar;
        [SerializeField, Header("振動数")]
        public int shakNum;
        [SerializeField, Header("揺れのランダム性")]
        public int shakRand;
    }

    [SerializeField, Header("ヒットストップ")]
    public HitStopState stopState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public EnemyData EnemySet(string id)
    {
        foreach (EnemyData enemyData in EnemyListEntity.EnemyDataList)
        {
            if (enemyData.id == id)
            {
                return new EnemyData(enemyData.hp, enemyData.score, enemyData.attackPower, enemyData.power, 
                                     enemyData.knockBackValue, enemyData.angle, enemyData.num, enemyData.type, 
                                     enemyData.speed);
            }
        }
        return null;
    }

}
