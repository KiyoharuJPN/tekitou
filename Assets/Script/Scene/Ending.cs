using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField]
    private FadeImage fade;
    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (!fade.IsFadeInComplete()) return;
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Title");
        }        
    }
}
