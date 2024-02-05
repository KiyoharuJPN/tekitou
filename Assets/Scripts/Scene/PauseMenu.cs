using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour, MenuSystem
{
    [SerializeField] string sceneName = "none";
    [Tooltip("���̑I���������|�C���^�[�ł�"), Header("�g���C�A���O���|�C���^�[")]
    public GameObject target;

    public string backScene;

    public GameObject[] menuobj;            //���j���[��ʂ̃I�u�W�F�N�g
    public SoundSetting soundSetting;            //���j���[��ʂ̃I�u�W�F�N�g
    //�c��c�@�摜
    public Image stockImage;
    public Sprite[] stockImages;

    private MenuBasic basic;

    //���j���[�\���m�FBool
    private bool isPauseMenu = false;

    public GameObject menuTextObj;
    private bool isMenuText = true;
    public GameObject actionExpoObj;
    private bool isActionExpo = false;
    public GameObject soundSettingObj;
    private bool isSoundSetting = false;
    internal bool IsSoundSetting {  get { return isSoundSetting; } }

    [SerializeField]
    Animator player;

    [SerializeField]
    LoadFadeImage fade;

    bool canStart = true;

    //�|�C���^�[�ƈ�O�̃|�C���^�[
    int pointer;
    int pointerpreb;

    //�e��`�F�b�N�p�֐�
    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;

    //InputSystem
    public PlayerInput playerInput;
    internal InputAction back, decision, move;

    public float sliderMoveSpeed = 0.5f;

    private void Start()
    {
        pointer = 0;            //�|�C���^�[�̏�����
    }

    public void InputSet(PlayerInput playerInput, MenuBasic menuBasic)
    {
        if(playerInput != null)
        {
            this.playerInput = playerInput;
        }
        basic = menuBasic;

        var input = this.playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"];
        move = input.actions["Move"];

        Time.timeScale = 0;
        isPauseMenu = true;
        isSoundSetting = false;
        stockImage.sprite = stockImages[SceneData.Instance.stock];
        this.GetComponent<Canvas>().enabled = true;
        menuTextObj.SetActive(true);
        isMenuText = true;
        basic.SetMenu(this);
    }

    public bool PauseCheck()
    {
        return isPauseMenu;
    }

    public void PauseStart()
    {
        Time.timeScale = 0;
        isPauseMenu = true;
        stockImage.sprite = stockImages[SceneData.Instance.stock];
        this.GetComponent<Canvas>().enabled = true;
    }

    public void MenuUpdata()
    {
        Time.timeScale = 0;
        if (isSoundSetting)
        {
            return;
        }

        //�����L�[�̐ݒ�
        if (!upDownLock) StickerChangePointer();

        //�|�C���^�[���ς�������̐ݒ�
        if (pointer != pointerpreb)//�ύX���ꂽ�Ƃ��̍��
        {
            if (menuobj[0].activeSelf)//Menu
            {
                if (pointer < 0) pointer = 0;// menuobj.Length - 1;
                if (pointer > menuobj.Length - 1) pointer = menuobj.Length - 1;// 0;//�������

                target.transform.position = new Vector2(target.transform.position.x, menuobj[pointer].transform.position.y);
                //OnSelected(menuobj[pointer]);
                //if(pointer !=pointerpreb && pointerpreb != -1) OnDeselected(menuobj[pointerpreb]);
            }

            //�|�C���^�[�̏C��
            pointerpreb = pointer;
        }

        //�I���L�[�̐ݒ�
        if (decision.WasPressedThisFrame())
        {
            if (isActionExpo) BackMenu();
            else SelectMenu();
        }
        if (back.WasPressedThisFrame())
        {
            if (isActionExpo) BackMenu();
            else basic.MenuBack();
        }
    }

    private void SelectMenu()
    {
        if (menuobj[0].activeSelf && !hideKeyChecking && isMenuText)//Menu
        {
            switch (pointer)
            {
                case 0:
                    basic.MenuBack();
                    //OnDeselected(menuobj[pointer]);
                    break;
                case 1:
                    ActionExpo();
                    //OnDeselected(menuobj[pointer]);
                    break;
                case 2:
                    SoundSetting();
                    break;
                case 3:
                    Exit();
                    break;
                default:
                    if (pointer < menuobj.Length - 1)
                    {
                        //OnDeselected(menuobj[pointer]);
                        Debug.Log("�V�������ڂ�ǉ�����Ƃ��̓v���O���}�ɗ���ł��������B");
                    }
                    break;
            }
            hideKeyChecking = true;
        }

        //�|�C���^�[�̕���
        if (!volumeChecking && !inlineVolumeChecking)
        {
            pointer = 0;
            pointerpreb = -1;
        }
        inlineVolumeChecking = false;
        hideKeyChecking = false;
    }

    public MenuSystem Back()
    {
        if (isActionExpo)
        {
            BackMenu();
            return this;
        }
        else
        {
            actionExpoObj.SetActive(false);
            menuTextObj.SetActive(true);
            isMenuText = true;
            isActionExpo = false;
            isPauseMenu = false;
            this.GetComponent<Canvas>().enabled = false;
            Time.timeScale = 1;

            return null;
        }
    }

    //���j���[�ɖ߂�
    public void BackMenu()
    {
        actionExpoObj.SetActive(false);
        menuTextObj.SetActive(true);
        isMenuText = true;
        isActionExpo = false;
    }

    //�A�N�V�����ڍו\��
    private void ActionExpo()
    {
        menuTextObj.SetActive(false);
        actionExpoObj.SetActive(true);
        isMenuText = false;
        isActionExpo = true;
    }

    private void SoundSetting()
    {
        menuTextObj.SetActive(false);
        soundSettingObj.SetActive(true);
        isMenuText = false;
        isSoundSetting = true;
        soundSetting.InputSet(playerInput, basic);
    }

    void Exit()
    {
        upDownLock = true;
        Time.timeScale = 1;
        if(sceneName != "Tutorial")
        {
            SeveSystem.Instance.GameDataSeve(SceneData.Instance.GetEachStageState, SceneData.Instance.GetSetStageFirstOpen, SceneData.Instance.stock);
        }
        SceneManager.LoadScene(backScene);
    }

    void StickerChangePointer()
    {
        var input = move.ReadValue<Vector2>().y;
        if (input > 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer--;
        }
        if (input < 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer++;
        }
        if (input == 0)
        {
            pointerCheck = true;
        }
    }

    //��������
    void OnSelected(GameObject obj)
    {
        obj.GetComponent<Image>().color = Color.grey;               //UI�̐F�C��
    }
    void OnDeselected(GameObject obj)
    {
        obj.GetComponent<Image>().color = new Color(255, 255, 255); //�F��߂�
    }

}
