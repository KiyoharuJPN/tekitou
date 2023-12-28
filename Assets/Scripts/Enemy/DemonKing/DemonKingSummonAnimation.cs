using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DemonKingSummonAnimation : MonoBehaviour
{
    //�h��֘A
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("�h�ꎞ��")]
        public float Duration;
        [Tooltip("�h��̋���")]
        public float Strength;
    }

    [SerializeField]
    [Header("��ʗh��Ɋւ���")]
    public ShakeInfo _shakeInfo;
    CameraShake shake;

    //�G�t�F�N�g�֘A
    [System.Serializable]
    public struct EntryEffect
    {
        [Tooltip("�Đ���")]
        public int frequency;
        [Tooltip("�Đ��Ԋu")]
        public float interval;
        [Tooltip("�Đ��I�u�W�F�N�g")]
        public GameObject entryEffectObject;
    }
    [SerializeField]
    [Header("�G���g���G�t�F�N�g�Ɋւ���")]
    public EntryEffect _entryEffect = new EntryEffect { frequency = 4, interval = 0.2f };

    //�����֘A
    public float waitSecond;
    bool summon = true;

    //Animation�֘A
    public Animator animator, animatorL, animatorR;
    public RuntimeAnimatorController entryAnimationL , entryAnimationR;
    bool IsAnimation = true;//, StageCheck = false, anim3 = true, anim4 = true;
    
    [Header("HPGauge�̕\��")]
    [SerializeField]
    GameObject HPBar;

    private void Start()
    {
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }



    //���悪�I������畁�ʂ̓GScript�Ɉڂ��B
    protected void AnimationPlayed()
    {

        //gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<DemonKing>().enabled = true;
        GameManager.Instance.canPause = true;
        GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(true);
        //�A�j���[�V��������A�j���[�^�[�R���g���[���[�ɕς���C��
        IsAnimation = false;
        animator.SetBool("InAdanim", IsAnimation);
        //animatorL.SetBool("InAdanim", IsAnimation);
        //animatorR.SetBool("InAdanim", IsAnimation);
        //Debug.Log("++++++++++++++++++++++++++++++++++++++++++");
        gameObject.GetComponent<DemonKingSummonAnimation>().enabled = false;
        if (GameObject.Find("Hero").CompareTag("InvinciblePlayer"))
        {
            SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Invincibility, BGMSoundData.BGM.none);
        }
    }

    //�{�X�o��A�j���[�V����
    protected void BossSummonAnim1()
    {
        
        SoundManager.Instance.PlaySE(SESoundData.SE.DemonkingShout);
        shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        //Debug.Log(shake);
        HPBar.SetActive(true);
        for (int i = 1; i <= _entryEffect.frequency; i++)
        {
            Invoke("BossEntryEffect", (_entryEffect.interval*i));
        }

    }
    protected void BossEntryEffect()
    {
        Instantiate(_entryEffect.entryEffectObject, gameObject.transform.position, Quaternion.identity.normalized);
    }

    //������������
    IEnumerator BossSummon()
    {
        yield return new WaitForSeconds(waitSecond);
        animator.speed = animatorL.speed = animatorR.speed = 1;
    }




    //�v���C���[���~�߂鏈��
    private void OnBecameVisible()
    {
        if (GetComponent<DemonKingSummonAnimation>().enabled == true)
        {

            if (summon)
            {
                summon = false;

                animator.SetBool("InAdanim", true); animator.speed = 0;
                animatorL.runtimeAnimatorController = entryAnimationL; animatorL.speed = 0;
                animatorR.runtimeAnimatorController = entryAnimationR; animatorR.speed = 0;
                GameManager.Instance.canPause = false;
                StartCoroutine(BossSummon());
                GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(false);
            }
        }
    }

}
