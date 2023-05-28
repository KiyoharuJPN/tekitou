using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExAttackArea : MonoBehaviour
{
    PlayerController player;

    private void Start()
    {
        player = transform.parent.gameObject.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy") && !player.enemylist.Contains(other.gameObject))
        {
            player.enemylist.Add(other.gameObject);
        }
    }
}
