using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    override protected void OnTriggerEnter2D(Collider2D collision)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.GetCoin);
        PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + itemData.score);
        ComboParam.Instance.ResetTime();
        base.OnTriggerEnter2D(collision);
    }
}
