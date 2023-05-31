using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : MonoBehaviour
{
    private void Start()
    {
        SceneData.Instance.referer = "Tutorial";
        GameManager.Instance.PlayBGM(1);
    }
}
