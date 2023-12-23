using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class TitleMenu : MonoBehaviour
{
    [Tooltip("接続したいシーンの名前を入れてください"),Header("接続したいシーンの名前")]
    public string gameScene;
    [Tooltip("今の選択を示すポインターです"),Header("トライアングルポインター")]
    public GameObject target;

    public GameObject[] menuobj;            //メニュー画面のオブジェクト

    [SerializeField]
    GameObject backGround;

    [SerializeField]
    Animator player;

    [SerializeField]
    LoadFadeImage fade;

    bool canStart = true;

    //ポインターと一個前のポインター
    int pointer;
    int pointerpreb;

    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;//各種チェック用関数

    //デモムービー再生状態
    [SerializeField, Header("videoPlayer")]
    UnityEngine.Video.VideoPlayer videoPlayer;
    [SerializeField, Header("DmoVideoImage")]
    RawImage videoImage;
    [SerializeField, Header("PreesAnyKeyObj")]
    GameObject preeskey;
    [SerializeField, Header("デモムービー移行必要時間")]
    float demoVideoMoveTime = 40f;
    [SerializeField, Header("デモムービー表示時間")]
    float demoVideoTime = 20f;
    bool isDemoVideo = false;
    bool canDemoVideo = true;
    float demoTimer;

    //InputSystem
    internal InputAction back, decision, move;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        SceneData.Instance.referer = "Title";
        pointer = 0;            //ポインターの初期化
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
        videoImage.enabled = false;
        videoPlayer.enabled = false;

        var playerInput = GetComponent<PlayerInput>();
        decision = playerInput.actions["Decision"];
        back = playerInput.actions["Back"];
        move = playerInput.actions["Move"];
    }

    private void Update()
    {

        if (!SoundManager.Instance.isPlayBGM())
        {
            StartCoroutine(PlayBGM());
        }

        //デモムービー中
        if (isDemoVideo)
        {
            DemoMove();
            return;
        }

        if (!InputKeyCheck.GetAnyKey())
        {
            demoTimer += Time.deltaTime;
            if (demoTimer >= demoVideoMoveTime)
                isDemoVideo = true;
        }
        else if (InputKeyCheck.GetAnyKey())
        {
            demoTimer = 0;
        }

        //調整キーの設定
        if (!upDownLock) StickerChangePointer();
        
        //ポインターが変わった時の設定
        if (pointer != pointerpreb)//変更されたときの作業
        {
            demoTimer = 0;
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

        if (!fade.IsFadeInComplete())
        {
            return;
        }

        //選択キーの設定
        if (decision.WasPressedThisFrame())
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
        if (back.WasPressedThisFrame())
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

    private void DemoMove()
    {
        if(canDemoVideo)
        {
            canDemoVideo = false;
            StartCoroutine(DemoVideoPlay());
        }
    }

    private void BackGroundMove()
    {
        //backGround.transform.position -= new Vector3(Time.deltaTime * 10f, 0);
    }

    //メニューの動き
    void GameStart()
    {
        if (!canStart) return;
        upDownLock = true;
        StartCoroutine(Scene_Start());
        canStart = false;
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
        var input = move.ReadValue<Vector2>().y;
        if (input > 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer--;
        }
        if (input < 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer++;
        }
        if (input == 0)
        {
            pointerCheck = true;
        }
    }

    IEnumerator Scene_Start()
    {
        player.SetTrigger("Start");
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_CutIn);
        yield return new WaitForSeconds(2.4f);

        fade.StartFadeOut();
        SceneData.Instance.StageDataReset();

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

    IEnumerator PlayBGM()
    {
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
    }

    IEnumerator DemoVideoPlay()
    {
        demoTimer = 0;
        //フェードアウト開始
        fade.StartFadeOut();
        while (!fade.IsFadeOutComplete())
        {
            //キーが押されたら終了
            if (InputKeyCheck.GetAnyKey())
            {
                DemoVideoEnd(); yield break;
            }
            yield return null;
        }
        videoImage.enabled = true;
        videoPlayer.enabled = true;
        videoPlayer.Play();
        preeskey.SetActive(true);

        fade.StartFadeIn();
        while (!fade.IsFadeInComplete()) 
        {
            if (InputKeyCheck.GetAnyKey()) { DemoVideoEnd(); yield break; }
            yield return null;
        }

        while(demoTimer <= demoVideoTime) 
        {
            demoTimer += Time.deltaTime;
            if (InputKeyCheck.GetAnyKey()) { DemoVideoEnd(); yield break; }
            yield return null;
        };
        fade.StartFadeOut();
        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }
        DemoVideoEnd();
        fade.StartFadeIn();
        while (!fade.IsFadeInComplete())
        {
            yield return null;
        }
    }

    private void DemoVideoEnd()
    {
        fade.FadeStop();
        preeskey.SetActive(false);
        videoImage.enabled = false;
        videoPlayer.Stop();
        videoPlayer.enabled = false;
        demoTimer = 0;
        isDemoVideo = false;
        canDemoVideo = true;
    }
}
