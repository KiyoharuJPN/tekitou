using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Item
{
    bool OnCamera = false;
    //画面に入ったどうかをチェック
    protected void OnBecameVisible()
    {
        this.GetComponent<Animator>().enabled = true;
    }
    protected void OnBecameInvisible()
    {
        this.GetComponent<Animator>().enabled = false;
    }

    override protected void OnTriggerEnter2D(Collider2D collision)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.GetCoin);
        PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + itemData.score);
        ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo()+1);
        ComboParam.Instance.ResetTime();
        base.OnTriggerEnter2D(collision);
    }
}
