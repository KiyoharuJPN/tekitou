using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    enum SelectStage
    {
        Tutorial = 0,
        Stage1 = 1,
        Stage2 = 2,
        Stage3 = 3,
    }

    private SelectStage selectStage = 0;

    public Text[] selectMenu;

    //���j���[�\���m�FBool
    private bool isSelectMenu = false;

    public Canvas menuTextObj;
    private Color color = new Color(255, 69, 0);

    public bool PauseCheck()
    {
        return isSelectMenu;
    }

    public void StageSelectStart()
    {
        Time.timeScale = 0;
        isSelectMenu = true;
        this.GetComponent<Canvas>().enabled = true;
    }

    public void MenuUpdata()
    {
        //�����L�[�̐ݒ�
        if (Input.GetKeyDown(KeyCode.W) && (int)selectStage > 0) ChangePointer(-1);

        if (Input.GetKeyDown(KeyCode.S) && (int)selectStage < 3) ChangePointer(1);

        //�I���L�[�̐ݒ�
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Scene_Start();
        }

        ////�߂�L�[�̐ݒ�
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    BackMenu();
        //}
    }

    public void PauseStart()
    {
        menuTextObj.enabled = true;
        isSelectMenu = true;
        Time.timeScale = 0;
        ChangePointer(0);
    }

    public void BackPause()
    {
        menuTextObj.enabled= false;
        isSelectMenu = false;
    }

    public void BackGame()
    {
        menuTextObj.enabled = false;
        isSelectMenu = false;
        Time.timeScale = 1;
    }

    void ChangePointer(int pointer)
    {
        OnDeselected((int)selectStage);
        selectStage += pointer;
        OnSelected((int)selectStage);
    }

    private void Scene_Start()
    {
        SceneData.Instance.StageDataReset();

        SceneData.Instance.PlayTimeDelete();

        switch (selectStage) 
        {
            case SelectStage.Tutorial:
                SceneData.Instance.referer = "Title";
                break;
            case SelectStage.Stage1:
                SceneData.Instance.referer = "Tutorial";
                break;
            case SelectStage.Stage2:
                SceneData.Instance.referer = "Stage1";
                break;
            case SelectStage.Stage3:
                SceneData.Instance.referer = "Stage2";
                break;
        }
        Time.timeScale = 1;
        SceneManager.LoadScene("Load");
    }

    void OnSelected(int objNum)
    {
        selectMenu[objNum].color = color;               //UI�̐F�ύX
    }
    void OnDeselected(int objNum)
    {
        selectMenu[objNum].color = Color.white; //�F��߂�
    }

}
