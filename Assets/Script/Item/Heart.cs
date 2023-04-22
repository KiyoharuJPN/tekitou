using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Item
{
    override protected void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<PlayerController>().Heel(itemData.resilience);
        base.OnTriggerEnter2D(collision);
    }
}