using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : MonoBehaviour
{
    private void Awake()
    {
        SceneData.Instance.referer = "Tutorial";
    }
    private void Start()
    {
        Cursor.visible = false;
        GameManager.Instance.PlayStart(1);
    }
}
