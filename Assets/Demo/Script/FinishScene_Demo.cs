using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishScene_Demo : MonoBehaviour
{
    [Tooltip("�ڕW�̃V�[���l�[���������Ă�������")]
    public string SceneName;

    [System.Serializable]
    struct FadeOutOption
    {
        [Tooltip("�t�F�[�h�A�E�g�̊J�n����")]
        public float waitSecondTry, wateSecondTitle;
        [Tooltip("�t�F�[�h�A�E�g�̎�������")]
        public float fadeOutSpeedTry, fadeOutSpeedTitle;
    }
    public float mouseMoveWait = 1f;

    //�^�[�Q�b�g
    public GameObject target;
    public FadeImage fade;
    public GameObject[] Finishobj;
    public Animator animator;


    bool pointerCheck = true, canChangePointer = true, canChoose = true;
    bool isRetry = false, isBack = false;
    float animWait, animSpeed;

    //float timeCount;
    //�|�C���^�[
    int pointer = 0, pointerpreb = -1;


    [SerializeField]
    [Header("�t�F�[�h�A�E�g�ݒ�")]
    FadeOutOption fadeOutOption = new() { waitSecondTry = 1f, wateSecondTitle = 1f, fadeOutSpeedTry = 10f, fadeOutSpeedTitle = 10f };

    private void Start()
    {
        Cursor.visible = false;
        Time.timeScale = 1f;
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.GameOver_intro, BGMSoundData.BGM.GameOver_roop);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 7"))
        {
            SceneManager.LoadScene("Title_Demo");
        }
        //�����L�[�̐ݒ�
        if (canChangePointer)
        {
            ChangePointer();
        }

        //�|�C���^�[���ς�������̐ݒ�
        PointerHasChange();

        //�I���L�[�̎w��
        if (canChoose) Choose();

        animator.SetBool("IsRetry", isRetry);
        animator.SetBool("IsBack", isBack);

        //Debug.Log(Input.GetAxis("Vertical")+"MOUSEIN"+pointerCheck);
    }


    //���s����
    void TryAgain()
    {
        SceneManager.LoadScene("Stage1_Demo");
        SceneData.Instance.stock = 2;
        SceneData.Instance.wayPoint_1 = false;
        SceneData.Instance.wayPoint_2 = false;
    }
    void BackTitle()
    {
        if (SceneName != "") SceneManager.LoadScene("Title_Demo");
    }
    /*IEnumerator PointerMoveWait()
    {
        yield return new WaitForSecondsRealtime(mouseMoveWait);
        pointerCheck = true;
        //Debug.Log("=========================================================================================");
    }*/
    IEnumerator Wait(int pointer, float waitSecond, float fadeOutSpeed)
    {
        OnSelected(Finishobj[pointer]);
        for (float i = waitSecond; i >= 0; i -= Time.deltaTime)
        {
            yield return null;
        }
        //�t�F�[�h�A�E�g�J�n
        fade.StartFadeOut();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        Debug.Log(pointer);

        switch (pointer)
        {
            case 0:
                TryAgain();
                break;
            case 1:
                Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");

                BackTitle();
                break;
            default:
                Debug.Log("�V�������ڂ̒ǉ��̓v���O���}�ɗ���ł��������B");
                break;
        }
    }


    //�����L�[�̐ݒ�
    void ChangePointer()
    {
        /*if (Input.GetAxis("Vertical") > 0 && Input.GetAxis("Vertical") < vertical) vertical = 0;
        if (Input.GetAxis("Vertical") < 0 && Input.GetAxis("Vertical") > vertical) vertical = 0;*/
        if (Input.GetAxis("Vertical") > 0 && pointerCheck)
        {
            //StartCoroutine(PointerMoveWait());
            pointer--;
            pointerCheck = false;
        }
        if (Input.GetAxis("Vertical") < 0 && pointerCheck)
        {
            //StartCoroutine(PointerMoveWait());
            pointer++;
            pointerCheck = false;
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            pointerCheck = true;
        }
    }
    //�|�C���^�[���ς�������̐ݒ�
    void PointerHasChange()
    {
        if (pointer != pointerpreb)
        {
            if (pointer < 0) pointer = 0;// Finishobj.Length - 1;
            if (pointer > Finishobj.Length - 1) pointer = Finishobj.Length - 1;//0;

            target.transform.position = new Vector2(target.transform.position.x, Finishobj[pointer].transform.position.y);

            //OnSelected(Finishobj[pointer]);
            //Debug.Log("pointer" + pointer + '\n' + "pointerpreb" + pointerpreb);
            //if (pointer != pointerpreb && pointerpreb != -1) OnDeselected(Finishobj[pointerpreb]);

            //�|�C���^�[��������
            pointerpreb = pointer;
        }
    }
    //�I���L�[�̎w��
    void Choose()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0"))
        {
            switch (pointer)
            {
                case 0:
                    SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_CutIn);
                    isRetry = true;
                    animWait = fadeOutOption.waitSecondTry;
                    animSpeed = fadeOutOption.fadeOutSpeedTry;
                    break;
                case 1:
                    isBack = true;
                    animWait = fadeOutOption.wateSecondTitle;
                    animSpeed = fadeOutOption.fadeOutSpeedTitle;
                    break;
                default:
                    Debug.Log("�V�������ڂ̒ǉ��̓v���O���}�ɗ���ł��������B");
                    break;
            }
            StartCoroutine(Wait(pointer, animWait, animSpeed));
            canChangePointer = false;
            canChoose = false;
        }
    }

    //��������
    void OnSelected(GameObject obj)
    {
        obj.GetComponentInChildren<Text>().color = Color.yellow;             //UI�̐F�C��
    }
    /*    void OnDeselected(GameObject obj)
        {
            obj.GetComponentInChildren<Text>().color = new Color(255, 255, 255); //�F��߂�
        }*/
}