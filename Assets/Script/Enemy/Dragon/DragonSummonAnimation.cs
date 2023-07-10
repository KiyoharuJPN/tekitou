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

    //召喚関連
    public float waitSecond;
    bool summon = true;

    public GameObject WallCheck;

    //Animation関連
    Animator animator;
    int AnimController = 0;
    bool IsAnimation = true;//, StageCheck = false, anim3 = true, anim4 = true;
    Rigidbody2D enemyRb;

    [Header("HPGaugeの表示")]
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

    //動画が終わったら普通の敵Scriptに移す。
    void AnimationPlayed()
    {
        gameObject.GetComponent<Dragon>().enabled = true;
        GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(true);
        IsAnimation = false;
        animator.SetBool("IsAnimation", IsAnimation);
        //Debug.Log("++++++++++++++++++++++++++++++++++++++++++");
        //壁のチェック
        if(WallCheck!=null)WallCheck.SetActive(true);
        gameObject.GetComponent<DragonSummonAnimation>().enabled = false;
    }

    //ボス登場アニメーション
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


    //初見召喚部分
    IEnumerator BossSummon()
    {
        yield return new WaitForSeconds(waitSecond);
        StartCoroutine(BossSummonAnim1());
    }

    //プレイヤーを止める処理
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
    //重力付け
    private void FixedUpdate()
    {
        Gravity();
    }
    void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -5));
    }

}
