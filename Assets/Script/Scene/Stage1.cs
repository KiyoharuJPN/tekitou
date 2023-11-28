using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : StageCtrl
{
    public string stageName = "Stage1";

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
    protected override void Update()
    {
        base.Update();
    }

    public override void playTimeStop()
    {
        base.playTimeStop();
        SceneData.Instance.PlayTimeSeve(Gamepara.StageType.stage1);
    }
}
