using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSummonAnimation : MonoBehaviour
{
    //揺れ関連
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("揺れ時間")]
        public float Duration;
        [Tooltip("揺れの強さ")]
        public float Strength;
    }

    [SerializeField]
    [Header("画面揺れに関する")]
    public ShakeInfo _shakeInfo;
    CameraShake shake;



    //Animation関連
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

    //動画が終わったら普通の敵Scriptに移す。
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
