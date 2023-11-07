using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PBF.PlayerBuffBase;

public class BuffTimer : MonoBehaviour
{
    [SerializeField, Header("ÉQÅ[ÉW")]
    Image timeBarImg;

    public void BarSet(float fillAmount)
    {
        timeBarImg.fillAmount = 1;
        timeBarImg.fillAmount -= fillAmount;
    }
}
