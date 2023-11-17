using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seika_Tutorial : TutorialScene
{
    [SerializeField]
    TutorialPlayer player;

    [SerializeField]
    CameraManager mainCamera;

    public string stageName = "Tutorial";

    //進行度
    int num = 0;

    bool stepStart;
    bool goal = false;

    [System.Serializable]
    struct TutorialPanelObj
    {
        [SerializeField, Header("チュートリアルパネル")]
        internal GameObject tutorialPanel;
        [SerializeField, Header("進行不能壁")]
        internal GameObject stopTiles;
        [SerializeField, Header("ワープドア")]
        internal GameObject warpDoor;
        [SerializeField, Header("専用カメラエリア")]
        internal GameObject cameraArea;
        public TutorialPanel tutorialArea;
        [SerializeField, Header("クリア条件に敵を倒す場合")]
        //クリアが敵を倒す条件の際に使用
        internal List<Enemy> enemylist;
    }

    bool isAttackPlay = false;
    bool panelEnemyDaeth = false;

    [SerializeField, Header("チュートリアルパネル")]
    TutorialPanelObj[] tutorialPanels;


    //チュートリアルバフ獲得したか
    internal struct farstGetBaff
    {
        internal bool speedUp;
        internal bool exGage;
        internal bool slashing;
        internal bool invincible;
    }

    farstGetBaff getCheckBuff = new() { speedUp = false, exGage = false, slashing = false, invincible = false };

    private void Awake()
    {
        SceneData.Instance.referer = stageName;
    }

    protected override void Start()
    {
        GameManager.Instance.ClearEnemyList();
        GameManager.Instance.PlayStart(1);
        Cursor.visible = false;

        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            tutorialPanels[i].tutorialArea = tutorialPanels[i].tutorialPanel.GetComponent<TutorialPanel>();
        }

        base.Start();
    }

    private void FixedUpdate()
    {
        if (TutorialClearCheck())
        {
            stepStart = false;
            tutorialPanels[num].tutorialPanel.transform.Find("UI_Correct").GetComponent<SpriteRenderer>().enabled = true;
            SoundManager.Instance.PlaySE(SESoundData.SE.tutorialCorrect);
            num++;
        }
    }

    private bool TutorialClearCheck()
    {
        if (!stepStart || goal) return false;
        if (!tutorialPanels[num].tutorialArea.isPlayer)
        {
            return false;
        }
        switch (num)
        {
            case 0:
                if (player.isMoving)
                {
                    return TutorialClear();
                }
                break;
            case 1:
                if (player.isJumping)
                {
                    return TutorialClear();
                };
                break;
            case 2:
                if (player.jump.isSecondJump)
                    return TutorialClear();
                break;
            case 3:
                if (player.isNomalAttack && player.isGround)
                {
                    isAttackPlay = true;
                }
                if (!panelEnemyDaeth)
                {
                    int count = 0;
                    foreach (Enemy d in tutorialPanels[num].enemylist)
                    {
                        if (d.isDestroy) count++;
                    }
                    if (tutorialPanels[num].enemylist.Count == count) panelEnemyDaeth = true;

                }
                Debug.Log(isAttackPlay);
                if (isAttackPlay && panelEnemyDaeth) return TutorialClear();
                break;
            case 4:
                if (player.isNomalAttack && player.isJumping)
                {
                    isAttackPlay = true;
                }
                if (!panelEnemyDaeth)
                {
                    int count = 0;
                    foreach (Enemy d in tutorialPanels[num].enemylist)
                    {
                        if (d.isDestroy) count++;
                    }
                    if (tutorialPanels[num].enemylist.Count == count) panelEnemyDaeth = true;
                }
                if (isAttackPlay && panelEnemyDaeth) return TutorialClear();
                break;
            case 5:
                if (player.isSideAttack)
                {
                    return TutorialClear();
                }
                break;
            case 6:
                if (player.isUpAttack)
                {
                    return TutorialClear();
                }
                break;
            case 7:
                if (player.isDropAttack)
                {
                    return TutorialClear();
                }
                break;
            case 8:
                if (!panelEnemyDaeth)
                {
                    int count = 0;
                    foreach (Enemy d in tutorialPanels[num].enemylist)
                    {
                        if (d.isDestroy) count++;
                    }
                    if (tutorialPanels[num].enemylist.Count == count) panelEnemyDaeth = true;
                }
                if (player.tExAttackActivCheck && panelEnemyDaeth)
                {
                    return TutorialClear();
                }
                break;
            case 9:
                if (PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.SpeedUp) > 0 && !getCheckBuff.speedUp)
                {
                    getCheckBuff.speedUp = true;
                }
                if (getCheckBuff.speedUp)
                {
                    return TutorialClear();
                }
                break;
            case 10:
                if (PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.SpeedUp) > 1 && !getCheckBuff.speedUp)
                {
                    getCheckBuff.speedUp = true;
                }
                if (PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.ExGage) > 0 && !getCheckBuff.exGage)
                {
                    getCheckBuff.exGage = true;
                }
                if (PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Slashing) > 0 && !getCheckBuff.slashing)
                {
                    getCheckBuff.slashing = true;
                }
                if (PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Invincible) > 0 && !getCheckBuff.invincible)
                {
                    getCheckBuff.invincible = true;
                }
                if (getCheckBuff.speedUp && getCheckBuff.exGage && getCheckBuff.slashing && getCheckBuff.invincible)
                {
                    return TutorialClear();
                }
                break;
        }
        return false;
    }

    bool TutorialClear()
    {
        isAttackPlay = false;
        panelEnemyDaeth = false;
        //現在のチュートリアルに進行不能壁があるなら
        if (tutorialPanels[num].stopTiles != null)
        {
            tutorialPanels[num].stopTiles.SetActive(false);
        }
        ////現在のチュートリアルに専用カメラエリアがあるなら
        //if (tutorialPanels[num].cameraArea != null)
        //{
        //    mainCamera.NomalCameraAreaSet();
        //}

        if (tutorialPanels.Length == num + 1)
        {
            Debug.Log("チュートリアル終了");
            goal = true;
            return true;
        }

        ////次のパネルに専用カメラエリアがあるなら
        //if (tutorialPanels[num + 1].cameraArea != null) 
        //{
        //    mainCamera.SetOriCameraArea(tutorialPanels[num + 1].cameraArea);
        //}

        //次のパネルがFalseなら
        if (!tutorialPanels[num + 1].tutorialPanel.activeSelf)
        {
            StartCoroutine(ObjSetActive(1f, tutorialPanels[num].tutorialPanel, tutorialPanels[num + 1].tutorialPanel));
        }

        tutorialPanels[num].tutorialPanel.GetComponent<BoxCollider2D>().enabled = false;
        tutorialPanels[num].tutorialPanel.GetComponent<TutorialPanel>().enabled = false;

        return true;
    }

    IEnumerator ObjSetActive(float time, GameObject obj, GameObject nextObj)
    {
        yield return new WaitForSeconds(time);

        obj.SetActive(false);
        nextObj.SetActive(true);
    }

    internal void TutorialStep()
    {
        switch (num)
        {
            case 0:
                player.canTWalk = true;
                break;
            case 1:
                player.canTJump = true;
                break;
            case 2:
                player.canTAirJump = true;
                break;
            case 3:
                player.canTAttack = true;
                break;
            case 4:
                player.canTAirAttack = true;
                break;
            case 5:
                player.canTSideAttack = true;
                break;
            case 6:
                player.canTUpAttack = true;
                break;
            case 7:
                player.canTDownAttack = true;
                break;
            case 8:
                ExAttackParam.Instance.SetGage(50);
                player.canExAttack = true;
                player.canTExAttack = true;
                player.canTExGageGet = true;
                break;
            case 9:
                PlayerBuff.Instance.BuffCountReset();
                break;
            case 10:
                getCheckBuff.speedUp = false;
                break;

        }
        stepStart = true;
    }
}
