using UnityEngine;

public class EnemyGeneratar : MonoBehaviour
{
    [SerializeField]
    EnemyListEntity EnemyListEntity;

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
