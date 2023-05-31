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
        GameManager.Instance.PlayBGM(1);
    }
}
