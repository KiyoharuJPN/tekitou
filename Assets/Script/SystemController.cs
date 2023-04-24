using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemController : MonoBehaviour
{
    string sceneNamePreb;

    public static SystemController Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name != "FinishScene")
        {
            if(SceneManager.GetActiveScene().name != sceneNamePreb)
            {
                sceneNamePreb = SceneManager.GetActiveScene().name;
            }
        }
    }





    //�F��ȃQ�b�g�Z�b�g�֐�
    public string GetLastScene()//�V�[���Q�b�g
    {
        return sceneNamePreb;
    }

}
