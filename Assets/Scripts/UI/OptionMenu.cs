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

    //�Z�[�u�@�\�i������)
    [SerializeField] private GameObject saveSystem;

    //�N���W�b�g
    [SerializeField] private GameObject credit;

    [SerializeField] private GameObject option;

    [SerializeField, Header("�g���C�A���O���|�C���^�[")]
    private GameObject target;

    enum SelectMenu
    {
        SAVE = 0,
        SOUND = 1,
        CREDIT = 2,
    }
    private SelectMenu selectMenu = 0;

    public Text[] menuObj;            //���j���[��ʂ̃I�u�W�F�N�g

    //InputSystem
    public InputAction back, decision, move;
    private bool isPointerMove = true;

    //���j���[�\���m�FBool
    private bool isOpenMenu = false;
    private Color color = new Color(255, 69, 0); //�F

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

        //�I���L�[�̐ݒ�
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
                Debug.Log("���̋@�\�͖������ł�");
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

        menuObj[objNum].color = color;    //UI�̐F�ύX
    }
    void OnDeselected(int objNum)
    {
        menuObj[objNum].color = Color.white; //�F��߂�
    }
}
