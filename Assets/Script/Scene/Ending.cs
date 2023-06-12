using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField]
    FadeImage fade;
    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {

        if (!fade.IsFadeInComplete()) return;
        if (Input.GetKeyDown("joystick button 0")
            || Input.GetKeyDown("joystick button 1")
            || Input.GetKeyDown("joystick button 2")
            || Input.GetKeyDown("joystick button 3") || Input.anyKeyDown
            || Input.GetKeyDown("joystick button 7"))
        {
            SceneManager.LoadScene("Title");
        }        
    }
}
