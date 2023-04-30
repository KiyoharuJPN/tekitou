using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Item
{
    GameObject player;
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    override protected void OnTriggerEnter2D(Collider2D collision)
    {
        player.GetComponent<PlayerController>()._Heel(itemData.resilience);
        base.OnTriggerEnter2D(collision);
    }
}