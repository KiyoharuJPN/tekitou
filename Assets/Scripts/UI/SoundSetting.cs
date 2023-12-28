using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [Tooltip("今の選択を示すポインターです"), Header("トライアングルポインター")]
    public GameObject target;

    public PauseMenu pauseMenu;

    enum SelectMenu
    {
        BGM = 0,
        SE = 1
    }
    private SelectMenu selectMenu;

    public Text[] menuObj;            //メニュー画面のオブジェクト
    public Slider[] selectSlider;            //メニュー画面のオブジェクト

    //各種チェック用関数
    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;

    //InputSystem
    public PlayerInput playerInput;
    internal InputAction back, decision, move;

    public float sliderMoveSpeed = 0.5f;
    private Color color = new Color(255, 69, 0);

    private void Start()
    {

        var input = playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"];
        move = input.actions["Move"];
    }

    public void MenuUpdata()
    {
        //調整キーの設定
        StickerChangePointer();

        //選択キーの設定
        if (decision.WasPressedThisFrame())
        {
            //SelectMenu();
        }

        //戻るキーの設定
        if (back.WasPressedThisFrame())
        {
            BackMenu();
        }
    }

    //private void SelectMenu()
    //{
        
    //}

    public void BackGame()
    {
        //actionExpoObj.SetActive(false);
        //soundSettingObj.SetActive(false);
        //menuTextObj.SetActive(true);
        //isMenuText = true;
        //isActionExpo = false;
        //isSoundSetting = false;
        //isPauseMenu = false;
        //this.GetComponent<Canvas>().enabled = false;
        //Time.timeScale = 1;
    }

    //メニューに戻る
    public void BackMenu()
    {
        //actionExpoObj.SetActive(false);
        //soundSettingObj.SetActive(false);
        //menuTextObj.SetActive(true);
        //isMenuText = true;
        //isActionExpo = false;
        //isSoundSetting = false;
    }

    void StickerChangePointer()
    {
        var input = move.ReadValue<Vector2>().y;
        if (input > 0)
        {
            pointerCheck = false;
        }
        if (input < 0)
        {
            pointerCheck = false;
        }
        if (input == 0)
        {
            pointerCheck = true;
        }
    }

    //内部動き
    void OnSelected(GameObject obj)
    {
        //obj.GetComponent<Image>().color = Color.grey;               //UIの色修正
    }
    void OnDeselected(GameObject obj)
    {
        //obj.GetComponent<Image>().color = new Color(255, 255, 255); //色を戻す
    }
}
