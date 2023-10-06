using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon_Demo : Dragon
{
    public override void Boss_Down()
    {
        GameManager.Instance.DemoStage_BossDown();
    }
}
