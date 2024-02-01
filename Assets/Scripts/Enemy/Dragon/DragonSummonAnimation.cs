using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSummonAnimation : MonoBehaviour
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

    public GameObject WallCheck;

    //Animation�֘A
    Animator animator;
    int AnimController = 0;
    bool IsAnimation = true;//, StageCheck = false, anim3 = true, anim4 = true;
    Rigidbody2D enemyRb;

    [Header("HPGauge�̕\��")]
    [SerializeField]
    GameObject HPBar;

    private void Start()
    {
        animator = GetComponent<Animator>();
        enemyRb = GetComponent<Rigidbody2D>();
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    private void Update()
    {
        animator.SetInteger("AnimController", AnimController);
        animator.SetBool("IsAnimation", IsAnimation);
    }

    //���悪�I������畁�ʂ̓GScript�Ɉڂ��B
    void AnimationPlayed()
    {
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<Dragon>().enabled = true;
        GameManager.Instance.canPause = true;
        GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(true);
        IsAnimation = false;
        animator.SetBool("IsAnimation", IsAnimation);
        //�ǂ̃`�F�b�N
        if(WallCheck!=null)WallCheck.SetActive(true);
        gameObject.GetComponent<DragonSummonAnimation>().enabled = false;
        if (GameObject.Find("Hero").CompareTag("InvinciblePlayer"))
        {
            SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Invincibility, BGMSoundData.BGM.none);
        }
        GameManager.Instance.StartRecordTime();
    }

    //�{�X�o��A�j���[�V����
    IEnumerator BossSummonAnim1()
    {

        AnimController = 1;
        yield return new WaitForSeconds(1);
        SoundManager.Instance.PlaySE(SESoundData.SE.DragonRoar);
        shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        HPBar.SetActive(true);
        for (int i = 1; i <= _entryEffect.frequency; i++)
        {
            Invoke("BossEntryEffect", (_entryEffect.interval * i));
        }
        yield return new WaitForSeconds(2.1f);
        AnimationPlayed();

    }
    protected void BossEntryEffect()
    {
        Vector2 sumPos = new Vector2(gameObject.transform.position.x - 4, gameObject.transform.position.y - 0.3f); ;
        Instantiate(_entryEffect.entryEffectObject, sumPos, Quaternion.identity.normalized);
    }


    //������������
    IEnumerator BossSummon()
    {
        yield return new WaitForSeconds(waitSecond);
        StartCoroutine(BossSummonAnim1());
    }

    //�v���C���[���~�߂鏈��
    private void OnBecameVisible()
    {
        if(GetComponent<DragonSummonAnimation>().enabled == true)
        {
            if (summon)
            {
                summon = false;
                GameManager.Instance.canPause = false;
                StartCoroutine(BossSummon());
                GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(false);
            }
        }
    }
    //�d�͕t��
    private void FixedUpdate()
    {
        Gravity();
    }
    void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -5));
    }

}
