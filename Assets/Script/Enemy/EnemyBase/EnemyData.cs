using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public string id;          //�o�^ID

    public string charName;    //�L�����N�^�[�̖��O

    public enum moveType
    {
        NotMove, //�����Ȃ�
        Move,    //����
        FlyMove�@//���
    }

    public float hp;                //�̗�
    public int score;               //����X�R�A
    public float attackPower;       //�U����
    public float power;             //�ڐG�_���[�W
    public float knockBackValue;    //�m�b�N�o�b�N�l

    [SerializeField]
    [Header("������ъp�x")]
    public float angle;
    [SerializeField]
    [Header("������щ�")]
    public int num;

    public moveType type;           //�s�����
    public float speed;             //�f����


    public EnemyData(float hp, int score, float attackPower, float power,
                     float knockBackValue, float angle, int num, moveType type,  
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