using System.Drawing;
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
    [SerializeField, Header("���Ŏ��G�t�F�N�g")]
    public GameObject deathEffect;
    public static EnemyGeneratar instance;

    [System.Serializable]
    public struct HitStopState
    {
        [SerializeField, Header("�h��̎���")]
        public float shakTime;
        [SerializeField, Header("���ԏ���")]
        public float shakTimeMax;
        [SerializeField, Header("�h��̋���")]
        public float shakPowar;
        [SerializeField, Header("�U����")]
        public int shakNum;
        [SerializeField, Header("�h��̃����_����")]
        public int shakRand;
    }

    [SerializeField, Header("�q�b�g�X�g�b�v")]
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
