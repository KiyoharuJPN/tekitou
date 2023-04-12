using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class DarkController : MonoBehaviour
{
    public PlayerController playerController;
    public Light2D globalLight;
    public float darkSpeed = 0.01f,countTimeDark = 30, lightSpeed = 0.1f, countTimeLight = 10;
    private bool isWater;
    bool inCoroutine, inLighting = false, inDarking = false, playerLighting;
    public Image lightMeterUI;

    private void Start()
    {
        isWater = false;

    }
    private void Update()
    {
        //globalLight.intensity = lightMeterUI.fillAmount;
        playerLighting = lightMeterUI.fillAmount > 0;
        //Debug.Log(globalLight.intensity);

        if (isWater)
        {
            if (!inLighting)
            {
                if (inCoroutine)
                {
                    StopAllCoroutines();
                    inCoroutine = false;
                }
                StartCoroutine(ChangeDisplay(countTimeLight, lightSpeed, 1));
                inCoroutine = true;
                inLighting = true;
                inDarking = false;
            }
        }
        else
        {
            if (!inDarking)
            {
                if (inCoroutine)
                {
                    StopAllCoroutines();
                    inCoroutine = false;
                }
                StartCoroutine(ChangeDisplay(countTimeDark, -darkSpeed, 0));
                inCoroutine = true;
                inDarking = true;
                inLighting = false;
            }
        }
        //Debug.Log(inCoroutine+"coroutine"+inDarking+"darking"+inLighting+"lighting");
    }

    IEnumerator ChangeDisplay(float time,float speed,float color_a)
    {
        //globalLight.color = new Color(globalLight.color.r, globalLight.color.g, globalLight.color.b, globalLight.color.a + speed);
        if (isWater)
        {
            while(globalLight.intensity <= color_a)
            {
                globalLight.intensity += speed;
                lightMeterUI.GetComponent<LightMeter>().updateLightMeter(globalLight.intensity);
                yield return new WaitForSeconds(time);
            }
            /*while (globalLight.color.a >= color_a)
            {
                globalLight.color = new Color(globalLight.color.r, globalLight.color.g, globalLight.color.b, globalLight.color.a + speed);
                Debug.Log(globalLight.color);
                lightMeterUI.GetComponent<LightMeter>().updateLightMeter(1 - globalLight.color.a);
                yield return new WaitForSeconds(time);
            }*/
        }
        else
        {
            while(globalLight.intensity >= color_a)
            {
                globalLight.intensity += speed;
                lightMeterUI.GetComponent<LightMeter>().updateLightMeter(globalLight.intensity);
                yield return new WaitForSeconds(time);
            }
            /*while (globalLight.color.a <= color_a)
            {
                globalLight.color = new Color(globalLight.color.r, globalLight.color.g, globalLight.color.b, globalLight.color.a + speed);
                Debug.Log(globalLight.color);
                lightMeterUI.GetComponent<LightMeter>().updateLightMeter(1 - globalLight.color.a);
                yield return new WaitForSeconds(time);
            }*/
        }
    }

    public void SetIsWater(bool water)
    {
        isWater = water;
    }

    public bool GetPlayerLighting()
    {
        return playerLighting;
    }
}
