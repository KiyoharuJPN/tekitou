using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboParam : MonoBehaviour
{
    [Tooltip("�\���p�e�L�X�g�{�b�N�X")]
    public TextMeshProUGUI text;   //���A���^�C���X�V�p�e�L�X�g�{�b�N�X
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
    ComboStatus comboStatus = new ComboStatus { Distime = 3 };

    [SerializeField]
    Image comboTimeGage;

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

    // Update is called once per frame
    void Update()
    {

        //if (countCombo == 0) text.gameObject.SetActive(false);//0�̎��ɉ�ʂ������


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
            StartCoroutine(_ComboTime());
        }
        countCombo = Cb;
        if (countCombo <= 0) countCombo = 0;//�R���{�ŏ���
        if (countCombo >= 999) countCombo = 999;//�R���{�ő��

        string SpriteText = countCombo.ToString();
        text.text = "";
        foreach (var i in SpriteText){
            text.text += "<sprite=" + i + ">";
        }

        ResetTime();
        
    }

    //�v�����ԃ��Z�b�g
    public void ResetTime()
    {
        time = 0;
        comboTimeGage.fillAmount = 1.0f;
    }

    public float GetPowerUp()
    {
        return (float)countCombo * (float)0.05;
    }

    IEnumerator _ComboTime()
    {
        while (time < comboStatus.Distime)
        {
            time += Time.deltaTime;
            comboTimeGage.fillAmount -= 1.0f / comboStatus.Distime * Time.deltaTime;
            yield return null;
        }

        countCombo = 0;
        string SpriteText = countCombo.ToString();
        text.text = "<sprite=" + SpriteText + ">";
    }
}
