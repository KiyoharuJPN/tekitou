using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [Header("シーンID（0:タイトル,1:チュートリアル,2:ステージ1,3:リザルト,4:ゲームオーバー")]
    [Range(0, 4)]
    [SerializeField]
    int ID;
    [SerializeField]
    PlayerController player;

    GameObject[] enemys;
    List<GameObject> enemyList = new List<GameObject>();

    private int maxCombo;
    private int killEnemy;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        maxCombo = 0;
        killEnemy = 0;
        switch (ID)
        {
            case 0:
                BGMStart_Title();
                break;
            case 1:
                BGMStart_Tutorial();
                break;
            case 2:
                BGMStart_Stage1();
                break;
            case 3:
                BGMStart_GameOver();
                break;
        }
    }

    public void AddMaxComobo(int combo)
    {
        if( combo > maxCombo)
        {
            maxCombo = combo;
        }
    }

    public void AddKillEnemy()
    {
        killEnemy++;
    }

    public void Result_Start(int StageID)
    {
        player.canMove = false;
        Result.Instance.Result_Set(StageID, 
            PointParam.Instance.GetPoint(), maxCombo, killEnemy);
        StartCoroutine(Result_True());
    }

    public void PlayerExAttack_Start()
    {
        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject gameObj in enemys)
        {
            enemyList.Add(gameObj);
            gameObj.GetComponent<Enemy>().EnemyStop();
        }
    }

    public void PlayerExAttack_HitEnemyEnd(List<GameObject> hitEnemyList, float powar)
    {
        foreach (GameObject gameObj in hitEnemyList)
        {
            gameObj.GetComponent<Enemy>().PlaeyrExAttack_HitEnemyEnd(powar);
            enemyList.Remove(gameObj);
        }
    }

    public void PlayerExAttack_End()
    {
        foreach (GameObject gameObj in enemyList)
        {
            gameObj.GetComponent<Enemy>().PlaeyrExAttack_End();
        }
        enemys = null;
        enemyList.Clear();
    }

    private void BGMStart_Title()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
    }
    private void BGMStart_Tutorial()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Tutorial_intro, BGMSoundData.BGM.Tutorial_roop);
    }
    private void BGMStart_Stage1()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Stage1_intro, BGMSoundData.BGM.Stage1_roop);
    }
    private void BGMStart_Result()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Result, BGMSoundData.BGM.none);
    }
    private void BGMStart_GameOver()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.GameOver, BGMSoundData.BGM.none);
    }

    IEnumerator Result_True()
    {
        yield return new WaitForSeconds(1f);
        BGMStart_Result();
        Result.Instance.Result_Start();
    }
}
