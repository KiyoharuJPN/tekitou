using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [Tooltip("���̑I���������|�C���^�[�ł�"), Header("�g���C�A���O���|�C���^�[")]
    public GameObject target;

    public PauseMenu pauseMenu;

    enum SelectMenu
    {
        BGM = 0,
        SE = 1
    }
    private SelectMenu selectMenu;

    public Text[] menuObj;            //���j���[��ʂ̃I�u�W�F�N�g
    public Slider[] selectSlider;            //���j���[��ʂ̃I�u�W�F�N�g

    //�e��`�F�b�N�p�֐�
    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;

    //InputSystem
    public PlayerInput playerInput;
    internal InputAction back, decision, move;

    public float sliderMoveSpeed = 0.5f;
    private Color color = new Color(255, 69, 0);

    private void Start()
    {

        var input = playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"];
        move = input.actions["Move"];
    }

    public void MenuUpdata()
    {
        //�����L�[�̐ݒ�
        StickerChangePointer();

        //�I���L�[�̐ݒ�
        if (decision.WasPressedThisFrame())
        {
            //SelectMenu();
        }

        //�߂�L�[�̐ݒ�
        if (back.WasPressedThisFrame())
        {
            BackMenu();
        }
    }

    //private void SelectMenu()
    //{
        
    //}

    public void BackGame()
    {
        //actionExpoObj.SetActive(false);
        //soundSettingObj.SetActive(false);
        //menuTextObj.SetActive(true);
        //isMenuText = true;
        //isActionExpo = false;
        //isSoundSetting = false;
        //isPauseMenu = false;
        //this.GetComponent<Canvas>().enabled = false;
        //Time.timeScale = 1;
    }

    //���j���[�ɖ߂�
    public void BackMenu()
    {
        //actionExpoObj.SetActive(false);
        //soundSettingObj.SetActive(false);
        //menuTextObj.SetActive(true);
        //isMenuText = true;
        //isActionExpo = false;
        //isSoundSetting = false;
    }

    void StickerChangePointer()
    {
        var input = move.ReadValue<Vector2>().y;
        if (input > 0)
        {
            pointerCheck = false;
        }
        if (input < 0)
        {
            pointerCheck = false;
        }
        if (input == 0)
        {
            pointerCheck = true;
        }
    }

    //��������
    void OnSelected(GameObject obj)
    {
        //obj.GetComponent<Image>().color = Color.grey;               //UI�̐F�C��
    }
    void OnDeselected(GameObject obj)
    {
        //obj.GetComponent<Image>().color = new Color(255, 255, 255); //�F��߂�
    }
}
