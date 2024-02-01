using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, MenuBasic
{

    public FadeImage fade;
    public PlayerController player;
    [SerializeField] private PauseMenu pauseMenu;
    MenuSystem openMenu;
    StageSelect debugMenu_StageSelect;

    public bool PauseCheck
    {
        get
        {
            if (pauseMenu.PauseCheck())
            {
                return true;
            }
            else if (debugMenu_StageSelect.PauseCheck())
            {
                return true;
            }
            else return false;
        }
    }

    GameObject[] enemys;
    List<GameObject> enemyList = new List<GameObject>();

    private int maxCombo;
    private int killEnemy;
    public bool canPause = false;

    public GameObject hitEffect;

    [SerializeField,Header("������")]
    List<MoveWall> moveWalls;

    public bool isBossRoom = false;

    public static GameManager Instance { get; private set; }

    //InputSystem
    public PlayerInput playerInput;
    internal InputAction option, back;

    [HideInInspector]
    public bool isPlayerExSkill { get; private set; }
    //�X�e�[�W�R���g���[��
    [SerializeField, Header("�X�e�[�W�R���g���[��")]
    StageCtrl stageCtrl;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    void Start()
    {
        maxCombo = 0;
        killEnemy = 0;
        if (SceneData.Instance.referer == "Title")
        {
            Destroy(gameObject);
        }
        var playerInput = GetComponent<PlayerInput>();
        option = playerInput.actions["Option"];
        back = playerInput.actions["Back"];

        SetMenu(pauseMenu);
    }

    private void Update()
    {
        //�t�F�[�h�C���E�A�E�g���͏������s��Ȃ�
        if (!fade.IsFadeInComplete() || !fade.IsFadeOutComplete() || fade.IsFadeEnRoute()) return;

        //�W���p
        /////////////////////////////////////////
        //DebugMenu����
        if (Input.GetKey(KeyCode.LeftControl)
            && Input.GetKey(KeyCode.LeftShift)
            && !debugMenu_StageSelect.PauseCheck())
        {
            debugMenu_StageSelect.PauseStart();
        }
        else if (Input.GetKeyDown(KeyCode.Escape)
            && debugMenu_StageSelect.PauseCheck())
        {
            if(pauseMenu.PauseCheck())
            {
                debugMenu_StageSelect.BackPause();
            }
            else
            {
                debugMenu_StageSelect.BackGame();
            }
        }
        //�X�e�[�W�Z���N�g��Update
        if (debugMenu_StageSelect.PauseCheck())
        {
            debugMenu_StageSelect.MenuUpdata();
            return;
        }
        ///////////////////////////////////////////////

        //�|�[�Y���
        if (option.WasPressedThisFrame() && canPause) 
        {
            if (!openMenu.PauseCheck())
            {
                openMenu.InputSet(playerInput, this);
            }
            else if (openMenu.PauseCheck())
            {
                MenuBack();
            }
        }
        if (openMenu.PauseCheck())
        {
            openMenu.MenuUpdata();
        }
    }

    public void SetMenu(MenuSystem menu)
    {
        openMenu = menu;
    }

    public void MenuBack()
    {
        openMenu = openMenu.Back();
        if (openMenu != null)
        {
            openMenu.InputSet(playerInput, this);
        }
        else openMenu = pauseMenu;
    }

    public void PlayStart(int ID)
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        fade = GameObject.FindWithTag("FadeImage").GetComponent<FadeImage>();
        pauseMenu = GameObject.FindWithTag("PauseMenu").GetComponent<PauseMenu>();
        debugMenu_StageSelect = GameObject.FindWithTag("DebugMenu").GetComponent<StageSelect>();

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
            case 4:
                BGMStart_Stage2();
                break;
            case 5:
                BGMStart_Stage3();
                break;
        }

        canPause = true;

        if(stageCtrl != null)
        {
            stageCtrl.playTimeStart();
        }
    }

    //�v���C���[��~����
    public void PlayerStop()
    {
        player.playerState = PlayerController.PlayerState.Event;
    }
    public void PlayerMove()
    {
        player.playerState = PlayerController.PlayerState.Idle;
    }

    //�v���C���[���S������
    public void PlayerDeath()
    {
        StartCoroutine(_PlyerDeath());
    }
    public void DemoPlayerDeath()
    {
        StartCoroutine(_DemoPlyerDeath());
    }

    public void AddMaxComobo(int combo)
    {

        if( combo > maxCombo)
        {
            maxCombo = combo;

            //steam���ъm�F
            if (combo >= 50)
            {
                Accmplisment.Instance.AchvOpen("Combo1");
            }
            else if (combo >= 100) Accmplisment.Instance.AchvOpen("Combo2");
            else if(combo >= 500) Accmplisment.Instance.AchvOpen("Combo2");
        }
    }

    public void AddKillEnemy()
    {
        killEnemy++;
    }

    public void SetCanPause(bool bl) {
        canPause = bl;
    }
    public bool GetCanPause() { 
        return canPause;
    }
    public void Result_Start(int StageID)
    {
        player.SetCanMove(false);
        Result.Instance.Result_Set(StageID, 
            PointParam.Instance.GetPoint(), maxCombo, killEnemy);

        StartCoroutine(Result_True());
    }
    public void StopRecordTime()
    {
        if (stageCtrl != null)
            stageCtrl.playTimeStop();
        else UnityEngine.Debug.Log("�X�e�[�W�R���g���[���[��ǉ����Ă�������");
    }
    public void StartRecordTime()
    {

        if (stageCtrl != null)
            stageCtrl.playTimeStart();
        else UnityEngine.Debug.Log("�X�e�[�W�R���g���[���[��ǉ����Ă�������");
    }

    public void EnemyStop_Start()
    {
        enemyList.Clear();
        enemys = null;
        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject gameObj in enemys)
        {
            enemyList.Add(gameObj);
            gameObj.GetComponent<Enemy>().EnemyStop();
        }
    }
    public void EnemyStop_End()
    {
        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject gameObj in enemys)
        {
            enemyList.Add(gameObj);
            gameObj.GetComponent<Enemy>().Stop_End();
        }
    }

    public void PlayerExAttack_Start()
    {
        isPlayerExSkill = true;

        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        if(moveWalls.Count != 0)
        {
            foreach (MoveWall moveWall in moveWalls)
            {
                moveWall.MoveStop = true;
            }
        }

        foreach (GameObject gameObj in enemys)
        {
            enemyList.Add(gameObj);
            if (gameObj.GetComponent<Enemy>())
            {
                gameObj.GetComponent<Enemy>().EnemyStop();
            }
            else if(gameObj.GetComponent<Projectile>())
            {
                gameObj.GetComponent<Projectile>().EnemyStop();
            }
        }
    }

    public void PlayerExAttack_HitEnemyEnd(List<GameObject> hitEnemyList, float powar)
    {
        isPlayerExSkill = false;

        foreach (MoveWall moveWall in moveWalls)
        {
            moveWall.MoveStop = false;
        }

        foreach (GameObject gameObj in hitEnemyList)
        {
            if(gameObj != null)
            {
                ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
                gameObj.GetComponent<Enemy>().PlaeyrExAttack_HitEnemyEnd(powar);
                enemyList.Remove(gameObj);
            }
        }
    }

    public void PlayerExAttack_End()
    {
        isPlayerExSkill = false;

        foreach (MoveWall moveWall in moveWalls)
        {
            moveWall.MoveStop = false;
        }

        foreach (GameObject gameObj in enemyList)
        {

            if (gameObj != null && gameObj.GetComponent<Enemy>())
            {
                gameObj.GetComponent<Enemy>().Stop_End();
            }
            else if (gameObj != null && gameObj.GetComponent<Projectile>())
            {
                gameObj.GetComponent<Projectile>().Stop_End();
            }
        }
        enemys = null;
        enemyList.Clear();

        foreach (MoveWall moveWall in moveWalls)
        {
            moveWall.MoveStop = false;
        }
    }

    public void ClearEnemyList()
    {
        enemyList.Clear();
    }

    private void BGMStart_Title()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
    }
    public void BGMStart_Tutorial()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Tutorial_intro, BGMSoundData.BGM.Tutorial_roop);
    }
    private void BGMStart_Stage1()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Stage1_intro, BGMSoundData.BGM.Stage1_roop);
    }
    private void BGMStart_Stage2()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Stage2_intro, BGMSoundData.BGM.Stage2_roop);
    }
    private void BGMStart_Stage3()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Stage3, BGMSoundData.BGM.none);
    }
    public void BGMStart_BossRoom()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.KingSlimeBoss_intro, BGMSoundData.BGM.KingSlimeBoss_roop);
    }
    public void BGMStart_BossRoom2()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Stage3_Boss_intro, BGMSoundData.BGM.Stage3_Boss_roop);
    }
    private void BGMStart_Result()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Result, BGMSoundData.BGM.none);
    }
    private void BGMStart_GameOver()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.GameOver_intro, BGMSoundData.BGM.GameOver_roop);
    }

    //���G������߂����ۂ�BGM�Đ�
    public void BGMBack()
    {
        if (isBossRoom)//�{�X�풆�������ꍇ
        {
            if(SceneData.Instance.referer == "Stage3")
            {
                BGMStart_BossRoom2();
            }
            else
            {
                BGMStart_BossRoom();
            }
        }
        else if(!isBossRoom)
        {
            switch (SceneData.Instance.referer)
            {
                case "Tutorial":
                    BGMStart_Tutorial(); break;
                case "Stage1":
                    BGMStart_Stage1(); break;
                case "Stage2":
                    BGMStart_Stage2(); break;
                case "Stage3":
                    BGMStart_Stage3(); break;
            }
        }
    }

    IEnumerator Result_True()
    {
        yield return new WaitForSeconds(1f);
        
        Result.Instance.Result_Start();

        BGMStart_Result();
    }

    private IEnumerator _PlyerDeath()
    {
        GameManager.Instance.PlayTimeStop();
        _ = player.AttackEnd();
        player.SetCanMove(false);
        Time.timeScale = 0.3f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;

        fade.StartFadeOut();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        if (GameObject.Find("ObjectPool") != null)
        {
            GameObject.Find("ObjectPool").GetComponent<ObjectPoolScript>().SceneReset();
        }

        System.GC.Collect();
        Resources.UnloadUnusedAssets();

        if (SceneData.Instance.stock >= 1)
        {
            SceneData.Instance.stock--;
            //�f���X�e�[�W�݂̂̏���
            if (SceneData.Instance.referer == "Demo") SceneData.Instance.stock++;
            SceneData.Instance.revival = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneData.Instance.revival = false;
            SceneManager.LoadScene("FinishScene");
        }
    }

    private IEnumerator _DemoPlyerDeath()
    {
        player.canMove = false;
        Time.timeScale = 0.3f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;

        fade.StartFadeOut();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        if (SceneData.Instance.stock >= 1)
        {
            SceneData.Instance.stock--;
            //�f���X�e�[�W�݂̂̏���
            if (SceneData.Instance.referer == "Demo") SceneData.Instance.stock++;
            SceneData.Instance.revival = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneData.Instance.revival = false;
            SceneManager.LoadScene("FinishScene_Demo");
        }
    }

    internal void PauseBack()
    {
        player.SetCanMove(true);
    }

    /// <summary>
    /// �o�t�t�^
    /// </summary>
    public void SetBuff(int id)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.BuffGet);
        
        PlayerBuff.Instance.BuffSet(id);
    }

    //�f������@�\
    [SerializeField, Header("�{�X�I����̃��[�v�|�C���g")]
    GameObject[] warpPoint;
    CameraManager camera;

    public void DemoStage_BossDown()
    {
        camera = GameObject.FindWithTag("MainCamera").GetComponent<CameraManager>();
        StartCoroutine(bossDownMove());
        IEnumerator bossDownMove()
        {
            player.SetCanMove(false);

            //�t�F�[�h�A�E�g�J�n
            fade.StartFadeOut();

            while (!fade.IsFadeOutComplete())
            {
                yield return null;
            }

            //�t�F�[�h�A�E�g�I��
            ComboParam.Instance.ResetTime(); SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void DemoStage1_BossDown()
    {
        StartCoroutine(bossDownMove());
        IEnumerator bossDownMove()
        {
            player.SetCanMove(false);

            //�t�F�[�h�A�E�g�J�n
            fade.StartFadeOut();

            while (!fade.IsFadeOutComplete())
            {
                yield return null;
            }
            SceneData.Instance.revival = false;
            SceneData.Instance.wayPoint_1 = false;
            SceneData.Instance.wayPoint_2 = false;

            //�t�F�[�h�A�E�g�I��
            ComboParam.Instance.ResetTime(); SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //�v���C�^�C���v���֌W�̊֐�
    public void PlayTimeStop()
    {
        if(stageCtrl != null)
        {
            stageCtrl.playTimeStop();
        }
    }

    public void PlayTimeStart()
    {
        if (stageCtrl != null)
        {
            stageCtrl.playTimeStart();
        }
    }
}
