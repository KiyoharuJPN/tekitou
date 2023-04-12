using UnityEngine.Rendering.Universal;
using UnityEngine;

public class LightController : MonoBehaviour
{
    bool playerHasReached;
    new Light2D light;
    public AudioClip lightClip;
    private void Start()
    {
        light = GetComponent<Light2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!playerHasReached)
            {
                light.intensity = 1;
                playerHasReached = true;
                EffectSoundSource.instance.PlaySE(lightClip);
            }
        }
    }
}
