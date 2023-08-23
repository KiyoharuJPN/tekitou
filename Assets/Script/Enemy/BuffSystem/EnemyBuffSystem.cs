using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyBuffSystem : MonoBehaviour
{
    [SerializeField, Tooltip("��{�ǌ���")]
    int initialBuffAttackCheck = 3;
    int BuffAttackCheck;

    TextMeshProUGUI BuffAttackCheckText;
    GameObject BuffCanvas;

    bool checkBlowingUp = false;

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
    public GameObject TextObject,CanvasObject;

    private void Start()
    {
        if(buffType == SetBuffType.RandomSet)
        {
            var newbuffType = (int)Random.Range(0, (float)SetBuffType.NoBuff);
            buffType = (SetBuffType)newbuffType;
        }
        if (GameObject.Find("BuffCanvas"))
        {
            BuffCanvas = GameObject.Find("BuffCanvas");
        }
        else
        {
            BuffCanvas = Instantiate(CanvasObject);
            BuffCanvas.name = "BuffCanvas";
        }
        BuffAttackCheckText = Instantiate(TextObject,BuffCanvas.transform).GetComponent<TextMeshProUGUI>();
        BuffAttackCheckText.gameObject.SetActive(false);
        BuffAttackCheck = initialBuffAttackCheck;
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
            BuffAttackCheck = GetBuffAcquisitionCount();
            BuffAttackCheckText.color = GetColorByType();
            BuffAttackCheckText.text = "" + BuffAttackCheck-- + "";
            BuffAttackCheckText.gameObject.SetActive(true);
            return;
        }

        //�J�E���g�����炵�ĕ\������
        BuffAttackCheckText.text = "" + BuffAttackCheck-- + "";
        if(BuffAttackCheck < 0)
        {
            if (!checkBlowingUp)
            {
                checkBlowingUp = true;
                var enemys = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var emy in enemys)
                {
                    if (emy.GetComponent<Enemy>().isDestroy)
                    {
                        emy.GetComponent<Enemy>().BuffBoostSphere();
                    }
                }
                checkBlowingUp = false;
            }

            Debug.Log("Enemy Dead");
            _Destroy();
        }

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

    //�擾�񐔂��O�Ŏ擾����
    public int GetBuffAcquisitionCount()
    {
        int count = 0;
        switch (buffType)
        {
            case SetBuffType.HeroExSkillGaugeUp:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.ExGage);
                break;
            case SetBuffType.HeroSpeedUp:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.SpeedUp);
                break;
            case SetBuffType.HeroSlashingBuff:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Slashing);
                break;
            case SetBuffType.HeroinvincibleBuff:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Invincible);
                break;
            default:
                count = 0;
                break;
        }
        return initialBuffAttackCheck + (count / 2);
    }

    //������ё��x���O�Ŏ擾����
    public int GetBuffBlowingSpeed()
    {
        int count = 0;
        switch (buffType)
        {
            case SetBuffType.HeroExSkillGaugeUp:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.ExGage);
                break;
            case SetBuffType.HeroSpeedUp:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.SpeedUp);
                break;
            case SetBuffType.HeroSlashingBuff:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Slashing);
                break;
            case SetBuffType.HeroinvincibleBuff:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Invincible);
                break;
            default:
                count = 0;
                break;
        }
        return count;
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
