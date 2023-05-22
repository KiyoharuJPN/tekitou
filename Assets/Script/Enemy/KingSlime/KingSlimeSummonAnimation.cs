using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlimeSummonAnimation : MonoBehaviour
{
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("—h‚êŽžŠÔ")]
        public float Duration;
        [Tooltip("—h‚ê‚Ì‹­‚³")]
        public float Strength;
    }

    [SerializeField]
    [Header("‰æ–Ê—h‚ê‚ÉŠÖ‚·‚é")]
    public ShakeInfo _shakeInfo;
    CameraShake shake;

    Animator animator;
    bool IsAnimation = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    private void Update()
    {



        animator.SetBool("IsAnimation", IsAnimation);
    }


    void AnimationPlayed()
    {
        gameObject.GetComponent<KingSlimeSummonAnimation>().enabled = false;
        gameObject.GetComponent<KingSlime>().enabled = true;
        GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(true);
        IsAnimation = false;
    }
}
