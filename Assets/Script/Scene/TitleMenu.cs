using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class TitleMenu : MonoBehaviour
{
    [Tooltip("接続したいシーンの名前を入れてください"),Header("接続したいシーンの名前")]
    public string gameScene;
    [Tooltip("今の選択を示すポインターです"),Header("トライアングルポインター")]
    public GameObject target;

    public GameObject[] menuobj;            //メニュー画面のオブジェクト

    [Header("背景画像のサイズ")]
    [SerializeField]
    Vector2 backgroundSpriteSizes = new Vector2(1500, 2000);
    [SerializeField]
    GameObject[] backGround;
    [SerializeField]
    Animator player;

    [SerializeField]
    LoadFadeImage fade;

    //ポインターと一個前のポインター
    int pointer;
    int pointerpreb;

    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;//各種チェック用関数

    private void Start()
    {
        SceneData.Instance.referer = "Title";
        pointer = 0;            //ポインターの初期化

        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
    }

    private void Update()
    {
        //BackGroundMove();
        //調整キーの設定
        //KeyboardChangePoint()
        if(!upDownLock) StickerChangePointer();

        //ポインターが変わった時の設定
        if (pointer != pointerpreb)//変更されたときの作業
        {
            if (menuobj[0].activeSelf)//Menu
            {
                if (pointer < 0) pointer = 0;// menuobj.Length - 1;
                if (pointer > menuobj.Length - 1) pointer = menuobj.Length - 1;// 0;//上限調整

                target.transform.position = new Vector2(target.transform.position.x, menuobj[pointer].transform.position.y);
                //OnSelected(menuobj[pointer]);
                //if(pointer !=pointerpreb && pointerpreb != -1) OnDeselected(menuobj[pointerpreb]);
            }

            //ポインターの修正
            pointerpreb = pointer;
        }

        //選択キーの設定
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0"))
        {
            if (menuobj[0].activeSelf && !hideKeyChecking)//Menu
            {
                switch (pointer)
                {
                    case 0:
                        GameStart();
                        //OnDeselected(menuobj[pointer]);
                        break;
                    case 1:
                        Exit();
                        //OnDeselected(menuobj[pointer]);
                        break;
                    default:
                        if (pointer < menuobj.Length - 1)
                        {
                            //OnDeselected(menuobj[pointer]);
                            Debug.Log("新しい項目を追加するときはプログラマに頼んでください。");
                        }
                        break;
                }
                hideKeyChecking = true;
            }

            //ポインターの復元
            if (!volumeChecking&&!inlineVolumeChecking)
            {
                pointer = 0;
                pointerpreb = -1;
            }
            inlineVolumeChecking = false;
            hideKeyChecking = false;
        }

        //戻るキーの設定
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown("joystick button 1"))
        {
            //menu
            if (menuobj[0].activeSelf)
            {
                pointer = 0;
            }

            //ポインターの復元
            if (!inlineVolumeChecking)
            {
                pointer = 0;
                pointerpreb = -1;
            }
            inlineVolumeChecking = false;
        }
    }

    //private void BackGroundMove()
    //{
    //    //1画像分進んだ時、スクロールが繋がるように良い感じに戻している。
    //    for (int i = 0; i < backgroundMax; i++)
    //    {
    //        backgroundScrollValues[i] -= (playerPosition.x - previousPlayerPosition.x) * scrollRates[i];

    //        if (backgroundSpriteSizes.x < backgroundsRt[i].anchoredPosition.x)
    //        {
    //            backgroundScrollValues[i] -= backgroundSpriteSizes.x;
    //            tempBackgroundsPosition.Set(backgroundSpriteSizes.x, 0);
    //            backgroundsRt[i].anchoredPosition -= tempBackgroundsPosition;
    //        }
    //        else if (backgroundsRt[i].anchoredPosition.x < -backgroundSpriteSizes.x)
    //        {
    //            backgroundScrollValues[i] += backgroundSpriteSizes.x;
    //            tempBackgroundsPosition.Set(backgroundSpriteSizes.x, 0);
    //            backgroundsRt[i].anchoredPosition += tempBackgroundsPosition;
    //        }
    //    }
    //}

    //メニューの動き
    void GameStart()
    {
        upDownLock = true;
        StartCoroutine(Scene_Start());
    }

    void Exit()
    {
        upDownLock = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

    void StickerChangePointer()
    {
        if (Input.GetAxis("Vertical") > 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer--;
        }
        if (Input.GetAxis("Vertical") < 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer++;
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            pointerCheck = true;
        }
    }

    IEnumerator Scene_Start()
    {
        player.SetTrigger("Start");
        SoundManager.Instance.PlaySE(SESoundData.SE.GoalSE);
        yield return new WaitForSeconds(2.4f);

        fade.StartFadeOut();
        while (!fade.IsFadeOutComplete()) 
        {
            yield return null;
        }

        if (gameScene != "") SceneManager.LoadScene(gameScene);
    }

    //内部動き
    void OnSelected(GameObject obj)
    {
        obj.GetComponent<Image>().color = Color.grey;               //UIの色修正
    }
    void OnDeselected(GameObject obj)
    {
        obj.GetComponent<Image>().color = new Color(255, 255, 255); //色を戻す
    }
}
