using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3 : StageCtrl
{
    const string stageName = "Stage3";

    private void Awake()
    {
        SceneData.Instance.referer = stageName;
    }

    protected override void Start()
    {
        GameManager.Instance.ClearEnemyList();
        GameManager.Instance.PlayStart(5);
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
        SceneData.Instance.PlayTimeSeve(Gamepara.StageType.stage3);
    }
}
