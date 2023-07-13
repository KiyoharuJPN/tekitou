using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : StageCtrl
{
    [SerializeField]
    TutorialPlayer player;

    const string stageName = "Tutorial";

    //進行度
    int num = 0;

    bool stepStart;
    bool goal = false;

    [SerializeField, Header("進行不能壁")]
    GameObject[] stopTiles;
    [SerializeField, Header("チュートリアルパネル")]
    GameObject[] tutorialPanels;

    //クリアが敵を倒す条件の際に使用
    internal List<GameObject> enemylist = new List<GameObject>();


    private void Awake()
    {
        SceneData.Instance.referer = stageName;

        //各パネルを消す
        foreach(GameObject panel in tutorialPanels) 
        {
            panel.transform.Find("UI_Correct").GetComponent<SpriteRenderer>().enabled = false;
            panel.GetComponent<SpriteRenderer>().enabled = false;
        }

        tutorialPanels[0].GetComponent<SpriteRenderer>().enabled = true;
    }

    protected override void Start()
    {
        GameManager.Instance.ClearEnemyList();
        GameManager.Instance.PlayStart(1);
        Cursor.visible = false;

        base.Start();
        TutorialStep();

    }

    private void FixedUpdate()
    {
        if (TutorialClearCheck())
        {
            stepStart = false;
            tutorialPanels[num].transform.Find("UI_Correct").GetComponent<SpriteRenderer>().enabled = true;
            if (!goal)
            {
                tutorialPanels[num + 1].GetComponent<SpriteRenderer>().enabled = true;
            }
            SoundManager.Instance.PlaySE(SESoundData.SE.tutorialCorrect);
            num++;
        }
    }

    private bool TutorialClearCheck()
    {
        if(!stepStart) return false;
        switch (num)
        {
            case 0:
                if (player.isMoving)
                {
                    Destroy(stopTiles[0]);
                    return true;
                }
                break;
            case 1:
                if (player.isJumping)
                {
                    Destroy(stopTiles[1]);
                    return true;
                };
                break;
            case 2:
                if (player.jump.isSecondJump) return true;
                break;
            case 3:
                if (EnemyDeathCheck())
                {
                    Destroy(stopTiles[2]);
                    player.isTJump = true;
                    player.isTAirJump = true;
                    return true;
                }
                break;
            case 4:
                if ((player.jump.FarstJump || player.jump.isSecondJump) && player.isNomalAttack && EnemyDeathCheck())
                {
                    Destroy(stopTiles[3]);
                    return true;
                }
                break;
            case 5:
                if (EnemyDeathCheck())
                {
                    Destroy(stopTiles[4]);
                    return true;
                }
                break;
            case 6:
                if (EnemyDeathCheck())
                {
                    Destroy(stopTiles[5]);
                    player.isTJump = true;
                    player.isTAirJump = true;
                    return true;
                }
                break;
            case 7:
                if (EnemyDeathCheck())
                {
                    Destroy(stopTiles[6]);
                    ComboParam.Instance.SetCombo(0);
                    return true;
                }
                break;
            case 8:
                if (ComboParam.Instance.GetCombo() >= 5)
                {
                    Destroy(stopTiles[7]);
                    return true;
                }
                break;
            case 9:
                if (EnemyDeathCheck())
                {
                    Destroy(stopTiles[8]);
                    return true;
                }
                break;
            case 10:
                if (EnemyDeathCheck())
                {
                    Destroy(stopTiles[9]);
                    ComboParam.Instance.ComboResume();
                    return true;
                }
                break;
            case 11:
                if (EnemyDeathCheck())
                {
                    Destroy(stopTiles[10]);
                    goal = true;
                    return true;
                }
                else if (!player.canExAttack) {
                    ExAttackParam.Instance.SetGage(50);
                    player.canExAttack = true;
                }
                break;
        }
        return false;
    }

    private bool EnemyDeathCheck()
    {
        var enemyCount = enemylist.Count;
        var deathCount = 0;
        foreach(GameObject enemy in enemylist)
        {
            if(enemy.GetComponent<Enemy>().isDestroy) deathCount++;
        }

        if (deathCount == enemyCount)
        {
            enemylist.Clear();
            return true;
        }
        return false;
    }

    internal void TutorialStep()
    {
        switch (num)
        {
            case 0:
                player.isTWalk = true;
                break;
            case 1:
                player.isTJump = true;
                break;
            case 2:
                player.isTAirJump = true;
                break;
            case 3:
                player.isTAttack = true;
                player.isTJump = false;
                player.isTAirJump = false;
                break;
            case 4:
                player.isTAttack = true;
                player.isTAirAttack = true;
                break;
            case 5:
                player.isTSideAttack = true;
                player.isTJump = false;
                player.isTAirJump = false;
                player.isTAttack = false;
                player.isTAirAttack = false;
                break;
            case 6:
                player.isTUpAttack = true;
                player.isTSideAttack = false;
                break;
            case 7:
                player.isTUpAttack = false;
                player.isTDownAttack = true;
                player.isTJump = true;
                player.isTAirJump = true;
                break;
            case 8:
                ComboParam.Instance.ComboStop();
                player.isTAttack = true;
                player.isTAirAttack = true;
                player.isTUpAttack = true;
                player.isTSideAttack = true;
                break;
            case 9:
                ComboParam.Instance.ComboStop();
                ComboParam.Instance.SetCombo(0);
                player.isTUpAttack = false;
                player.isTSideAttack = false;
                player.isTDownAttack = false;
                break;
            case 10:
                ComboParam.Instance.SetCombo(100);
                break;
            case 11:
                ExAttackParam.Instance.SetGage(50);
                player.isTExAttack = true;
                player.canExAttack = true;
                player.isTJump = false;
                player.isTAirJump = false;
                player.isTAttack = false;
                player.isTAirAttack = false;
                break;
        }
        stepStart = true;
    }
}
