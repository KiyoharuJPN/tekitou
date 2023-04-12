using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightMeter : MonoBehaviour
{
    public Image lightMeter;
    public Image darkCanvas;
    public void updateLightMeter(float colorpre)
    {
        lightMeter.fillAmount = colorpre;
        //Debug.Log(lightMeter.fillAmount);
    }
}

