using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlimeSummonAnimation : MonoBehaviour
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

    public GameObject WallCheck;

    //Animation関連
    Animator animator;
    int animationControler;
    bool IsAnimation = true, StageCheck = false, anim3 =true,anim4 = true;
    Rigidbody2D enemyRb;

    [Header("HPGaugeの表示")]
    [SerializeField]
    GameObject HPBar;

    private void Start()
    {
        animationControler = 0;
        animator = GetComponent<Animator>();
        enemyRb = GetComponent<Rigidbody2D>();
        if(shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    private void Update()
    {
        //Debug.Log(animationControler);
        animator.SetInteger("AnimationControler", animationControler);
        animator.SetBool("IsAnimation", IsAnimation);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetComponent<KingSlimeSummonAnimation>().enabled)
        {
            if (collision.gameObject.CompareTag("Stage") && !StageCheck)
            {
                StageCheck = true;
                Debug.Log("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                animationControler++;
                SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
                shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength,true,true);
            }
            if (animationControler == 1 && anim3)
            {
                anim3 = false;
                Invoke("Animation_3", 1.1f);
            }
            if (animationControler == 4 && anim4)
            {
                anim4 = false;
                Invoke("AnimationPlayed", 0.2f);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (GetComponent<KingSlimeSummonAnimation>().enabled)
        {
            StageCheck = false;
        }
    }

    void Animation_3()
    {
        animationControler++;
        StartCoroutine(Animation_4());
    }

    IEnumerator Animation_4()
    {
        yield return new WaitForSeconds(0.1f);
        enemyRb.AddForce(new Vector2(0, 30), ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        enemyRb.AddForce(new Vector2(0, -30), ForceMode2D.Impulse);
        animationControler++;
        yield return new WaitForSeconds(0.2f);
        enemyRb.AddForce(new Vector2(0, -15), ForceMode2D.Impulse);
    }

    //動画が終わったら普通の敵Scriptに移す。
    void AnimationPlayed()
    {
        HPBar.SetActive(true);
        gameObject.GetComponent<KingSlime>().enabled = true;
        GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(true);
        IsAnimation = false;
        animator.SetBool("IsAnimation", IsAnimation);
        //Debug.Log("++++++++++++++++++++++++++++++++++++++++++");
        WallCheck.SetActive(true);
        gameObject.GetComponent<KingSlimeSummonAnimation>().enabled = false;
    }

    private void FixedUpdate()
    {
        Gravity();
    }
    void Gravity()
    {
        enemyRb.AddForce(new Vector2(0, -10));
    }


}
