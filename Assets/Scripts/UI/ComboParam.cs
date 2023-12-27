using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class ComboParam : MonoBehaviour
{
    [Tooltip("�\���p�e�L�X�g�{�b�N�X")]
    public TextMeshProUGUI text;   //���A���^�C���X�V�p�e�L�X�g�{�b�N�X
    public RectTransform comboCountObj;
    [Tooltip("�R���{UICanvas")]
    public Canvas comboCanvas;
    [Tooltip("�R���{ResultObj")]
    public GameObject comboResult;
    private int countCombo, CCb_preb;
    private float time;

    [System.Serializable]
    struct ComboStatus
    {
        [Header("Combo���Ŏ���")]
        public int Distime;
        //���JStatus�ǉ��p
    }
    [SerializeField]
    [Header("Combo�X�e�[�^�X")]
    ComboStatus comboStatus = new ComboStatus { Distime = 5 };

    [SerializeField]
    Image comboTimeGage;
    [SerializeField]
    Image comboTextImage;

    [SerializeField]
    PlayerController player;

    [System.Serializable]
    struct ComboImage
    {
        [Header("�Q�[�W")]
        public Sprite gageSprite;
        [Header("�e�L�X�g")]
        public Sprite textSprite;
    }
    [SerializeField]
    ComboImage[] comboImages = new ComboImage[5];

    [System.Serializable]
    struct ComboAnimDeta
    {
        [Header("���k�����܂ł̎���")]
        public float backTime;
        [Header("�g�嗦")]
        public float expansionRate;
        [Header("�g�厞�̈ʒu")]
        public Vector3 pos_Large;
        [Header("�ʏ펞�̈ʒu")]
        public Vector3 pos_Nomal;
    }
    [SerializeField]
    ComboAnimDeta comboAnim;
    Tweener posTween, scaleTween;
    Vector3 firstFontSize; //�R���{��Text�����T�C�Y�i�[�ϐ�

    bool isCombo;
    bool comboStop = false;

    public static ComboParam Instance { get; private set; }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        countCombo = 0;
        comboTimeGage.fillAmount = 0f;
        CCb_preb = 0;//�X�V����p
        text.text = "<sprite=" + 0 + ">"; 
        time = 0;
    }

    //�Q�b�g�Z�b�g�֐�
    public int GetCombo()
    {
        return countCombo;
    }

    public void SetCombo(int Cb)
    {
        
        if (countCombo == 0)
        {
            comboCanvas.enabled = true;
            StartCoroutine(ComboTimer());
        }

        countCombo = Cb;
        if (countCombo <= 0) countCombo = 0;//�R���{�ŏ���
        if (countCombo >= 999) countCombo = 999;//�R���{�ő��
        GameManager.Instance.AddMaxComobo(countCombo);

        ComboImageSet();

        ComboCountSet();

        //ComboAnim();

        ResetTime();
    }

    //�R���{���Z�b�g
    private void ComboCountSet()
    {
        string SpriteText = countCombo.ToString();
        text.text = "";
        foreach (var i in SpriteText)
        {
            int count = int.Parse(i.ToString());
            text.text += "<sprite=" + CountCheck(count) + ">";
        }

        //�R���{�����m�F���AspriteID��ύX���郍�[�J���֐�
        int CountCheck(int count)
        {
            if (countCombo >= 10 && countCombo < 20)
            {
                count += 10;
            }
            else if (countCombo >= 20 && countCombo < 50)
            {
                count += 20;
            }
            else if (countCombo >= 50 && countCombo < 100)
            {
                count += 30;
            }
            else if (countCombo >= 100)
            {
                count += 40;
            }
            return count;
        }

        ComboAnim();
    }

    //�R���{���ɉ������摜�Z�b�g
    private void ComboImageSet()
    {
        if (countCombo < 10)
        {
            comboTimeGage.sprite = comboImages[0].gageSprite;
            comboTextImage.sprite = comboImages[0].textSprite;
        }
        else if (countCombo >= 10 && countCombo < 20)
        {
            comboTimeGage.sprite = comboImages[1].gageSprite;
            comboTextImage.sprite = comboImages[1].textSprite;
        }
        else if (countCombo >= 20 && countCombo < 50)
        {
            comboTimeGage.sprite = comboImages[2].gageSprite;
            comboTextImage.sprite = comboImages[2].textSprite;
        }
        else if (countCombo >= 50 && countCombo < 100)
        {
            comboTimeGage.sprite = comboImages[3].gageSprite;
            comboTextImage.sprite = comboImages[3].textSprite;
        }
        else if (countCombo >= 100)
        {
            comboTimeGage.sprite = comboImages[4].gageSprite;
            comboTextImage.sprite = comboImages[4].textSprite;
        }
    }

    Sprite ComboText()
    {
        if (countCombo < 10)
        {
            return comboImages[0].textSprite;
        }
        else if (countCombo >= 10 && countCombo < 20)
        {
            return comboImages[1].textSprite;
        }
        else if (countCombo >= 20 && countCombo < 50)
        {
            return comboImages[2].textSprite;
        }
        else if (countCombo >= 50 && countCombo < 100)
        {
            return comboImages[3].textSprite;
        }
        else if (countCombo >= 100)
        {
            return comboImages[4].textSprite;
        }
        else
        {
            return null;
        }
    }

    public void ComboStop()
    {
        comboStop = true;
    }

    public void ComboResume()
    {
        comboStop = false;
    }

    //�v�����ԃ��Z�b�g
    public void ResetTime()
    {
        if (countCombo == 0) return;
        time = 0;
        comboTimeGage.fillAmount = 1.0f;
    }

    public float GetPowerUp()
    {
        return (float)countCombo * (float)0.05;
    }

    IEnumerator ComboTimer()
    {
        while (time < comboStatus.Distime)
        {
            if (player.playerState == PlayerController.PlayerState.ExAttack || comboStop ||
                player.isExAttack || player.isWarpDoor || !player.canMove)
            {
                yield return null;
            }
            else
            {
                time += Time.deltaTime;
                comboTimeGage.fillAmount -= 1.0f / comboStatus.Distime * Time.deltaTime;
                yield return null;
            }
        }

        comboCanvas.enabled = false;
        ComboResult(countCombo);
        countCombo = 0;
        time = 0;
        string SpriteText = countCombo.ToString();
        text.text = "<sprite=" + SpriteText + ">";
        
    }

    void ComboAnim()
    {
        if(posTween != null || scaleTween != null)
        {
            posTween.Kill();
            scaleTween.Kill();
            comboCountObj.localScale = firstFontSize;
        }
        //�����T�C�Y�ύX
        firstFontSize = comboCountObj.localScale;
        Vector2 vec = new(comboCountObj.localScale.x * comboAnim.expansionRate, comboCountObj.localScale.y * comboAnim.expansionRate);
        comboCountObj.localScale = new Vector3(vec.x, vec.y, 1);
        //�g�厞�ʒu�ɕύX
        comboCountObj.localPosition = comboAnim.pos_Large;

        //���ɖ߂�
        posTween = comboCountObj.DOAnchorPos(comboAnim.pos_Nomal, comboAnim.backTime).SetUpdate(true).OnComplete(() =>{ posTween = null;});
        scaleTween = comboCountObj.DOScale(firstFontSize, comboAnim.backTime).SetUpdate(true).OnComplete(() => { scaleTween = null; });
    }

    void ComboResult(int countCombo)
    {
        var resultObj = Instantiate(comboResult, player.gameObject.transform.position + new Vector3(0f, 1.5f), Quaternion.identity);
        resultObj.GetComponent<GetComboResult>().ComboSet(ComboText(),countCombo);
    }
}
