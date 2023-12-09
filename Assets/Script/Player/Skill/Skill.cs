using System;
using UnityEngine;

[Serializable]
public class Skill
{
    //�X�L�����X�g
    public enum Type
    {
        NormalAttack,//�ʏ�U��
        DropAttack,  //���U��
        SideAttack,  //���ړ��U��
        UpAttack,    //�㏸�U��
        ExAttack     //�K�E�Z
    }

    public Type type;       // ���

    [SerializeField, Header("�_���[�W")]
    public float damage;�@�@ // �_���[�W(�U����)

    [SerializeField, Header("��������")]
    public float activeTime;

    [SerializeField,Header("�ړ�����")]
    public float distance;   // �ړ�����

    [SerializeField, Header("�X�L���̐���")]
    public String skillText; // �X�L���̐�����

    [SerializeField,Header("�q�b�g�G�t�F�N�g�����p�x")]
    public int hitEffectAngle;

    public float coolTime;

    public Skill(Type type, float damage,float activeTime, float distance, String skillTxet, int hitEffectAngle, float coolTime)
    {
        this.type = type;
        this.damage = damage;
        this.activeTime = activeTime;
        this.distance = distance;
        this.skillText = skillTxet;
        this.hitEffectAngle = hitEffectAngle;
        this.coolTime = coolTime;
    }
}