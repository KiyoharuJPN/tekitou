using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Tooltip("���̑I���������|�C���^�[�ł�"), Header("�g���C�A���O���|�C���^�[")]
    public GameObject target;

    public string backScene;

    public GameObject[] menuobj;            //���j���[��ʂ̃I�u�W�F�N�g
    //�c��c�@�摜
    public Image stockImage;
    public Sprite[] stockImages;
    

    //���j���[�\���m�FBool
    private bool isPauseMenu = false;

    public GameObject menuTextObj;
    private bool isMenuText = true;
    public GameObject actionExpoObj;
    private bool isActionExpo = false;

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

    private void Start()
    {
        pointer = 0;            //�|�C���^�[�̏�����

        var input = playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"]; 
        move = input.actions["Move"];
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
            SelectMenu();
        }

        //�߂�L�[�̐ݒ�
        if (back.WasPressedThisFrame())
        {
            BackMenu();
        }
    }

    private void SelectMenu()
    {
        if (menuobj[0].activeSelf && !hideKeyChecking && isMenuText)//Menu
        {
            switch (pointer)
            {
                case 0:
                    BackGame();
                    //OnDeselected(menuobj[pointer]);
                    break;
                case 1:
                    ActionExpo();
                    //OnDeselected(menuobj[pointer]);
                    break;
                case 2:
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

    public void BackGame()
    {
        actionExpoObj.SetActive(false);
        menuTextObj.SetActive(true);
        isMenuText = true;
        isActionExpo = false;
        isPauseMenu = false;
        this.GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;
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

    void Exit()
    {
        upDownLock = true;
        Time.timeScale = 1;
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

    IEnumerator Scene_Start()
    {
        player.SetTrigger("Start");
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_CutIn);
        yield return new WaitForSeconds(2.4f);

        fade.StartFadeOut();
        SceneData.Instance.StageDataReset();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
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
