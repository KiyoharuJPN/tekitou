using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
        optionKey = input.actions["Option"];

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

        if (back.WasPressedThisFrame() || optionKey.WasPressedThisFrame())
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
            SeveSystem.Instance.SettingSeve(SceneData.Instance.GetSetBGMVolume, SceneData.Instance.GetSetSEVolume);

            return this;
        }
        else
        {
            isPauseMenu = false;
            this.gameObject.SetActive(false);

            return backMenu;
        }
    }

    internal override void OnSelected(int objNum)
    {
        if (objNum > 1)
        {
            target.transform.position = new Vector2(700, menuObj[objNum].transform.position.y);
        }
        else
        {
            target.transform.position = new Vector2(400, menuObj[objNum].transform.position.y);
        }

        menuObj[objNum].color = color;    //UIの色変更
    }
}
