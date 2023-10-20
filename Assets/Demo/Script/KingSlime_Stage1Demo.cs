using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlime_Stage1Demo : KingSlime
{
    public override void Boss_Down()
    {
        GameManager.Instance.DemoStage1_BossDown();
    }
}
