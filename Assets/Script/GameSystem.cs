
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    bool isShiftKey = false;
    bool isTKey = false;
    bool isSKey = false;
    bool isEKey = false;

    public static GameSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        KeyCheck();

        Menu();
    }

    //�L�[���͊m�F
    private void KeyCheck()
    {
        //�V�t�g�L�[�m�F
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isShiftKey = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isShiftKey = false;
        }
        //T�L�[�m�F
        if (Input.GetKeyDown(KeyCode.T))
        {
            isTKey = true;
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            isTKey = false;
        }
        //S�L�[�m�F
        if (Input.GetKeyDown(KeyCode.S))
        {
            isSKey = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            isSKey = false;
        }
        //E�L�[�m�F
        if (Input.GetKeyDown(KeyCode.E))
        {
            isEKey = true;
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            isEKey = false;
        }
    }

    //
    private void Menu()
    {
        if (isShiftKey)
        {
            if(isTKey) 
            {

                System.GC.Collect();
                Resources.UnloadUnusedAssets();
                SceneManager.LoadScene("Title");
            }
            if(isSKey)
            {
                System.GC.Collect();
                Resources.UnloadUnusedAssets();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if(isEKey) 
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
            }
        }
    }
}
