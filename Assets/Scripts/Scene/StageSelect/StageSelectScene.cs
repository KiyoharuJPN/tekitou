using UnityEngine;
using Cysharp.Threading.Tasks;//UniTask
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class StageSelectScene : MonoBehaviour, MenuBasic
{
    [SerializeField] private PauseMenu pauseMenu;
    MenuSystem openMenu;
    [SerializeField, Header("�v���C���[�A�C�R��")]
    GameObject playerIcon;
    Animator p_Animator;

    [SerializeField, Header("�ړ��|�C���g")]
    SelectMovePoint[] movePos;

    [SerializeField, Header("�t�F�[�h")]
    FadeImage fade;

    [SerializeField, Header("�e�X�e�[�W�̉摜")]
    GameObject[] stageImage;

    [SerializeField, Header("�|�C���g�̉摜")]
    Sprite[] pointSprites;

    enum SelectStageID
    {
        Stage1 = 0,
        Stage2 = 1,
        Stage3 = 2,
    }
    private SelectStageID selectStage = 0;

    public MapLine[] mapList;

    //�|�C���g�h��
    [System.Serializable]
    public struct ShakeInfo
    {
        [SerializeField, Header("�ő�h�ꕝ")]
        public Vector2 swingWigth;
        [SerializeField, Header("����")]
        public float duration;
        [SerializeField, Header("�U����")]
        public int vibrato;
        [SerializeField, Header("�U������͈�")]
        public float randomness;
    }
    [SerializeField, Header("�|�C���g�h��Ɋւ���ڍ�")]
    public ShakeInfo shakeInfo;

    //�ړ����m�FBool
    private bool isMove = false;
    private bool isEvent = false;
    public bool IsEvent { set { isEvent = value; } }
    private int openStageID;

    private PlayerInput playerInput;
    private InputAction move, decision, option;
    public bool canPause = true;

    //�v���C���[�A�C�R�����炵��
    const float IconShift = 0.7f;

    private void Awake()
    {
        p_Animator = playerIcon.GetComponent<Animator>();
    }

    private void Start()
    {
        System.GC.Collect();
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.StageSelect, BGMSoundData.BGM.none);

        playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        option = playerInput.actions["Option"];
        decision = playerInput.actions["Decision"];

        StagePointSet();

        canPause = true;
        SetMenu(pauseMenu);
    }

    private void Update()
    {
        if (!fade.IsFadeInComplete() || !fade.IsFadeOutComplete() ||fade.IsFadeEnRoute() || isEvent) return;

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
            return;
        }

        InputKey();
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
            openMenu.InputSet(playerInput);
        }
        else openMenu = pauseMenu;
    }

    private void InputKey()
    {
        if(isMove) { return; }

        var inputMoveAxis = move.ReadValue<Vector2>();

        //����L�[
        if (decision.WasPerformedThisFrame() && !isEvent)
        {
            isEvent = true;
            StartCoroutine(StageStart());
            return;
        }
        
        //���E�L�[
        if(inputMoveAxis.x >= 0.3 && (int)selectStage < 2)
        {
            IconMove(1);
            return;
        }
        
        if (inputMoveAxis.x <= -0.3 && (int)selectStage > 0)
        {
            IconMove(-1);
            return;
        }
    }

    //�v���C���[�A�C�R���ړ�
    private async void IconMove(int pointer)
    {
        isMove = true;
        if (!movePos[(int)selectStage + pointer].IsPlayPoint)
        {
            isMove = false;
            return;
        }
        SoundManager.Instance.PlaySE(SESoundData.SE.StageSelect_Move);
        movePos[(int)selectStage].transform.localScale = Vector3.one;
        selectStage += pointer;
        playerIcon.transform.position = movePos[(int)selectStage].GetPos + new Vector2(0, 0.7f);
        movePos[(int)selectStage].transform.localScale = new Vector3(1.5f,1.5f,1f);

        p_Animator.Play("Select_Jump");
        await UniTask.Delay(210);

        _ = playerIcon.transform.DOPunchPosition(shakeInfo.swingWigth, shakeInfo.duration, shakeInfo.vibrato, shakeInfo.randomness);
        movePos[(int)selectStage].StartShake(shakeInfo.swingWigth, shakeInfo.duration, shakeInfo.vibrato, shakeInfo.randomness);

        await UniTask.Delay(200);
        isMove = false;
    }

    private void StagePointSet()
    {
        //�X�e�[�W��Ԑݒ�
        var eathStageData = SceneData.Instance.GetEachStageState;

        switch (SceneData.Instance.referer)
        {
            case "Tutorial":
                selectStage = SelectStageID.Stage1;
                break;
            case "Stage1":
                selectStage = SelectStageID.Stage1;
                break;
            case "Stage2": 
                selectStage = SelectStageID.Stage2;
                break;
            case "Stage3":
                selectStage = SelectStageID.Stage3;
                break;
        }

        playerIcon.transform.position = movePos[(int)selectStage].GetPos + new Vector2(0, IconShift);

        //for (int i = 0; i < eathStageData.Length; i++)
        //{
        //    Debug.Log("isClea�Fstege" + i + eathStageData[i].isClear);

        //    Debug.Log("isOpen�Fstege" + i + eathStageData[i].openStage);
        //}

        //�|�C���g�F�ύX
        for (int i = 1; i <= movePos.Length; i++)
        {
            if (!eathStageData[i].isClear && !eathStageData[i].openStage)
            {
                movePos[i-1].IsPlayPoint = false;
                movePos[i-1].PointImage.sprite = pointSprites[0];
            }
            else
            if (eathStageData[i].openStage)
            {
                movePos[i-1].IsPlayPoint = true;
                movePos[i-1].PointImage.sprite = pointSprites[2];
            }
            if (eathStageData[i].isClear)
            {
                movePos[i - 1].PointImage.sprite = pointSprites[1];
            }

            //�X�e�[�W�I�[�v��������̏ꍇ
            if (eathStageData[i].firstOpen)
            {
                SceneData.Instance.StagePlay(i);
                openStageID = i - mapList.Length;
                isEvent = true;
            }
            else if(i >= mapList.Length && eathStageData[i].openStage)
            {
                mapList[i - mapList.Length].MapSet();
            }
        }

        if(isEvent)
        {
           mapList[openStageID].MapFirstSet(this);
        }
    }

    private IEnumerator StageStart()
    {
        switch (selectStage)
        {
            case SelectStageID.Stage1:
                SceneData.Instance.referer = "Tutorial";
                break;
            case SelectStageID.Stage2:
                SceneData.Instance.referer = "Stage1";
                break;
            case SelectStageID.Stage3:
                SceneData.Instance.referer = "Stage2";
                break;
        }

        p_Animator.Play("Select_Start");
        SoundManager.Instance.PlaySE(SESoundData.SE.StageSelect_Decision);
        yield return new WaitForSeconds(1.2f);

        fade.StartFadeOut();
        SceneData.Instance.StageDataReset();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("Load");
    }
}
