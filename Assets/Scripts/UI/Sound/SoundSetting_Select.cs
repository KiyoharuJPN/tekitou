using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundSetting_Select : SoundSetting
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
}
