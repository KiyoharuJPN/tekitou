using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class TitleMenu : MonoBehaviour
{
    [Tooltip("�ڑ��������V�[���̖��O�����Ă�������"),Header("�ڑ��������V�[���̖��O")]
    public string gameScene;
    [Tooltip("���̑I���������|�C���^�[�ł�"),Header("�g���C�A���O���|�C���^�[")]
    public GameObject target;

    public GameObject[] menuobj;            //���j���[��ʂ̃I�u�W�F�N�g

    [Header("�w�i�摜�̃T�C�Y")]
    [SerializeField]
    Vector2 backgroundSpriteSizes = new Vector2(1500, 2000);
    [SerializeField]
    GameObject[] backGround;
    [SerializeField]
    Animator player;

    [SerializeField]
    LoadFadeImage fade;

    //�|�C���^�[�ƈ�O�̃|�C���^�[
    int pointer;
    int pointerpreb;

    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;//�e��`�F�b�N�p�֐�

    private void Start()
    {
        SceneData.Instance.referer = "Title";
        pointer = 0;            //�|�C���^�[�̏�����

        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
    }

    private void Update()
    {
        //BackGroundMove();
        //�����L�[�̐ݒ�
        //KeyboardChangePoint()
        if(!upDownLock) StickerChangePointer();

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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0"))
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
                            Debug.Log("�V�������ڂ�ǉ�����Ƃ��̓v���O���}�ɗ���ł��������B");
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
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown("joystick button 1"))
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

    //private void BackGroundMove()
    //{
    //    //1�摜���i�񂾎��A�X�N���[�����q����悤�ɗǂ������ɖ߂��Ă���B
    //    for (int i = 0; i < backgroundMax; i++)
    //    {
    //        backgroundScrollValues[i] -= (playerPosition.x - previousPlayerPosition.x) * scrollRates[i];

    //        if (backgroundSpriteSizes.x < backgroundsRt[i].anchoredPosition.x)
    //        {
    //            backgroundScrollValues[i] -= backgroundSpriteSizes.x;
    //            tempBackgroundsPosition.Set(backgroundSpriteSizes.x, 0);
    //            backgroundsRt[i].anchoredPosition -= tempBackgroundsPosition;
    //        }
    //        else if (backgroundsRt[i].anchoredPosition.x < -backgroundSpriteSizes.x)
    //        {
    //            backgroundScrollValues[i] += backgroundSpriteSizes.x;
    //            tempBackgroundsPosition.Set(backgroundSpriteSizes.x, 0);
    //            backgroundsRt[i].anchoredPosition += tempBackgroundsPosition;
    //        }
    //    }
    //}

    //���j���[�̓���
    void GameStart()
    {
        upDownLock = true;
        StartCoroutine(Scene_Start());
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
        if (Input.GetAxis("Vertical") > 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer--;
        }
        if (Input.GetAxis("Vertical") < 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer++;
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            pointerCheck = true;
        }
    }

    IEnumerator Scene_Start()
    {
        player.SetTrigger("Start");
        SoundManager.Instance.PlaySE(SESoundData.SE.GoalSE);
        yield return new WaitForSeconds(2.4f);

        fade.StartFadeOut();
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
}
