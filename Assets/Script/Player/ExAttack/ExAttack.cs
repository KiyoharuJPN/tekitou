using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExAttack
{
    public static void ExAttackStart(PlayerController player)
    {
        player.animator.SetBool("IsExAttack", true);
        player.exAttackEnemylist.Clear();
        player.rb.velocity = Vector2.zero;

        //�Q�[�W������ۂ�
        ExAttackParam.Instance.EXAttack();
        //�G�Z�b�g
        player.exAttacArea.ExAttackEnemySet();
        //�K�E�Z�ׂ̈̒�~����
        GameManager.Instance.PlayerExAttack_Start();
        //�K�E�Z�J�b�g�C��
        ExAttackCutIn.Instance.StartCoroutine("_ExAttackCutIn", player);
    }
}
