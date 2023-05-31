using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxBackground : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    bool isInitialized = false;

    //あらかじめ各画像を紐付けしておく。
    [Header("背景画像 (0が最奥、順に手前)")]
    [SerializeField]
    Sprite[] backgroundSprites;

    [Header("背景画像のオフセット (ズラす値)")]
    [SerializeField]
    Vector2 backgroundOffsets = new Vector2(1500, 0);

    [Header("背景画像のサイズ")]
    [SerializeField]
    Vector2 backgroundSpriteSizes = new Vector2(1500, 2000);

    [Header("背景画像のスクロール率 (0が最奥、順に手前)")]
    [SerializeField]
    float[] scrollRates;

    [Header("スクロール時間")]
    [Range(0.1f, 3.0f)]
    [SerializeField]
    float scrollDuration = 1.0f;

    float smoothTime;

    [Header("スクロール速度の上限 (多分deltaTimeが掛かるので大きめに指定)")]
    [Range(500.0f, 10000.0f)]
    [SerializeField]
    float scrollSpeedMax = 1000.0f;

    //各背景画像のRectTransform。
    [HideInInspector]
    [SerializeField]
    RectTransform[] backgroundsRt;

    //背景画像数。
    [SerializeField]
    int backgroundMax;

    //各背景画像がスクロールした量。
    [HideInInspector]
    [SerializeField]
    float[] backgroundScrollValues;

    //RectMask2Dを有効にした状態で実行すると、スクロールしても画面外に設置した画像が非表示になる仕様っぽいので、実行時に有効化している。
    [HideInInspector]
    [SerializeField]
    RectMask2D parallaxBackgroundRectMask2D;

    //スクロール経過時間。
    float scrollElapsedTime;

    //スクロール加速度。SmoothDampに必要。
    [HideInInspector]
    [SerializeField]
    Vector2[] scrollVelocities;

    //コルーチンの管理に使用。
    Coroutine scroll;

    //前にスクロールが呼ばれた時のプレイヤーの位置。
    Vector3 previousPlayerPosition = Vector3.zero;

    //一時的に使用。
    Vector2 tempBackgroundsPosition;

    void Awake()
    {
        parallaxBackgroundRectMask2D.enabled = true;

        //SmoothDampのsmoothTimeと、スクロールの長さが厳密には違うので、一回り小さく計算しておく。
        smoothTime = scrollDuration * 0.85f;
    }


    //背景画像をスクロールしたい場合にコレを呼ぶ。引数にはプレイヤーの位置を渡す(位置差でなく)。
    public void StartScroll(Vector3 playerPosition)
    {

        //1画像分進んだ時、スクロールが繋がるように良い感じに戻している。
        for (int i = 0; i < backgroundMax; i++)
        {
            backgroundScrollValues[i] -= (playerPosition.x - previousPlayerPosition.x) * scrollRates[i];

            if (backgroundSpriteSizes.x < backgroundsRt[i].anchoredPosition.x)
            {
                backgroundScrollValues[i] -= backgroundSpriteSizes.x;
                tempBackgroundsPosition.Set(backgroundSpriteSizes.x, 0);
                backgroundsRt[i].anchoredPosition -= tempBackgroundsPosition;
            }
            else if (backgroundsRt[i].anchoredPosition.x < -backgroundSpriteSizes.x)
            {
                backgroundScrollValues[i] += backgroundSpriteSizes.x;
                tempBackgroundsPosition.Set(backgroundSpriteSizes.x, 0);
                backgroundsRt[i].anchoredPosition += tempBackgroundsPosition;
            }
        }


        //多重実行防止。
        if (scroll != null)
        {
            StopCoroutine(scroll);
        }

        scroll = StartCoroutine(Scroll());


        previousPlayerPosition = playerPosition;
    }


    IEnumerator Scroll()
    {
        scrollElapsedTime = 0;
        while (true)
        {
            scrollElapsedTime += Time.deltaTime;


            for (int i = 0; i < backgroundMax; i++)
            {
                tempBackgroundsPosition.Set(backgroundScrollValues[i], backgroundOffsets.y);
                backgroundsRt[i].anchoredPosition = Vector2.SmoothDamp(backgroundsRt[i].anchoredPosition, tempBackgroundsPosition, ref scrollVelocities[i], smoothTime, scrollSpeedMax);
            }


            if (scrollDuration <= scrollElapsedTime)
            {
                //SmoothDampはVelocityの値を参考にして現在の速度を出す為、初期化しておかないと次回実行時に動きが残る。
                for (int i = 0; i < backgroundMax; i++)
                {
                    scrollVelocities[i] = Vector2.zero;
                }

                scroll = null;
                yield break;
            }

            yield return null;
        }
    }
}