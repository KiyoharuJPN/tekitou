using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : StageCtrl
{
    const string stageName = "Stage1";

    private void Awake()
    {
        SceneData.Instance.referer = stageName;
    }

    protected override void Start()
    {
        GameManager.Instance.ClearEnemyList();
        GameManager.Instance.PlayStart(2);
        Cursor.visible = false;

        base.Start();
    }
}
