using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class BossHPBar : MonoBehaviour
{
    public float FillSpeed = 1, DecreaseSpeed = 1;

    [Header("�Q�[�W�̉摜")]
    [SerializeField]
    Image BossHPGauge;
    [SerializeField]
    Image BossHPBackGauge;

    Coroutine hpDecreasingCoro;

    float BossFullHP = 0, BossHP = 0;              //�{�X��FullHP�A�{�X��HP���L�^�p�AHP�̕΍��l

    bool StartAnimation = false, HPdecreasing = false;      //�J�n�A�j���[�V������������Ă��邩�ǂ������m�F�AHP��ύX����R���[�`���������Ă��邩�ǂ����̊m�F

    private void Start()
    {
        //������
        BossHPGauge.fillAmount = 0f;
        BossHPBackGauge.fillAmount = 0f;
        FillSpeed *= 0.001f;
        DecreaseSpeed *= 0.001f;
    }

    private void OnEnable()
    {
        //�L��������鎞�ɏ�����������
        StartAnimation = true;
        BossHPGauge.fillAmount = 0f;
        BossHPBackGauge.fillAmount = 0f;
        StartCoroutine(SetFullHP());
    }

    IEnumerator SetFullHP()
    {
        //�������A�j���[�V����
        while (BossHPGauge.fillAmount < 1)
        {
            BossHPGauge.fillAmount += FillSpeed;
            BossHPBackGauge.fillAmount += FillSpeed;
            yield return new WaitForEndOfFrame();
        }
        //������
        BossHP = BossFullHP = GetComponentInParent<Enemy>(true).GetEnemyFullHP();
        //BossHP = GetComponentInParent<Enemy>(true).GetEnemyHP();
        StartAnimation = false;
        ReductionHP();
    }

    //HP�������ɌĂԃR�}���h
    public void ReductionHP()
    {
        if (!StartAnimation)
        {
            var currentHP = GetComponentInParent<Enemy>().GetEnemyHP();
            //�E�����u�Ԃ�HP�Q�[�W������
            if(currentHP <= 0)
            {
                gameObject.SetActive(false);
                return;
            }
            //HP���ς�����ꍇ�͍���HP��\�����āA�����A�j���[�V�����𗬂��B
            if (currentHP != BossHP)
            {
                //����HP���X�V����
                BossHP = currentHP;
                BossHPGauge.fillAmount = BossHP / BossFullHP;
                //HP�����A�j���[�V�����𗬂�
                if (!HPdecreasing)
                {
                    if(hpDecreasingCoro!=null)StopCoroutine(hpDecreasingCoro);
                    hpDecreasingCoro = StartCoroutine(DecreaseHP());
                }
            }
        }
    }

    //HP�̔w�i�����̃A�j���[�V����
    IEnumerator DecreaseHP()
    {
        yield return new WaitForSeconds(0.7f);
        HPdecreasing = true;
        while(BossHPBackGauge.fillAmount > BossHPGauge.fillAmount)
        {
            if((BossHPBackGauge.fillAmount - DecreaseSpeed) > BossHPGauge.fillAmount)
            {
                BossHPBackGauge.fillAmount -= DecreaseSpeed;
            }
            else
            {
                BossHPBackGauge.fillAmount = BossHPGauge.fillAmount;
            }
            yield return new WaitForEndOfFrame();
        }
        HPdecreasing = false;
    }
}
