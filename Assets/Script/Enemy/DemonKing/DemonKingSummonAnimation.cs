using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DemonKingSummonAnimation : MonoBehaviour
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

    //Animation関連
    public Animator animator,animatorL,animatorR;
    bool IsAnimation = true;//, StageCheck = false, anim3 = true, anim4 = true;

    [Header("HPGaugeの表示")]
    [SerializeField]
    GameObject HPBar;

    private void Start()
    {
        if (shake == null) shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }



    //動画が終わったら普通の敵Scriptに移す。
    protected void AnimationPlayed()
    {
        GameManager.Instance.Result_Start(3);

        //ぜんためコメントアウト
        ////gameObject.GetComponent<BoxCollider2D>().enabled = true;
        //gameObject.GetComponent<DemonKing>().enabled = true;
        //GameManager.Instance.canPause = true;
        //GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(true);
        //IsAnimation = false;
        //animator.SetBool("InAdanim", IsAnimation);
        //animatorL.SetBool("InAdanim", IsAnimation);
        //animatorR.SetBool("InAdanim", IsAnimation);
        ////Debug.Log("++++++++++++++++++++++++++++++++++++++++++");
        //gameObject.GetComponent<DemonKingSummonAnimation>().enabled = false;
        //if (GameObject.Find("Hero").CompareTag("InvinciblePlayer"))
        //{
        //    SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Invincibility, BGMSoundData.BGM.none);
        //}
    }

    //ボス登場アニメーション
    protected void BossSummonAnim1()
    {

        SoundManager.Instance.PlaySE(SESoundData.SE.DragonRoar);
        shake.Shake(_shakeInfo.Duration, _shakeInfo.Strength, true, true);
        
        //ぜんためコメントアウト
        //HPBar.SetActive(true);

    }


    //初見召喚部分
    IEnumerator BossSummon()
    {
        yield return new WaitForSeconds(waitSecond);
        animator.speed = animatorL.speed = animatorR.speed = 1;
    }




    //プレイヤーを止める処理
    private void OnBecameVisible()
    {
        if (GetComponent<DemonKingSummonAnimation>().enabled == true)
        {
            
            if (summon)
            {
                summon = false;
                animator.SetBool("InAdanim", true); animator.speed = 0;
                animatorL.SetBool("InAdanim", true); animatorL.speed = 0;
                animatorR.SetBool("InAdanim", true); animatorR.speed = 0;
                GameManager.Instance.canPause = false;
                StartCoroutine(BossSummon());
                GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(false);
            }
        }
    }

}
