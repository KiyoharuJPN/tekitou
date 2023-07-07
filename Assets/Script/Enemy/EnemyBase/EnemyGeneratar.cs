using UnityEngine;

public class EnemyGeneratar : MonoBehaviour
{
    [SerializeField]
    EnemyListEntity EnemyListEntity;

    [SerializeField, Header("������ё��x")]
    public float speed = 15f;
    //������ђ��̉��G�t�F�N�g
    [SerializeField, Header("������ђ��̉��G�t�F�N�g")]
    public GameObject smokeEffect;
    [SerializeField, Header("���G�t�F�N�g�����Ԋu")]
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
