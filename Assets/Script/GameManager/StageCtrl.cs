using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class StageCtrl : MonoBehaviour
{
    [Header("�v���C���[�Q�[���I�u�W�F�N�g")] public GameObject playerObj;
    [Header("�R���e�B�j���[�ʒu")] public GameObject[] continuePoint;

    //�N���b�V���΍�̒���������
    private float startButtonTime = 5f;
    private float getKayTime = 0;

    //�v���C���Ԍv��
    public bool playTimeMeasurement = false;
    private float playTime = 0;

    //InputSystem
    internal InputAction option;

    virtual protected void Start()
    {
        Time.timeScale = 1f;
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && !SceneData.Instance.wayPoint_1 && !SceneData.Instance.wayPoint_2)
        {
            playerObj.transform.position = continuePoint[0].transform.position;
        }
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && SceneData.Instance.wayPoint_1)
        {
            playerObj.transform.position = continuePoint[1].transform.position;
        }
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && SceneData.Instance.wayPoint_2)
        {
            playerObj.transform.position = continuePoint[2].transform.position;
        }
        Cursor.visible = false;

        var playerInput = GameManager.Instance.playerInput;
        option = playerInput.actions["Option"];

        path = Application.dataPath + "/" + folderName + "/";
    }

    virtual protected void Update()
    {
        //�����I��
        if(!option.IsPressed())
        {
            getKayTime = 0;
        }
        else if (option.IsPressed())
        {
            getKayTime += Time.unscaledDeltaTime;

            if (getKayTime > startButtonTime)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        //�v���C���Ԏ擾
        if (playTimeMeasurement)
        {
            SceneData.Instance.playTime += Time.deltaTime;
        }

        //�X�N���[���V���b�g
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PrintScreen();
        }
    }

    public void playTimeStart()
    {
        playTimeMeasurement = true;
    }

    virtual public void playTimeStop()
    {
        playTimeMeasurement = false;
        //SceneData.Instance.PlayTimeSeve(Gamepara.StageType);
    }

    [Header("�ۑ���̐ݒ�")]
    [SerializeField]
    string folderName = "Screenshots";

    bool isCreatingScreenShot = false;
    string path;
    SoundManager soundManager;

    public void PrintScreen()
    {
        StartCoroutine("PrintScreenInternal");
    }

    IEnumerator PrintScreenInternal()
    {
        if (isCreatingScreenShot)
        {
            yield break;
        }

        isCreatingScreenShot = true;

        yield return null;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string date = DateTime.Now.ToString("yy-MM-dd_HH-mm-ss");
        string fileName = path + date + ".png";
        

        ScreenCapture.CaptureScreenshot(fileName);
        Debug.Log(fileName);

        yield return new WaitUntil(() => File.Exists(fileName));

        isCreatingScreenShot = false;
    }
}
