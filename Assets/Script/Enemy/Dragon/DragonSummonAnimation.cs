using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSummonAnimation : MonoBehaviour
{
    //—h‚êŠÖ˜A
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("—h‚êŠÔ")]
        public float Duration;
        [Tooltip("—h‚ê‚Ì‹­‚³")]
        public float Strength;
    }

    [SerializeField]
    [Header("‰æ–Ê—h‚ê‚ÉŠÖ‚·‚é")]
    public ShakeInfo _shakeInfo;
    CameraShake shake;



    //AnimationŠÖ˜A
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

    //“®‰æ‚ªI‚í‚Á‚½‚ç•’Ê‚Ì“GScript‚ÉˆÚ‚·B
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
