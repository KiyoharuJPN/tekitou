using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Reflection;
using Steamworks;

public class TitleMenu : MonoBehaviour
{
    [Tooltip("接続したいシーンの名前を入れてください"),Header("接続したいシーンの名前")]
    public string gameScene;
    [Tooltip("今の選択を示すポインターです"),Header("トライアングルポインター")]
    public GameObject target;

    [SerializeField] private OptionMenu optionMenu;
    [SerializeField] private StartConfirmUI startCfmUI;
    private MenuSystem openMenu;

    enum SelectMenu
    {
        START = 0,
        OPTION = 1,
        EXIT = 2,
        FIRST = 3, //最初から
        CONTD = 4　//続きから
    }
    private SelectMenu selectMenu = 0;

    public Text[] menuObj;            //メニュー画面のオブジェクト
    public GameObject titleObj;
    public GameObject startObj;            //スタート画面のオブジェクト

    [SerializeField]
    GameObject backGround;

    [SerializeField]
    Animator player;

    [SerializeField]
    LoadFadeImage fade;

    bool canStart = true;

    private bool isStartMenu = false;
    private bool isPointerMove = true, upDownLock = false;//各種チェック用関数

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
    private PlayerInput playerInput;
    internal InputAction back, decision, move;

    private Color color = new Color(255, 69, 0);

    SeveData seveData;

    private void Awake()
    {
        SettingData settingData = SeveSystem.Instance.SettingLoad();

        if(settingData != null)
        {
            SceneData.Instance.SetVolume(settingData.bgmValum, settingData.seValum);
        }

        if (Accmplisment.Instance.GameStart())
        {
            SteamUserStats.RequestCurrentStats();

            SteamUserStats.ResetAllStats(true);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        SceneData.Instance.referer = "Title";
        SceneData.Instance.StageStateReset();
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
        videoImage.enabled = false;
        videoPlayer.enabled = false;

        playerInput = GetComponent<PlayerInput>();
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

        if (openMenu != null)
        {
            openMenu.MenuUpdata();
            return;
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

        if (!fade.IsFadeInComplete()) return; //フェード中は以下の処理は行わない

        //選択キーの設定
        if (decision.WasPressedThisFrame())
        {
            SelectMenuProcess();
        }
    }

    public void SetMenu(MenuSystem menu)
    {
        openMenu = menu;
        if(openMenu != null)
        {
            openMenu.InputSet(playerInput);
        }
    }

    public void MenuBack()
    {
        openMenu = openMenu.Back();
        if (openMenu != null)
        {
            openMenu.InputSet(playerInput);
        }
        else openMenu = optionMenu;
    }

    private void SelectMenuProcess()
    {
        switch (selectMenu)
        {
            case SelectMenu.START:
                StartModeCheck();
                break;
            case SelectMenu.OPTION:
                optionMenu.gameObject.SetActive(true);
                SetMenu(optionMenu);
                break;
            case SelectMenu.EXIT:
                Exit();
                break;
            case SelectMenu.FIRST:
                startCfmUI.gameObject.SetActive(true);
                SetMenu(startCfmUI);
                break;
            case SelectMenu.CONTD:
                StageSelectStart();
                break;
        }
    }

    public void TitelMenuOpen()
    {
        openMenu = null;
        isStartMenu = false;
        OnDeselected((int)selectMenu);
        selectMenu = SelectMenu.START;
        OnSelected((int)selectMenu);
        startObj.SetActive(false);
        titleObj.SetActive(true);
    }

    private void DemoMove()
    {
        if(canDemoVideo)
        {
            canDemoVideo = false;
            StartCoroutine(DemoVideoPlay());
        }
    }

    //スタート状態選択の処理（初めから・続きから）
    void StartModeCheck()
    {
        seveData = SeveSystem.Instance.seveDataLoad();
        if (seveData != null)
        {
            isStartMenu = true;
            OnDeselected((int)selectMenu);
            selectMenu = SelectMenu.FIRST;
            OnSelected((int)selectMenu);
            titleObj.SetActive(false);
            startObj.SetActive(isStartMenu);
        }
        else //セーブデータがなかった場合
        {
            GameStart();
        }
    }

    //メニューの動き
    public void GameStart()
    {
        if (!canStart) return;
        upDownLock = true;
        StartCoroutine(Scene_Start(gameScene));
        canStart = false;
    }
    public void StageSelectStart()
    {
        SceneData.Instance.GetEachStageState = seveData.stageState;
        SceneData.Instance.stock = seveData.remain;

        upDownLock = true;
        StartCoroutine(Scene_Start("StageSelect"));
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

        if (!isStartMenu) //スタートメニューが開いていない時
        {
            if (input > 0.3f && (int)selectMenu > 0 && isPointerMove)
            {
                isPointerMove = false;
                ChangePointer(-1);
            }
            if (input < -0.3f && (int)selectMenu < 2 && isPointerMove)
            {
                isPointerMove = false;
                ChangePointer(1);
            }
        }
        else if (isStartMenu)
        {
            if (input > 0.3f && (int)selectMenu > 3 && isPointerMove)
            {
                isPointerMove = false;
                ChangePointer(-1);
            }
            if (input < -0.3f && (int)selectMenu < 4 && isPointerMove)
            {
                isPointerMove = false;
                ChangePointer(1);
            }
        }

        
        if (input == 0)
        {
            isPointerMove = true;
        }
    }

    void ChangePointer(int pointer)
    {
        OnDeselected((int)selectMenu);
        selectMenu += pointer;
        OnSelected((int)selectMenu);
    }

    IEnumerator Scene_Start(string sceneName)
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

        if (sceneName != "") SceneManager.LoadScene(sceneName);
    }

    void OnSelected(int objNum)
    {
        target.transform.position = new Vector2(target.transform.position.x, menuObj[objNum].transform.position.y);

        menuObj[objNum].color = color;    //UIの色変更
    }
    void OnDeselected(int objNum)
    {
        menuObj[objNum].color = Color.white; //色を戻す
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
