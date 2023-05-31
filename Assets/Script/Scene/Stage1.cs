using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : MonoBehaviour
{
    private void Awake()
    {
        SceneData.Instance.referer = "Stage1";
    }

    void Start()
    {
        GameManager.Instance.ClearEnemyList();
        GameManager.Instance.PlayBGM(2);
        Cursor.visible = false;
    }
}
