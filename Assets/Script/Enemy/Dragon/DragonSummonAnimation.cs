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
        gameObject.GetComponent<Dragon>().enabled = true;
        GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(true);
        IsAnimation = false;
        animator.SetBool("IsAnimation", IsAnimation);
        //Debug.Log("++++++++++++++++++++++++++++++++++++++++++");
        //�ǂ̃`�F�b�N
        if(WallCheck!=null)WallCheck.SetActive(true);
        gameObject.GetComponent<DragonSummonAnimation>().enabled = false;
    }

    //�{�X�o��A�j���[�V����
    IEnumerator BossSummonAnim1()
    {
        AnimController = 1;
        yield return new WaitForSeconds(1);
        SoundManager.Instance.PlaySE(SESoundData.SE.DragonRoar);
        shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        yield return new WaitForSeconds(2);
        HPBar.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        AnimationPlayed();

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
