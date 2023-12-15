using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayTime : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playTimeObj;

    // Update is called once per frame

    private void FixedUpdate()
    {
        playTimeObj.text = getTimeString(SceneData.Instance.playTime);
    }

    //ÉvÉåÉCéûä‘ï∂èÕâª
    string getTimeString(float time)
    {
        int sec = (int)time;
        int mm = sec / 60;
        int ss = sec % 60;
        return mm.ToString("D2") + ":" + ss.ToString("D2");
    }
}
