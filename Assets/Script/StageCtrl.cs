using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StageCtrl : MonoBehaviour
{
    [Header("�v���C���[�Q�[���I�u�W�F�N�g")] public GameObject playerObj;
    [Header("�R���e�B�j���[�ʒu")] public GameObject[] continuePoint;

    //�N���b�V���΍�̒���������
    private float startButtonTime = 5f;
    private float getKayTime = 0;

    //�v���C���Ԍv��
    public bool playTimeMeasurement = true;
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
        
        Debug.Log(SceneData.Instance.playTime);
    }

    void playTimeStop()
    {

    }
}
