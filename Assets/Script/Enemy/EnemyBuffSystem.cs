using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyBuffSystem : MonoBehaviour
{
    [SerializeField, Tooltip("Buff���l������܂ł̕K�v�U����")]
    int BuffAttackCheck = 3;
    

    TextMeshProUGUI BuffAttackCheckText;
    GameObject BuffCanvas;

    //�\����(�b��)
    public enum DisplayType
    {
        EnemyLive,
        EnemyDead,
        Alltime,
    }

    //�o�t���
    public enum SetBuffType
    {
        HeroExSkillGaugeUp,
        HeroSpeedUp,
        HeroSlashingBuff,
        HeroinvincibleBuff,
        NoBuff,
        RandomSet,
    }

    //Enemy enemy;
    //public DisplayType displayType = DisplayType.Alltime;
    public Vector3 intervalPos;
    public SetBuffType buffType = SetBuffType.NoBuff;
    public GameObject[] DeadEffect;
    public GameObject TextObject;

    private void Start()
    {
        if(buffType == SetBuffType.RandomSet)
        {
            var newbuffType = (int)Random.Range(0, (float)SetBuffType.NoBuff);
            buffType = (SetBuffType)newbuffType;
        }
        BuffCanvas = GameObject.Find("BuffCanvas");
        BuffAttackCheckText = Instantiate(TextObject,BuffCanvas.transform).GetComponent<TextMeshProUGUI>();
        BuffAttackCheckText.gameObject.SetActive(false);
        //enemy = GetComponentInParent<Enemy>();
    }

    private void Update()
    {
        if (BuffAttackCheckText.gameObject.activeSelf)
        {
            BuffAttackCheckText.gameObject.transform.position = transform.position + intervalPos;
        }
    }

    //�ŏ��ɕ\�������A�^�b�N�K�v���̃Z�b�g
    public void SetBuffAttackCheckCount(int count)
    {
        BuffAttackCheck = count;
    }
    //�A�^�b�N�K�v���̃Q�b�g�֐�
    public int GetBuffAttackCheckCount()
    {
        return BuffAttackCheck;
    }

    //���ݎc��̃A�^�b�N�K�v���\��
    public void ShowAttackChecking()
    {
        //�|���ꂽ���͕\������������
        if (!BuffAttackCheckText.gameObject.activeSelf)
        {
            BuffAttackCheckText.color = GetColorByType();
            BuffAttackCheckText.text = "" + BuffAttackCheck-- + "";
            BuffAttackCheckText.gameObject.SetActive(true);
            return;
        }

        //�J�E���g�����炵�ĕ\������
        BuffAttackCheckText.text = "" + BuffAttackCheck-- + "";
        if(BuffAttackCheck < 0) _Destroy();

    }


    //BuffType���O����擾
    public SetBuffType GetBuffType()
    {
        return buffType;
    }


    //Buff�Ή��̃G�t�F�N�g���O����擾
    public GameObject GetBuffEffect()
    {
        return DeadEffect[(int)buffType];
    }

    //Buff�Ή��̐F���O����擾
    public Color GetColorByType(/*SetBuffType type*/)
    {
        Color color = new Color(0,0,0,0);
        switch (buffType)
        {
            case SetBuffType.HeroExSkillGaugeUp:
                color = HeroExSkillGaugeUpOrange();
                break;
            case SetBuffType.HeroSpeedUp:
                color = HeroSpeedUpBlue();
                break;
            case SetBuffType.HeroSlashingBuff:
                color = HeroSlashingBuffGreen();
                break;
            case SetBuffType.HeroinvincibleBuff:
                color = HeroinvincibleBuffYello();
                break;
            case SetBuffType.NoBuff:
            default:
                break;
        }
        return color;
    }


    //Buff�F�ݒ�
    Color HeroExSkillGaugeUpOrange()
    {
        //return new Color(243, 152, 0, 255);
        return new Color(0.95f, 0.53f, 0, 1);
    }
    Color HeroSpeedUpBlue()
    {
        //return new Color(0, 0, 255, 255);
        return new Color(0, 0, 1, 1);
    }
    Color HeroSlashingBuffGreen()
    {
        //return new Color(0, 255, 0, 255);
        return new Color(0, 1, 0, 1);
    }
    Color HeroinvincibleBuffYello()
    {
        //return new Color(255, 255, 0, 255);
        return new Color(1, 1, 0, 1);
    }

    public void _Destroy()
    {
        Destroy(BuffAttackCheckText.gameObject);
    }
}
