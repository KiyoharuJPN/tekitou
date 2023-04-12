using UnityEngine.UI;
using UnityEngine;

public class EffectSoundSlider : MonoBehaviour
{
    public Slider SE;
    void Start()
    {
        SE.value = EffectSoundSource.instance.GetVolume();
    }

    void Update()
    {
        if (EffectSoundSource.instance.GetVolume() != SE.value)
        {
            EffectSoundSource.instance.Volume(SE.value);
        }
    }
}
