using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartConfirmUI : MonoBehaviour, MenuSystem
{
    [SerializeField] public TitleMenu titleMenu;

    [SerializeField, Header("トライアングルポインター")]
    private GameObject target;

    internal enum SelectMenu
    {
        YES = 0,
        NO = 1,
    }
    internal SelectMenu selectMenu = 0;

    public Text[] menuObj;            //メニュー画面のオブジェクト

    //InputSystem
    public InputAction back, decision, move;
    private bool isPointerMove = true;
    private bool canInput = true;
    //メニュー表示確認Bool
    internal bool isPauseMenu = false;
    private Color color = new Color(255, 69, 0);

    public virtual void InputSet(PlayerInput playerInput, MenuBasic menuBasic = null)
    {
        isPauseMenu = true;
        var input = playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"];
        move = input.actions["Move"];

        selectMenu = 0;
        OnSelected((int)selectMenu);
    }

    public bool PauseCheck()
    {
        return isPauseMenu;
    }

    public virtual void MenuUpdata()
    {
        if(canInput) StickerChangePointer();

        //選択キーの設定
        if (decision.WasPressedThisFrame() && canInput)
        {
            SelectMenuProcess();
        }
    }

    internal void StickerChangePointer()
    {
        if ((int)selectMenu == 2) return;

        var input = move.ReadValue<Vector2>().x;

        if (input < -0.3f && (int)selectMenu > 0 && isPointerMove)
        {
            isPointerMove = false;
            ChangePointer(-1);
        }
        if (input > 0.3f && (int)selectMenu < 1 && isPointerMove)
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

    internal virtual void SelectMenuProcess()
    {
        canInput = false;
        switch (selectMenu)
        {
            case SelectMenu.YES:
                SeveDataDelete();
                break;
            case SelectMenu.NO:
                isPauseMenu = false;
                this.gameObject.SetActive(false);
                titleMenu.TitelMenuOpen();
                OnDeselected((int)selectMenu);
                selectMenu = 0;
                OnSelected((int)selectMenu);
                canInput = true;
                break;
        }
    }

    private void SeveDataDelete()
    {
        SeveSystem.Instance.seveDataDelete();
        SceneData.Instance.StageDataReset();
        SceneData.Instance.stock = SceneData.MAX_STOCK;

        titleMenu.GameStart();
    }

    public virtual MenuSystem Back()
    {
        isPauseMenu = false;
        this.gameObject.SetActive(false);

        return null;
    }

    internal void OnSelected(int objNum)
    {
        target.transform.position = new Vector2(menuObj[objNum].transform.position.x - 180, target.transform.position.y);
        menuObj[objNum].color = color;    //UIの色変更
    }
    internal void OnDeselected(int objNum)
    {
        menuObj[objNum].color = Color.white; //色を戻す
    }
}
