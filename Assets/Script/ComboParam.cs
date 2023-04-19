using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ComboParam : MonoBehaviour
{
    [Tooltip("�\���p�e�L�X�g�{�b�N�X")]
    public Text text;   //���A���^�C���X�V�p�e�L�X�g�{�b�N�X
    private int countCombo, CCb_preb;
    private float time;

    [System.Serializable]
    struct ComboStatus
    {
        [Tooltip("Combo���Ŏ���")]
        public int Distime;
        //���JStatus�ǉ��p
    }
    [SerializeField]
    [Header("Combo�X�e�[�^�X")]
    ComboStatus comboStatus = new ComboStatus { Distime = 3 };

    // Start is called before the first frame update
    void Start()
    {
        countCombo = 0;
        CCb_preb = 0;//�X�V����p
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (countCombo <= 0) countCombo = 0;//�R���{�ŏ���
        if (countCombo >= 999) countCombo = 999;//�R���{�ő��
        //if (countCombo == 0) text.gameObject.SetActive(false);//0�̎��ɉ�ʂ������

        if (countCombo != 0 && countCombo != CCb_preb || countCombo == 1)//�\���R�[�h
        {
            //if (!text.gameObject.activeSelf) text.gameObject.SetActive(true);
            CCb_preb = countCombo;
            text.text = "<size=25>X</size><size=50> " + countCombo + " </size><size=30>COMBO</size>";
            time = 0;
        }

        //Debug.Log("time"+time +"distime"+comboStatus.Distime);
        time += Time.deltaTime;//���Ԍo�߂̌v�Z
        if (time > comboStatus.Distime)//��莞�Ԃ����ƃ[���ɖ߂�
        {
            countCombo = 0;
        }
    }




    //�Q�b�g�Z�b�g�֐�
    public int GetCombo()
    {
        return countCombo;
    }
    public void SetCombo(int Cb)
    {
        countCombo = Cb;
    }

    public float GetPowerUp()
    {
        return (float)countCombo * (float)0.05;
    }
}
