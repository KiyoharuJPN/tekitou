using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PBF.PlayerBuffBase;

public class BuffTimer : MonoBehaviour
{
    [SerializeField, Header("ゲージ")]
    Image timeBarImg;

    public void BarSet(float fillAmount)
    {
        //Debug.Log("バフタイマーセット");
        timeBarImg.fillAmount = fillAmount;
    }
}
