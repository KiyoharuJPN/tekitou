using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class DestroyBlink
{
    public float[] destroyBlinkSpeed;

    public void DestroyBlinkSpeedSet(out float[] speed)
    {
        speed = destroyBlinkSpeed;
    }
}
