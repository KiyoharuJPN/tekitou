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

    public float hp;           //�̗�
    public float power;     //�U����
    public moveType type;      //�s�����
    public float speed;        //�f����
    public int score;            //����X�R�A
    public float knockBackValue; //�m�b�N�o�b�N�l

    [SerializeField]
    [Header("������ъp�x")]
    public float angle;
    [SerializeField]
    [Header("������щ�")]
    public int num;

    public EnemyData(float hp, float power, moveType type,  float speed,
                     int score, float knockBackValue, float angle, int num)
    {
        this.hp = hp;
        this.power = power;
        this.type = type;
        this.speed = speed;
        this.score = score;
        this.knockBackValue = knockBackValue;
        this.angle = angle;
        this.num = num;
    }
}