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

    //������ъp�x
    [Serializable]
    public struct BlowOffAngle
    {
        public float x;
        public float y;
    }
    [SerializeField]
    [Header("������ъp�x")]
    public BlowOffAngle blowOffAngle;
    
    public EnemyData(float hp, float power, moveType type,  float speed,
                     int score, float knockBackValue, BlowOffAngle blowOffAngle)
    {
        this.hp = hp;
        this.power = power;
        this.type = type;
        this.speed = speed;
        this.score = score;
        this.knockBackValue = knockBackValue;
        this.blowOffAngle = blowOffAngle;
    }
}