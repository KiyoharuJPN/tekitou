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



    //Animation�֘A
    Animator animator;
    int animationControler;
    bool IsAnimation = true;//, StageCheck = false, anim3 = true, anim4 = true;
    Rigidbody2D enemyRb;


    private void Start()
    {
        animationControler = 0;
        animator = GetComponent<Animator>();
        enemyRb = GetComponent<Rigidbody2D>();
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    private void Update()
    {
        Debug.Log(animationControler);
        animator.SetInteger("AnimationControler", animationControler);
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
        gameObject.GetComponent<DragonSummonAnimation>().enabled = false;
    }
}
