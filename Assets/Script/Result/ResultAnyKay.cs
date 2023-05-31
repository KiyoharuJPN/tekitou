using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultAnyKay : MonoBehaviour
{
    Result result;

    private void Awake()
    {
        this.enabled = false;
    }

    private void Start()
    {
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
            SceneManager.LoadScene("Load");
        }
    }
}
