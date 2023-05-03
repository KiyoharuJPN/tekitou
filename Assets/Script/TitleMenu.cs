using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    [Tooltip("�ڑ��������V�[���̖��O�����Ă�������"),Header("�ڑ��������V�[���̖��O")]
    public string gameScene;
    [Tooltip("�^�C�g���摜�����Ă�������"),Header("�^�C�g���摜")]
    public GameObject TitleImage;
    [Tooltip("���̑I���������|�C���^�[�ł�"),Header("�g���C�A���O���|�C���^�[")]
    public GameObject target;

    public GameObject[] menuobj;            //���j���[��ʂ̃I�u�W�F�N�g

    public GameObject[] optionShow;         //�I�v�V������ʂ̃I�u�W�F�N�g

    public GameObject[] SEoptionShow;       //����������ʂ̃I�u�W�F�N�g
    [Tooltip("SE�I�v�V�����R���g���[�������Ă�������"), Header("SE�I�v�V�������")]
    public GameObject SEdisplay;            //SE�̔w�i���
    public Slider masterVolume, BGMVolume, SEVolume;//���������̘A��

    //�|�C���^�[�ƈ�O�̃|�C���^�[
    int pointer;
    int pointerpreb;

    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;//�e��`�F�b�N�p�֐�

    //Sound�l�̏�����
    public float master = 0.4f, BGM = 0.4f, SE = 0.4f;

    private void Start()
    {
        pointer = 0;            //�|�C���^�[�̏�����
        //OnSelected(menuobj[0]); //�Z���N�g�̏�����

        //�����C��
        SoundManager.Instance.masterVolume = master;
        SoundManager.Instance.bgmMasterVolume = BGM;
        SoundManager.Instance.seMasterVolume = SE;
        masterVolume.value = master;
        BGMVolume.value = BGM;
        SEVolume.value = SE;
    }

    private void Update()
    {
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

            if (optionShow[0].activeSelf)//Option
            {
                if (pointer < 0) pointer = 0;// optionShow.Length - 1;
                if (pointer > optionShow.Length - 1) pointer = optionShow.Length - 1;//0;//�������

                target.transform.position = new Vector2(target.transform.position.x, optionShow[pointer].transform.position.y);
                //OnSelected(optionShow[pointer]);
                //Debug.Log("p" + pointer + '\n' + "pp" + pointerpreb);
                //if (pointer != pointerpreb && pointerpreb != -1) OnDeselected(optionShow[pointerpreb]);
            }

            if (SEoptionShow[0].activeSelf)//Sound
            {
                if (pointer < 0) pointer = SEoptionShow.Length - 1;
                if (pointer > SEoptionShow.Length - 1) pointer = 0;//�������

                OnSelected(SEoptionShow[pointer]);
                if (pointer != pointerpreb && pointerpreb != -1)
                {
                    if (pointerpreb > 0)
                    {
                        OnDeselectedSE(SEoptionShow[pointerpreb]);
                    }
                    else
                    {
                        OnDeselected(SEoptionShow[pointerpreb]);
                    }
                }
            }


            //�|�C���^�[�̏C��
            pointerpreb = pointer;
        }


        //Debug.Log(pointerCheck);
        

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
                        Option();
                        //OnDeselected(menuobj[pointer]);
                        break;
                    case 2:
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

            if (optionShow[0].activeSelf && !hideKeyChecking)//Option
            {
                switch (pointer)
                {
                    case 0:
                        //�ۑ����Ă���
                        masterVolume.value = master = SoundManager.Instance.masterVolume;
                        BGMVolume.value = BGM = SoundManager.Instance.bgmMasterVolume;
                        SEVolume.value = SE = SoundManager.Instance.seMasterVolume;
                        //��ʂ��J��
                        SEOption();
                        //OnDeselected(optionShow[pointer]);    //���̑����SEOption�ɒǉ����܂���
                        break;
                    default:
                        if(pointer < optionShow.Length - 1)
                        {
                            //OnDeselected(optionShow[pointer]);
                            Debug.Log("�V�������ڂ�ǉ�����Ƃ��̓v���O���}�ɗ���ł��������B");
                        }
                        break;
                }
                hideKeyChecking = true;
            }

            if (SEoptionShow[0].activeSelf && !hideKeyChecking)
            {
                bool SEcheck = false;
                if (upDownLock&&!SEcheck)
                {
                    volumeChecking = false;
                    upDownLock = false;
                    OnSelected(SEoptionShow[pointer]);
                    SEcheck = true;
                    inlineVolumeChecking = true;
                }
                if (!upDownLock&&!SEcheck)
                {
                    //SEcheck = true;
                    switch (pointer)
                    {
                        case 0:
                            volumeChecking = false;
                            //OnDeselected(SEoptionShow[pointer]);  //���̑����DeSEOption�ɒǉ����܂���
                            DeSEOption();
                            break;
                        case 1:
                            volumeChecking = true;
                            upDownLock = true;
                            OnselectedSE(SEoptionShow[pointer]);
                            break;
                        case 2:
                            volumeChecking = true;
                            upDownLock = true;
                            OnselectedSE(SEoptionShow[pointer]);
                            break;
                        case 3:
                            volumeChecking = true;
                            upDownLock = true;
                            OnselectedSE(SEoptionShow[pointer]);
                            break;
                        default:
                            if (pointer < SEoptionShow.Length - 1) OnDeselectedSE(SEoptionShow[pointer]);
                            Debug.Log("�V�������ڂ�ǉ�����Ƃ��̓v���O���}�ɗ���ł��������B");
                            break;
                    }
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
        if (volumeChecking) SetVolume();





        //�߂�L�[�̐ݒ�
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown("joystick button 1"))
        {
            //menu
            if (menuobj[0].activeSelf)
            {
                pointer = 0;
            }

            //option
            if (optionShow[0].activeSelf)
            {
                //OnDeselected(optionShow[pointer]);    //���̑����DeSEOption�ɒǉ����܂���
                DeOption();
            }

            //sound
            if (SEoptionShow[0].activeSelf)
            {
                if (!upDownLock)
                {
                    if (pointer > 0)
                    {
                        OnDeselectedSE(SEoptionShow[pointer]);
                    }
                    else
                    {
                        OnDeselected(SEoptionShow[pointer]);
                    }
                    //�{�����[����߂�
                    SoundManager.Instance.masterVolume = master;
                    SoundManager.Instance.bgmMasterVolume = BGM;
                    SoundManager.Instance.seMasterVolume = SE;

                    DeSEOption();
                }


                if (upDownLock)
                {
                    volumeChecking = false;
                    upDownLock = false;
                    OnSelected(SEoptionShow[pointer]);
                    inlineVolumeChecking = true;
                }
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

    //���j���[�̓����@�@����𖳎�����?//.getcompeont<Code>().getmove();
    void GameStart()
    {
        if(gameScene!="")SceneManager.LoadScene(gameScene);
        Debug.Log("�Q�[����ʂ����[�h���܂����B");
    }

    void Option()
    {
        for(int i = 0; i < menuobj.Length; i++)
        {
            menuobj[i].SetActive(false);
        }
        for(int i = 0; i < optionShow.Length; i++)
        {
            optionShow[i].SetActive(true);
        }
    }
    void DeOption()
    {
        for (int i = 0; i < optionShow.Length; i++)
        {
            optionShow[i].SetActive(false);
        }
        for (int i = 0; i < menuobj.Length; i++)
        {
            menuobj[i].SetActive(true);
        }
    }
    void Exit()
    {
        Application.Quit();
        Debug.Log("�Q�[�����甲���܂����B");
    }

    void SEOption()
    {
        TitleImage.SetActive(false);
        for(int i = 0; i < optionShow.Length; i++)
        {
            optionShow[i].SetActive(false);
        }

        target.SetActive(false);
        SEdisplay.SetActive(true);
        for ( int i = 0; i < SEoptionShow.Length; i++)
        {
            SEoptionShow[i].SetActive(true);
        }
        OnSelected(SEoptionShow[0]);
    }
    void DeSEOption()
    {
        for (int i = 0; i < SEoptionShow.Length; i++)
        {
            SEoptionShow[i].SetActive(false);
        }
        SEdisplay.SetActive(false);
        OnDeselected(SEoptionShow[0]);
        
        target.SetActive(true);
        TitleImage.SetActive(true);
        for (int i = 0; i < optionShow.Length; i++)
        {
            optionShow[i].SetActive(true);
        }
    }





    //��������
    void SetVolume()
    {
        
        switch (pointer)
        {
            case 0:
                break;
            case 1:
                if (Input.GetKey(KeyCode.A))
                {
                    SoundManager.Instance.masterVolume = SoundManager.Instance.masterVolume + -0.001f;
                }else if (Input.GetKey(KeyCode.D))
                {
                    SoundManager.Instance.masterVolume = SoundManager.Instance.masterVolume + 0.001f;
                }
                SoundManager.Instance.masterVolume = SoundManager.Instance.masterVolume + (Input.GetAxis("Horizontal") * 0.001f);
                masterVolume.value = SoundManager.Instance.masterVolume;
                break;
            case 2:
                if (Input.GetKey(KeyCode.A))
                {
                    SoundManager.Instance.bgmMasterVolume = SoundManager.Instance.bgmMasterVolume + -0.001f;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    SoundManager.Instance.bgmMasterVolume = SoundManager.Instance.bgmMasterVolume + 0.001f;
                }
                SoundManager.Instance.bgmMasterVolume = SoundManager.Instance.bgmMasterVolume + (Input.GetAxis("Horizontal") * 0.001f);
                BGMVolume.value = SoundManager.Instance.bgmMasterVolume;
                break;
            case 3:
                if (Input.GetKey(KeyCode.A))
                {
                    SoundManager.Instance.seMasterVolume = SoundManager.Instance.seMasterVolume + -0.001f;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    SoundManager.Instance.seMasterVolume = SoundManager.Instance.seMasterVolume + 0.001f;
                }
                SoundManager.Instance.seMasterVolume = SoundManager.Instance.seMasterVolume + (Input.GetAxis("Horizontal") * 0.001f);
                SEVolume.value = SoundManager.Instance.seMasterVolume;
                break;
            default:
                break;
        }
    }



    //�����L�[�̐ݒ�
    //void KeyboardChangePoint()
    //{
    //    if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        pointer--;
    //    }
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        pointer++;
    //    }
    //}
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


    //��������
    void OnSelected(GameObject obj)
    {
        obj.GetComponent<Image>().color = Color.grey;               //UI�̐F�C��
    }
    void OnDeselected(GameObject obj)
    {
        obj.GetComponent<Image>().color = new Color(255, 255, 255); //�F��߂�
    }
    void OnDeselectedSE(GameObject obj)
    {
        obj.GetComponent<Image>().color = new Color(255, 255, 255, 0); //�F��߂�
    }
    void OnselectedSE(GameObject obj)
    {
        obj.GetComponent <Image>().color = Color.green;
    }
}
