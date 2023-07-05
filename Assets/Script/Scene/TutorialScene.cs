using UnityEngine;

public class TutorialScene : StageCtrl
{
    const string stageName = "Tutorial";

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
}
