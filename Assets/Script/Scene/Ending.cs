using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField]
    FadeImage fade;
    void Update()
    {
        if (fade.IsFadeInComplete()) return;
        if ((Input.GetKeyDown("joystick button 0")
            || Input.GetKeyDown("joystick button 1")
            || Input.GetKeyDown("joystick button 2")
            || Input.GetKeyDown("joystick button 3")))
        {
            SceneManager.LoadScene("Title");
        }        
    }
}
