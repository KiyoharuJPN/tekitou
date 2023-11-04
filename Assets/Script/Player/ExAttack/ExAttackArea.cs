using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExAttackArea : MonoBehaviour
{
    PlayerController player;
    GameObject[] enemys;

    private void Start()
    {
        player = transform.parent.gameObject.GetComponent<PlayerController>();
    }

    public void ExAttackEnemySet()
    {
        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        player.exAttackEnemylist.Clear();

        foreach (GameObject gameObj in enemys)
        {
            if (gameObj.GetComponent<Enemy>().OnCamera)
            {
                player.exAttackEnemylist.Add(gameObj);
            }
        }
    }
}
