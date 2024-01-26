using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour, MenuSystem
{
    [SerializeField] private TitleMenu titleMenu;

    [SerializeField] private SoundSetting soundSetting;

    //セーブ機能（未実装)
    [SerializeField] private GameObject saveSystem;

    //クレジット
    [SerializeField] private GameObject credit;

    [SerializeField] private GameObject option;

    [SerializeField, Header("トライアングルポインター")]
    private GameObject target;

    enum SelectMenu
    {
        SAVE = 0,
        SOUND = 1,
        CREDIT = 2,
    }
    private SelectMenu selectMenu = 0;

    public Text[] menuObj;            //メニュー画面のオブジェクト

    //InputSystem
    public InputAction back, decision, move;
    private bool isPointerMove = true;

    //メニュー表示確認Bool
    private bool isOpenMenu = false;
    private Color color = new Color(255, 69, 0); //色

    public void InputSet(PlayerInput playerInput)
    {
        var input = playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"];
        move = input.actions["Move"];

        option.SetActive(true);

        OnSelected((int)selectMenu);
    }

    public void MenuUpdata()
    {
        if(!isOpenMenu) StickerChangePointer();

        //選択キーの設定
        if (decision.WasPressedThisFrame())
        {
            if(isOpenMenu)
            {
                BackMenu();
            }
            else
            {
                SelectMenuProcess();
            }
        }

        if (back.WasPressedThisFrame())
        {
            if (isOpenMenu)
            {
                BackMenu();
            }
            else BackTitle();
        }
    }

    private void BackTitle()
    {
        option.SetActive(false);
        titleMenu.SetMenu(null);
    }

    private void BackMenu()
    {
        credit.SetActive(false);
        option.SetActive(true);
        isOpenMenu = false;
    }

    void StickerChangePointer()
    {
        var input = move.ReadValue<Vector2>().y;

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

    private void SelectMenuProcess()
    {

        switch (selectMenu)
        {
            case SelectMenu.SAVE:
                Debug.Log("この機能は未実装です");
                break;
            case SelectMenu.SOUND:
                soundSetting.gameObject.SetActive(true);
                titleMenu.SetMenu(soundSetting);
                break;
            case SelectMenu.CREDIT:
                CreditOpen();
                break;
        }
    }

    private void CreditOpen()
    {
        isOpenMenu = true;
        option.SetActive(false);
        credit.SetActive(true);
    }

    public MenuSystem Back()
    {
        throw new System.NotImplementedException();
    }

    public bool PauseCheck()
    {
        throw new System.NotImplementedException();
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
}
