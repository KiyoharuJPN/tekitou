using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlimeSummonAnimation : MonoBehaviour
{
    //SEä÷òA
    //Ç‹ÇæÇ»Ç¢
    
    //óhÇÍä÷òA
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("óhÇÍéûä‘")]
        public float Duration;
        [Tooltip("óhÇÍÇÃã≠Ç≥")]
        public float Strength;
    }

    [SerializeField]
    [Header("âÊñ óhÇÍÇ…ä÷Ç∑ÇÈ")]
    public ShakeInfo _shakeInfo;
    CameraShake shake;

    //Animationä÷òA
    Animator animator;
    int animationControler;
    bool IsAnimation = true;
    Rigidbody2D enemyRb;

    private void Start()
    {
        animationControler = 0;
        animator = GetComponent<Animator>();
        enemyRb = GetComponent<Rigidbody2D>();
        shake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    private void Update()
    {
        Debug.Log(animationControler);
        animator.SetInteger("AnimationControler", animationControler);
        animator.SetBool("IsAnimation", IsAnimation);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
        {
            animationControler++;
            SoundManager.Instance.PlaySE(SESoundData.SE.KingSlimeLanding);
        }
        if(animationControler == 1)
        {
            Invoke("Animation_3", 1.1f);
        }
        if(animationControler == 4)
        {
            Invoke("AnimationPlayed", 0.2f);
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

    //ìÆâÊÇ™èIÇÌÇ¡ÇΩÇÁïÅí ÇÃìGScriptÇ…à⁄Ç∑ÅB
    void AnimationPlayed()
    {
        gameObject.GetComponent<KingSlime>().enabled = true;
        GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(true);
        IsAnimation = false;
        animator.SetBool("IsAnimation", IsAnimation);
        Debug.Log("++++++++++++++++++++++++++++++++++++++++++");
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
