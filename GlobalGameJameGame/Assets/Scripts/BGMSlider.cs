using UnityEngine.UI;
using UnityEngine;

public class BGMSlider : MonoBehaviour
{
    public Slider BGM;
    void Start()
    {
        BGM.value = BGMSource.instance.GetVolume();
    }

    void Update()
    {
        if(BGMSource.instance.GetVolume() != BGM.value)
        {
            BGMSource.instance.Volume(BGM.value);
        }
    }
}
