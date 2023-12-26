using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class TitleMenu : MonoBehaviour
{
    [Tooltip("�ڑ��������V�[���̖��O�����Ă�������"),Header("�ڑ��������V�[���̖��O")]
    public string gameScene;
    [Tooltip("���̑I���������|�C���^�[�ł�"),Header("�g���C�A���O���|�C���^�[")]
    public GameObject target;

    public GameObject[] menuobj;            //���j���[��ʂ̃I�u�W�F�N�g

    [SerializeField]
    GameObject backGround;

    [SerializeField]
    Animator player;

    [SerializeField]
    LoadFadeImage fade;

    bool canStart = true;

    //�|�C���^�[�ƈ�O�̃|�C���^�[
    int pointer;
    int pointerpreb;

    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;//�e��`�F�b�N�p�֐�

    //�f�����[�r�[�Đ����
    [SerializeField, Header("videoPlayer")]
    UnityEngine.Video.VideoPlayer videoPlayer;
    [SerializeField, Header("DmoVideoImage")]
    RawImage videoImage;
    [SerializeField, Header("PreesAnyKeyObj")]
    GameObject preeskey;
    [SerializeField, Header("�f�����[�r�[�ڍs�K�v����")]
    float demoVideoMoveTime = 40f;
    [SerializeField, Header("�f�����[�r�[�\������")]
    float demoVideoTime = 20f;
    bool isDemoVideo = false;
    bool canDemoVideo = true;
    float demoTimer;

    //InputSystem
    internal InputAction back, decision, move;

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        SceneData.Instance.referer = "Title";
        pointer = 0;            //�|�C���^�[�̏�����
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
        videoImage.enabled = false;
        videoPlayer.enabled = false;

        var playerInput = GetComponent<PlayerInput>();
        decision = playerInput.actions["Decision"];
        back = playerInput.actions["Back"];
        move = playerInput.actions["Move"];
    }

    private void Update()
    {

        if (!SoundManager.Instance.isPlayBGM())
        {
            StartCoroutine(PlayBGM());
        }

        //�f�����[�r�[��
        if (isDemoVideo)
        {
            DemoMove();
            return;
        }

        if (!InputKeyCheck.GetAnyKey())
        {
            demoTimer += Time.deltaTime;
            if (demoTimer >= demoVideoMoveTime)
                isDemoVideo = true;
        }
        else if (InputKeyCheck.GetAnyKey())
        {
            demoTimer = 0;
        }

        //�����L�[�̐ݒ�
        if (!upDownLock) StickerChangePointer();
        
        //�|�C���^�[���ς�������̐ݒ�
        if (pointer != pointerpreb)//�ύX���ꂽ�Ƃ��̍��
        {
            demoTimer = 0;
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

        if (!fade.IsFadeInComplete())
        {
            return;
        }

        //�I���L�[�̐ݒ�
        if (decision.WasPressedThisFrame())
        {
            if (menuobj[0].activeSelf && !hideKeyChecking)//Menu
            {
                switch (pointer)
                {
                    case 0:
                        GameStart();
                        //OnDeselected(menuobj[pointer]);
                        break;
                    case 1:
                        Exit();
                        //OnDeselected(menuobj[pointer]);
                        break;
                    default:
                        if (pointer < menuobj.Length - 1)
                        {
                            //OnDeselected(menuobj[pointer]);
                        }
                        break;
                }
                hideKeyChecking = true;
            }

            //�|�C���^�[�̕���
            if (!volumeChecking&&!inlineVolumeChecking)
            {
                pointer = 0;
                pointerpreb = -1;
            }
            inlineVolumeChecking = false;
            hideKeyChecking = false;
        }

        //�߂�L�[�̐ݒ�
        if (back.WasPressedThisFrame())
        {
            //menu
            if (menuobj[0].activeSelf)
            {
                pointer = 0;
            }

            //�|�C���^�[�̕���
            if (!inlineVolumeChecking)
            {
                pointer = 0;
                pointerpreb = -1;
            }
            inlineVolumeChecking = false;
        }
    }

    private void DemoMove()
    {
        if(canDemoVideo)
        {
            canDemoVideo = false;
            StartCoroutine(DemoVideoPlay());
        }
    }

    private void BackGroundMove()
    {
        //backGround.transform.position -= new Vector3(Time.deltaTime * 10f, 0);
    }

    //���j���[�̓���
    void GameStart()
    {
        if (!canStart) return;
        upDownLock = true;
        StartCoroutine(Scene_Start());
        canStart = false;
    }

    void Exit()
    {
        upDownLock = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
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

        if (gameScene != "") SceneManager.LoadScene(gameScene);
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

    IEnumerator PlayBGM()
    {
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
    }

    IEnumerator DemoVideoPlay()
    {
        demoTimer = 0;
        //�t�F�[�h�A�E�g�J�n
        fade.StartFadeOut();
        while (!fade.IsFadeOutComplete())
        {
            //�L�[�������ꂽ��I��
            if (InputKeyCheck.GetAnyKey())
            {
                DemoVideoEnd(); yield break;
            }
            yield return null;
        }
        videoImage.enabled = true;
        videoPlayer.enabled = true;
        videoPlayer.Play();
        preeskey.SetActive(true);

        fade.StartFadeIn();
        while (!fade.IsFadeInComplete()) 
        {
            if (InputKeyCheck.GetAnyKey()) { DemoVideoEnd(); yield break; }
            yield return null;
        }

        while(demoTimer <= demoVideoTime) 
        {
            demoTimer += Time.deltaTime;
            if (InputKeyCheck.GetAnyKey()) { DemoVideoEnd(); yield break; }
            yield return null;
        };
        fade.StartFadeOut();
        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }
        DemoVideoEnd();
        fade.StartFadeIn();
        while (!fade.IsFadeInComplete())
        {
            yield return null;
        }
    }

    private void DemoVideoEnd()
    {
        fade.FadeStop();
        preeskey.SetActive(false);
        videoImage.enabled = false;
        videoPlayer.Stop();
        videoPlayer.enabled = false;
        demoTimer = 0;
        isDemoVideo = false;
        canDemoVideo = true;
    }
}
