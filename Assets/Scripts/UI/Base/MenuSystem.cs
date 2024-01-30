using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface MenuSystem
{
    public void InputSet(PlayerInput input, MenuBasic menuBasic = null);

    public bool PauseCheck();

    public void MenuUpdata();

    public MenuSystem Back();
}

public interface MenuBasic
{
    public void SetMenu(MenuSystem menu);
    public void MenuBack();
}
