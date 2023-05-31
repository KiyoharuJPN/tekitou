using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Item
{
    GameObject player;

    bool inCheck = false;
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    override protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!inCheck)
        {
            inCheck = true;
            Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            SoundManager.Instance.PlaySE(SESoundData.SE.GetHeart);
            player.GetComponent<PlayerController>()._Heel(itemData.resilience);
            base.OnTriggerEnter2D(collision);
        }
    }
}