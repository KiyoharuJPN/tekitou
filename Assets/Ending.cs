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
        if (Input.anyKey)
        {
            SceneManager.LoadScene("Title");
        }        
    }
}
