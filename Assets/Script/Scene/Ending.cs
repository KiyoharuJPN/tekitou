using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField]
    private FadeImage fade;

    //プレイ時間表示TextObj
    [SerializeField]
    Canvas playTimeResult;
    [SerializeField]
    bool IsPlayTimeObj = false; //プレイタイムの表示確認
    [SerializeField]
    GameObject anyKeyDownImage;
    [SerializeField]
    private TextMeshProUGUI[] playTime_Stage;
    [SerializeField]
    private TextMeshProUGUI totalTime;

    float toBeConTime = 5;

    private float playTime;
    private void Start()
    {
        playTime = SceneData.Instance.playTime;
        Cursor.visible = false;

        //プレイ時間セット
        playTime_Stage[0].text = getTimeString(SceneData.Instance.stage1Time);
        playTime_Stage[1].text = getTimeString(SceneData.Instance.stage2Time);
        playTime_Stage[2].text = getTimeString(SceneData.Instance.stage3Time);
        //トータルタイム表示
        totalTime.text = getTimeString(SceneData.Instance.stage1Time +
            SceneData.Instance.stage2Time +
            SceneData.Instance.stage3Time);
    }

    void Update()
    {
        if (!fade.IsFadeInComplete()) return;

        if ((Input.anyKeyDown || toBeConTime <= 0) && !IsPlayTimeObj)
        {
            SceneData.Instance.playTime = 0;
            playTimeResult.enabled = true;
            IsPlayTimeObj = true;
            toBeConTime = 2;
        }
        else if(!IsPlayTimeObj)
        {
            toBeConTime -= Time.deltaTime;
        }

        if (IsPlayTimeObj)
        {
            if (toBeConTime < 0)
            {
                anyKeyDownImage.SetActive(true);
                if (Input.anyKeyDown)
                {

                    SceneData.Instance.stage1Time = 0;
                    SceneData.Instance.stage2Time = 0f;
                    SceneData.Instance.stage3Time = 0f;
                    SceneManager.LoadScene("Title");
                }
            }
            else
            {
                toBeConTime -= Time.deltaTime;
            }
        }
    }

    string getTimeString(float time)
    {
        int sec = (int)time;
        int mm = sec / 60;
        int ss = sec % 60;
        return mm.ToString("D2") + ":" + ss.ToString("D2");
    }
}
