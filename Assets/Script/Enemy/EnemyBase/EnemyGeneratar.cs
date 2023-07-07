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

    public static EnemyGeneratar instance;

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
