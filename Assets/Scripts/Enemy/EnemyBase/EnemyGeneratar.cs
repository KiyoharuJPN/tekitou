using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
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

    private Enemy DamgeEnemy;
    private Coroutine m_Corutine;
    private Tween tween;

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

    //�G�̃_���[�W����
    public async UniTask HitStopProcess(float power, Transform transform)
    {
        Vector3 initialPos = transform.position;//�����ʒu�ۑ�
        Time.timeScale = 0;

        var stopTime = power * stopState.shakTime;
        if (stopTime > stopState.shakTimeMax)
        {
            stopTime = stopState.shakTimeMax;
        }

        //�q�b�g�X�g�b�v�����J�n
        await transform.DOShakePosition(stopTime, stopState.shakPowar, stopState.shakNum, stopState.shakRand)
            .SetUpdate(true)
            .SetLink(transform.gameObject)
            .OnComplete(() =>
            {
                
                //�����ʒu�ɖ߂�
                transform.position = initialPos;
            });

        //�A�j���[�V�������I�������玞�Ԃ�߂�
        if (!GameManager.Instance.PauseCheck)
        {
            Time.timeScale = 1;
        }
    }
}
