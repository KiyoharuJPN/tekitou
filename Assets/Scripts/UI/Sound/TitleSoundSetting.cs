using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleSoundSetting : SoundSetting
{
    [SerializeField] public OptionMenu backMenu;
    [SerializeField] public TitleMenu titleMenu;

    public override void InputSet(PlayerInput playerInput, MenuBasic menuBasic = null)
    {
        isPauseMenu = true;
        var input = playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"];
        move = input.actions["Move"];

        OnSelected((int)selectMenu);
    }

    public override void MenuUpdata()
    {
        if (selectSlider == null)
        {
            StickerChangePointer();
        }
        else if (selectSlider != null)
        {
            ChangeSlider();
        }

        //選択キーの設定
        if (decision.WasPressedThisFrame())
        {
            SelectMenuProcess();
        }

        if (back.WasPressedThisFrame())
        {
            titleMenu.MenuBack();
        }
    }

    internal override void SelectMenuProcess()
    {

        switch (selectMenu)
        {
            case SelectMenu.BGM:
                selectSlider = BGMSlider;
                selectSlider.Active(true);
                break;
            case SelectMenu.SE:
                selectSlider = SESlider;
                selectSlider.Active(true);
                break;
            case SelectMenu.RESET: //音量を初期値に変更
                SoundValueReset();
                break;
            case SelectMenu.BACK:
                titleMenu.MenuBack();
                break;
        }
    }

    public override MenuSystem Back()
    {
        if (selectSlider != null)
        {
            selectSlider.Active(false);
            selectSlider = null;

            return this;
        }
        else
        {
            isPauseMenu = false;
            this.gameObject.SetActive(false);

            return backMenu;
        }
    }
}
