using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface MenuSystem
{
    public void InputSet(PlayerInput input);

    public bool PauseCheck();

    public void MenuUpdata();

    public MenuSystem Back();
}
