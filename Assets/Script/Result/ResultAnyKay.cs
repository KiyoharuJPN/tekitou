using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultAnyKay : MonoBehaviour
{
    public Result result;

    string loadScene = "";

    private void Awake()
    {
        this.enabled = false;
    }

    private void Start()
    {
        loadScene = SceneData.Instance.referer;
        result = this.GetComponent<Result>();
    }

    private void Update()
    {
        if (!result.getCanAnyKey) return;

        if (Input.GetKeyDown("joystick button 0")
            || Input.GetKeyDown("joystick button 1")
            || Input.GetKeyDown("joystick button 2")
            || Input.GetKeyDown("joystick button 3"))//
        {
            if (loadScene == "Stage2")
            {
                SceneManager.LoadScene("Ending");
                return;
            }
            SceneManager.LoadScene("Load");
        }
    }
}
