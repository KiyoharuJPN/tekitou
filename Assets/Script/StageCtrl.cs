using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCtrl : MonoBehaviour
{
    [Header("プレイヤーゲームオブジェクト")] public GameObject playerObj;
    [Header("コンティニュー位置")] public GameObject[] continuePoint;

    //クラッシュ対策の長押し時間
    private float startButtonTime = 5f;
    private float getKayTime = 0;

    virtual protected void Start()
    {
        Time.timeScale = 1f;
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && !SceneData.Instance.wayPoint_1 && !SceneData.Instance.wayPoint_2)
        {
            playerObj.transform.position = continuePoint[0].transform.position;
        }
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && SceneData.Instance.wayPoint_1)
        {
            playerObj.transform.position = continuePoint[1].transform.position;
        }
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && SceneData.Instance.wayPoint_2)
        {
            playerObj.transform.position = continuePoint[2].transform.position;
        }
    }

    virtual protected void Update()
    {
        if(!Input.GetKey("joystick button 7"))
        {
            getKayTime = 0;
        }
        else if (Input.GetKey("joystick button 7"))
        {
            getKayTime += Time.unscaledDeltaTime;

            if (getKayTime > startButtonTime)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
