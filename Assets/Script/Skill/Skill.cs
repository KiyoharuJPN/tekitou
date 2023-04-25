using System;
using UnityEngine;

//
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

    public Type type;     �@ // ���
    public float damage;�@�@ // �_���[�W(�U����)
    public float distance;   // �ړ�����
    public String skillText; // �X�L���̐�����

    public float coolTime;

    public Skill(Type type, float damage, float distance, String skillTxet, float coolTime)
    {
        this.type = type;
        this.damage = damage;
        this.distance = distance;
        this.skillText = skillTxet;
        this.coolTime = coolTime;
    }
}