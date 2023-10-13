using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Demo : PlayerController
{
    protected override void InputKay()
    {
        float lsh = Input.GetAxis("L_Stick_H");
        float lsv = Input.GetAxis("L_Stick_V");
        //�ȈՓ��͂Ŏg�p
        //float rsh = Input.GetAxis("R_Stick_H");
        //float rsv = Input.GetAxis("R_Stick_V");

        if (Input.GetKey(KeyCode.JoystickButton2))
        {
            isAttackKay = true;
        }
        else { isAttackKay = false; }

        //�㏸�U��
        if (lsv >= 0.9 && lsh <= 0.45 && isAttackKay)
        //rsv >= 0.8
        {
            AttackAction("UpAttack");
        }
        //�����U���U��
        if (lsv <= -0.9 && lsh <= 0.45 && isAttackKay)
        //rsv <= -0.8
        {
            AttackAction("DawnAttack");
        }
        //���ړ��U��
        if (lsh >= 0.9 && lsv <= 0.45 && isAttackKay)
        {
            AttackAction("SideAttack_right");
        }
        else if (lsh <= -0.9 && lsv <= 0.45 && isAttackKay)
        {
            AttackAction("SideAttack_left");
        }
        //�K�E�Z
        if (Input.GetKey(KeyCode.JoystickButton4) && Input.GetKey(KeyCode.JoystickButton5))
        {
            if (!isAttack && canExAttack)
            {
                AttackAction("ExAttack");
            }
        }
        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (Input.GetKeyDown(KeyCode.JoystickButton2) && canNomalAttack)
        {
            //�ʏ�U������
            AttackAction("NomalAttack");
        }
        if (Input.GetKey(KeyCode.JoystickButton2) && canNomalAttack)
        {
            //�ʏ�U����������
            AttackAction("NomalAttack");
        }
    }
}
