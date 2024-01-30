using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBase : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [SerializeField] private Image handle;

    //”ñ‘I‘ðE‘I‘ð‚ÌUI
    [SerializeField] private Sprite onHandle;
    [SerializeField] private Sprite offHandle;

    public float GetSliderValue => slider.value;

    public void SetValue(float value)
    {
        slider.SetValueWithoutNotify(value);
    }

    public void Active(bool isActive)
    {
        handle.sprite = isActive ? onHandle : offHandle;
    }
}
