using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoStage : StageCtrl
{
    const string stageName = "Stage1";

    private void Awake()
    {
        SceneData.Instance.referer = stageName;
    }

    protected override void Start()
    {
        GameManager.Instance.ClearEnemyList();
        GameManager.Instance.PlayStart(1);
        Cursor.visible = false;

        base.Start();
    }
    protected override void Update()
    {
        base.Update();
    }
}
