using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour, MenuSystem
{
    [SerializeField] string sceneName = "none";
    [Tooltip("今の選択を示すポインターです"), Header("トライアングルポインター")]
    public GameObject target;

    public string backScene;

    public GameObject[] menuobj;            //メニュー画面のオブジェクト
    public SoundSetting soundSetting;            //メニュー画面のオブジェクト
    //残り残機画像
    public Image stockImage;
    public Sprite[] stockImages;

    private MenuBasic basic;

    //メニュー表示確認Bool
    private bool isPauseMenu = false;

    public GameObject menuTextObj;
    private bool isMenuText = true;
    public GameObject actionExpoObj;
    private bool isActionExpo = false;
    public GameObject soundSettingObj;
    private bool isSoundSetting = false;
    internal bool IsSoundSetting {  get { return isSoundSetting; } }

    [SerializeField]
    Animator player;

    [SerializeField]
    LoadFadeImage fade;

    bool canStart = true;

    //ポインターと一個前のポインター
    int pointer;
    int pointerpreb;

    //各種チェック用関数
    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;

    //InputSystem
    public PlayerInput playerInput;
    internal InputAction back, decision, move;

    public float sliderMoveSpeed = 0.5f;

    private void Start()
    {
        pointer = 0;            //ポインターの初期化
    }

    public void InputSet(PlayerInput playerInput, MenuBasic menuBasic)
    {
        if(playerInput != null)
        {
            this.playerInput = playerInput;
        }
        basic = menuBasic;

        var input = this.playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"];
        move = input.actions["Move"];

        Time.timeScale = 0;
        isPauseMenu = true;
        isSoundSetting = false;
        stockImage.sprite = stockImages[SceneData.Instance.stock];
        this.GetComponent<Canvas>().enabled = true;
        menuTextObj.SetActive(true);
        isMenuText = true;
        basic.SetMenu(this);
    }

    public bool PauseCheck()
    {
        return isPauseMenu;
    }

    public void PauseStart()
    {
        Time.timeScale = 0;
        isPauseMenu = true;
        stockImage.sprite = stockImages[SceneData.Instance.stock];
        this.GetComponent<Canvas>().enabled = true;
    }

    public void MenuUpdata()
    {
        Time.timeScale = 0;
        if (isSoundSetting)
        {
            return;
        }

        //調整キーの設定
        if (!upDownLock) StickerChangePointer();

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
        if (decision.WasPressedThisFrame())
        {
            if (isActionExpo) BackMenu();
            else SelectMenu();
        }
        if (back.WasPressedThisFrame())
        {
            if (isActionExpo) BackMenu();
            else basic.MenuBack();
        }
    }

    private void SelectMenu()
    {
        if (menuobj[0].activeSelf && !hideKeyChecking && isMenuText)//Menu
        {
            switch (pointer)
            {
                case 0:
                    basic.MenuBack();
                    //OnDeselected(menuobj[pointer]);
                    break;
                case 1:
                    ActionExpo();
                    //OnDeselected(menuobj[pointer]);
                    break;
                case 2:
                    SoundSetting();
                    break;
                case 3:
                    Exit();
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
        if (!volumeChecking && !inlineVolumeChecking)
        {
            pointer = 0;
            pointerpreb = -1;
        }
        inlineVolumeChecking = false;
        hideKeyChecking = false;
    }

    public MenuSystem Back()
    {
        if (isActionExpo)
        {
            BackMenu();
            return this;
        }
        else
        {
            actionExpoObj.SetActive(false);
            menuTextObj.SetActive(true);
            isMenuText = true;
            isActionExpo = false;
            isPauseMenu = false;
            this.GetComponent<Canvas>().enabled = false;
            Time.timeScale = 1;

            return null;
        }
    }

    //メニューに戻る
    public void BackMenu()
    {
        actionExpoObj.SetActive(false);
        menuTextObj.SetActive(true);
        isMenuText = true;
        isActionExpo = false;
    }

    //アクション詳細表示
    private void ActionExpo()
    {
        menuTextObj.SetActive(false);
        actionExpoObj.SetActive(true);
        isMenuText = false;
        isActionExpo = true;
    }

    private void SoundSetting()
    {
        menuTextObj.SetActive(false);
        soundSettingObj.SetActive(true);
        isMenuText = false;
        isSoundSetting = true;
        soundSetting.InputSet(playerInput, basic);
    }

    void Exit()
    {
        upDownLock = true;
        Time.timeScale = 1;
        if(sceneName != "Tutorial")
        {
            SeveSystem.Instance.GameDataSeve(SceneData.Instance.GetEachStageState, SceneData.Instance.GetSetStageFirstOpen, SceneData.Instance.stock);
        }
        SceneManager.LoadScene(backScene);
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
