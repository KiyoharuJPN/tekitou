using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StageCtrl : MonoBehaviour
{
    [Header("プレイヤーゲームオブジェクト")] public GameObject playerObj;
    [Header("コンティニュー位置")] public GameObject[] continuePoint;

    //クラッシュ対策の長押し時間
    private float startButtonTime = 5f;
    private float getKayTime = 0;

    //プレイ時間計測
    public bool playTimeMeasurement = true;
    private float playTime = 0;

    //InputSystem
    internal InputAction option;

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
        Cursor.visible = false;

        var playerInput = GameManager.Instance.playerInput;
        option = playerInput.actions["Option"];
    }

    virtual protected void Update()
    {
        //強制終了
        if(!option.IsPressed())
        {
            getKayTime = 0;
        }
        else if (option.IsPressed())
        {
            getKayTime += Time.unscaledDeltaTime;

            if (getKayTime > startButtonTime)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }


        //プレイ時間取得
        if (playTimeMeasurement)
        {
            SceneData.Instance.playTime += Time.deltaTime;
        }
        
        Debug.Log(SceneData.Instance.playTime);
    }

    void playTimeStop()
    {

    }
}
