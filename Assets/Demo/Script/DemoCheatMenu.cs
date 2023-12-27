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
    [Tooltip("���̑I���������|�C���^�[�ł�"), Header("�g���C�A���O���|�C���^�[")]
    public GameObject target;

    public GameObject[] menuText;            //���j���[��ʂ̃I�u�W�F�N�g
    public Text[] menu;

    public GameObject sW_Obj_Demo;
    public GameObject sW_Obj;

    public GameObject[] warpPoint;
    int warpNum = 0;

    //���j���[�\���m�FBool
    private bool isCheatMenu = true;

    //�e��DebugMenu�Ǘ��pBool
    private bool playerMode, sW_OnOff, sW_mode = false;

    //�|�C���^�[�ƈ�O�̃|�C���^�[
    int pointer;
    int pointerpreb;

    //�e��`�F�b�N�p�֐�
    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;

    //InputSystem
    internal InputAction selectKey_Up, selectKey_Down, selectKey_Right, selectKey_Left, menu_OnOff, warpKey;

    private void Start()
    {
        //pointer = 0;            //�|�C���^�[�̏�����

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
        //�����L�[�̐ݒ�
        if (!upDownLock) StickerChangePointer();

        //�|�C���^�[���ς�������̐ݒ�
        if (pointer != pointerpreb)//�ύX���ꂽ�Ƃ��̍��
        {
            if (menuText[0].activeSelf)//Menu
            {
                if (pointer < 0) pointer = 0;// menuobj.Length - 1;
                if (pointer > menuText.Length - 1) pointer = menuText.Length - 1;// 0;//�������

                target.transform.position = new Vector2(target.transform.position.x, menuText[pointer].transform.position.y);
            }

            //�|�C���^�[�̏C��
            pointerpreb = pointer;
        }

        //���ݑI�𒆂̋@�\�A���[�h�؂�ւ�
        if(isCheatMenu) MenuSetText();

        //UI�̕\���E��\��
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

        //���[�v�L�[�̐ݒ�
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
                    return "�ʏ�";
                }
                if (!playerMode)
                {
                    playerMode = true;
                    player.playerOpe = true;
                    return "�����{�^��";
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
                    return "�t�^ON";
                }
                break;
            case 2:
                if (sW_mode)
                {
                    sW_mode = false;
                    PlayerBuff.Instance.slashing.slashingObj = sW_Obj;
                    return "�c��";
                }
                if (!sW_mode)
                {
                    sW_mode = true;
                    PlayerBuff.Instance.slashing.slashingObj = sW_Obj_Demo;
                    return "�G�q�b�g����";
                }
                break;
        }
        return null;
    }
}
