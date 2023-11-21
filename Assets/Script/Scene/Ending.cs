using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField]
    private FadeImage fade;

    private float playTime;
    private void Start()
    {
        playTime = SceneData.Instance.playTime;
        Cursor.visible = false;
        Debug.Log("ëçÉvÉåÉCéûä‘:" + getTimeString(playTime));
    }

    void Update()
    {
        if (!fade.IsFadeInComplete()) return;
        if (Input.anyKeyDown)
        {
            SceneData.Instance.playTime = 0;
            SceneManager.LoadScene("Title");
        }
    }

    string getTimeString(float time)
    {
        int sec = (int)time;
        int mm = sec / 60;
        int ss = sec % 60;
        int ms = (int)(time * 100.0f) % 100;
        return mm.ToString("D2") + "'" + ss.ToString("D2") + "\"" + ms.ToString("D2");
    }
}
