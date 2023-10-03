using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2 : StageCtrl
{
    const string stageName = "Stage2";

    private void Awake()
    {
        SceneData.Instance.referer = stageName;
    }

    protected override void Start()
    {
        GameManager.Instance.ClearEnemyList();
        GameManager.Instance.PlayStart(4);
        //Cursor.visible = false;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
