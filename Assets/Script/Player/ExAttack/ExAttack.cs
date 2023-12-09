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

        //ゲージを空っぽに
        ExAttackParam.Instance.EXAttack();
        //敵セット
        player.exAttacArea.ExAttackEnemySet();
        //必殺技の為の停止処理
        GameManager.Instance.PlayerExAttack_Start();
        //必殺技カットイン
        ExAttackCutIn.Instance.StartCoroutine("_ExAttackCutIn", player);
    }
}
