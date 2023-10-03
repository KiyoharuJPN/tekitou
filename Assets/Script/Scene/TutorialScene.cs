using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : StageCtrl
{
    [SerializeField]
    TutorialPlayer player;

    [SerializeField]
    CameraManager mainCamera;

    const string stageName = "Tutorial";

    //�i�s�x
    int num = 0;

    bool stepStart;
    bool goal = false;

    [System.Serializable]
    struct TutorialPanelObj
    {
        [SerializeField, Header("�`���[�g���A���p�l��")]
        internal GameObject tutorialPanel;
        [SerializeField, Header("�i�s�s�\��")]
        internal GameObject stopTiles;
        [SerializeField, Header("���[�v�h�A")]
        internal GameObject warpDoor;
        [SerializeField, Header("��p�J�����G���A")]
        internal GameObject cameraArea;
        public TutorialPanel tutorialArea;
        [SerializeField, Header("�N���A�����ɓG��|���ꍇ")]
        //�N���A���G��|�������̍ۂɎg�p
        internal List<Enemy> enemylist;
    }

    bool isAttackPlay = false;
    bool panelEnemyDaeth = false;

    [SerializeField, Header("�`���[�g���A���p�l��")]
    TutorialPanelObj[] tutorialPanels;


    //�`���[�g���A���o�t�l��������
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
                if (player.isNomalAttack && player.isGround )
                {
                    isAttackPlay = true;
                }
                if (!panelEnemyDaeth)
                {
                    int count = 0;
                    foreach (Enemy d in tutorialPanels[num].enemylist)
                    {
                        if(d.isDestroy) count++;
                    }
                    if (tutorialPanels[num].enemylist.Count == count) panelEnemyDaeth = true;
                }
                if(isAttackPlay && panelEnemyDaeth) return TutorialClear();
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
                if (PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.SpeedUp) > 0)
                {
                    return TutorialClear();
                }
                break;
            case 6:
                if (PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.SpeedUp) > 1 && !getCheckBuff.speedUp)
                {
                    getCheckBuff.speedUp = true;
                }
                if(PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.ExGage) > 0 && !getCheckBuff.exGage)
                {
                    getCheckBuff.exGage = true;
                }
                if(PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Slashing) > 0 && !getCheckBuff.slashing)
                {
                    getCheckBuff.slashing = true;
                }
                if(PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Invincible) > 0 && !getCheckBuff.invincible)
                {
                    getCheckBuff.invincible = true;
                }
                if(getCheckBuff.speedUp && getCheckBuff.exGage && getCheckBuff.slashing && getCheckBuff.invincible)
                {
                    return TutorialClear();
                }
                break;
            case 7:
                if (player.isSideAttack)
                {
                    return TutorialClear();
                }
                break;
            case 8:
                if (player.isUpAttack)
                {
                    return TutorialClear();
                }
                break;
            case 9:
                if (player.isDropAttack)
                {
                    return TutorialClear();
                }
                break;
            case 10:
                if (player.isExAttack)
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
        //���݂̃`���[�g���A���ɐi�s�s�\�ǂ�����Ȃ�
        if (tutorialPanels[num].stopTiles != null)
        {
            tutorialPanels[num].stopTiles.SetActive(false);
        }
        //���݂̃`���[�g���A���Ƀ��[�v�h�A������Ȃ�
        if (tutorialPanels[num].warpDoor != null)
        {
            tutorialPanels[num].tutorialPanel.transform.Find("TextBox_7").gameObject.SetActive(false);
            tutorialPanels[num].tutorialPanel.transform.Find("TextBox_8").gameObject.SetActive(true);
            StartCoroutine(SetWarpDoor(tutorialPanels[num].warpDoor));
        }
        ////���݂̃`���[�g���A���ɐ�p�J�����G���A������Ȃ�
        //if (tutorialPanels[num].cameraArea != null)
        //{
        //    mainCamera.NomalCameraAreaSet();
        //}

        if (tutorialPanels.Length == num + 1) {
            Debug.Log("�`���[�g���A���I��");
            goal = true;
            return true; 
        }

        ////���̃p�l���ɐ�p�J�����G���A������Ȃ�
        //if (tutorialPanels[num + 1].cameraArea != null) 
        //{
        //    mainCamera.SetOriCameraArea(tutorialPanels[num + 1].cameraArea);
        //}

        //���̃p�l����False�Ȃ�
        if (!tutorialPanels[num + 1].tutorialPanel.activeSelf)
        {
            StartCoroutine(ObjSetActive(1f, tutorialPanels[num].tutorialPanel, tutorialPanels[num + 1].tutorialPanel));
        }

        tutorialPanels[num].tutorialPanel.GetComponent<BoxCollider2D>().enabled = false;
        tutorialPanels[num].tutorialPanel.GetComponent<TutorialPanel>().enabled = false;

        return true;
    }

    IEnumerator ObjSetActive(float time,GameObject obj, GameObject nextObj)
    {
        yield return new WaitForSeconds(time);

        obj.SetActive(false);
        nextObj.SetActive(true);
    }

    IEnumerator SetWarpDoor(GameObject warpDoor)
    {
        //�o������
        float time = 1f;
        SpriteRenderer warpDoor_Sprite = warpDoor.GetComponent<SpriteRenderer>();
        float color_A = 0f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            color_A += 1f * Time.deltaTime;
            warpDoor_Sprite.color = new Color(warpDoor_Sprite.color.r, warpDoor_Sprite.color.g, warpDoor_Sprite.color.b, color_A);
            yield return null;
        }

        warpDoor.GetComponent<BoxCollider2D>().enabled = true;
        warpDoor.GetComponent<WarpDoor>().enabled = true;
    }

    //private bool EnemyDeathCheck()
    //{
    //    var enemyCount = enemylist.Count;
    //    var deathCount = 0;
    //    foreach(GameObject enemy in enemylist)
    //    {
    //        if(enemy.GetComponent<Enemy>().isDestroy) deathCount++;
    //    }

    //    if (deathCount == enemyCount)
    //    {
    //        enemylist.Clear();
    //        return true;
    //    }
    //    return false;
    //}

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
                break;
            case 4:
                player.isTAirAttack = true;
                break;
            case 7:
                player.isTSideAttack = true;
                break;
            case 8:
                player.isTUpAttack = true;
                break;
            case 9:
                player.isTDownAttack = true;
                break;
            case 10:
                ExAttackParam.Instance.SetGage(50);
                player.canExAttack = true;
                player.isTExAttack = true;
                break;
        }
        stepStart = true;
    }
}
