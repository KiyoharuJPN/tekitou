using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DemoCheatMenu : MonoBehaviour
{
    [SerializeField]
    Player_Demo player;
    [Tooltip("今の選択を示すポインターです"), Header("トライアングルポインター")]
    public GameObject target;

    public GameObject[] menuText;            //メニュー画面のオブジェクト
    public Text[] menu;

    public GameObject sW_Obj_Demo;
    public GameObject sW_Obj;

    public GameObject[] warpPoint;
    int warpNum = 0;

    //メニュー表示確認Bool
    private bool isCheatMenu = true;

    //各種DebugMenu管理用Bool
    private bool playerMode, sW_OnOff, sW_mode = false;

    //ポインターと一個前のポインター
    int pointer;
    int pointerpreb;

    //各種チェック用関数
    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;

    //InputSystem
    internal InputAction selectKey_Up, selectKey_Down, selectKey_Right, selectKey_Left, menu_OnOff, warpKey;

    private void Start()
    {
        //pointer = 0;            //ポインターの初期化

        var playerInput = GetComponent<PlayerInput>();
        selectKey_Up = playerInput.actions["SelectUp"];
        selectKey_Down = playerInput.actions["SelectDown"];
        selectKey_Right = playerInput.actions["SelectRight"];
        selectKey_Left = playerInput.actions["SelectLeft"];

        menu_OnOff = playerInput.actions["MenuOnOff"];

        warpKey = playerInput.actions["Warp"];

    }

    public void Update()
    {
        //調整キーの設定
        if (!upDownLock) StickerChangePointer();

        //ポインターが変わった時の設定
        if (pointer != pointerpreb)//変更されたときの作業
        {
            if (menuText[0].activeSelf)//Menu
            {
                if (pointer < 0) pointer = 0;// menuobj.Length - 1;
                if (pointer > menuText.Length - 1) pointer = menuText.Length - 1;// 0;//上限調整

                target.transform.position = new Vector2(target.transform.position.x, menuText[pointer].transform.position.y);
            }

            //ポインターの修正
            pointerpreb = pointer;
        }

        //現在選択中の機能、モード切り替え
        if(isCheatMenu) MenuSetText();

        //UIの表示・非表示
        if (menu_OnOff.WasPressedThisFrame())
        {
            var canvas = GetComponent<Canvas>();
            if(canvas.enabled)
            {
                isCheatMenu = false;
                canvas.enabled = false;
            }
            else
            {
                isCheatMenu = true;
                canvas.enabled = true;
            }
        }

        //ワープキーの設定
        if (warpKey.WasPressedThisFrame())
        {
            player.gameObject.transform.position = warpPoint[warpNum].transform.position;
            warpNum++;

            if(warpNum == 3)
            {
                warpNum = 0;
            }
        }

    }

    void StickerChangePointer()
    {
        if (!isCheatMenu) return;
        if (selectKey_Up.WasPressedThisFrame())
        {
            pointerCheck = false;
            pointer--;
        }
        if (selectKey_Down.WasPressedThisFrame())
        {
            pointerCheck = false;
            pointer++;
        }
    }

    void MenuSetText()
    {
        if(selectKey_Left.WasPressedThisFrame() || selectKey_Right.WasPressedThisFrame())
        {
            menu[pointer].text = MenuSet();
        }
    }

    string MenuSet()
    {
        switch (pointer)
        {
            case 0:
                if (playerMode)
                {
                    playerMode = false;
                    player.playerOpe = false;
                    return "通常";
                }
                if (!playerMode)
                {
                    playerMode = true;
                    player.playerOpe = true;
                    return "ワンボタン";
                }
                break;
            case 1:
                if (sW_OnOff)
                {
                    sW_OnOff = false;
                    PlayerBuff.Instance.SlashingBuffRemove();
                    return "OFF";
                }
                if (!sW_OnOff)
                {
                    sW_OnOff = true;
                    PlayerBuff.Instance.BuffSet(1);
                    return "付与ON";
                }
                break;
            case 2:
                if (sW_mode)
                {
                    sW_mode = false;
                    PlayerBuff.Instance.slashing.slashingObj = sW_Obj;
                    return "残留";
                }
                if (!sW_mode)
                {
                    sW_mode = true;
                    PlayerBuff.Instance.slashing.slashingObj = sW_Obj_Demo;
                    return "敵ヒット消滅";
                }
                break;
        }
        return null;
    }
}
