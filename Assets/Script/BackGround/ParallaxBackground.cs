using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxBackground : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    bool isInitialized = false;

    //���炩���ߊe�摜��R�t�����Ă����B
    [Header("�w�i�摜 (0���ŉ��A���Ɏ�O)")]
    [SerializeField]
    Sprite[] backgroundSprites;

    [Header("�w�i�摜�̃I�t�Z�b�g (�Y�����l)")]
    [SerializeField]
    Vector2 backgroundOffsets = new Vector2(1500, 0);

    [Header("�w�i�摜�̃T�C�Y")]
    [SerializeField]
    Vector2 backgroundSpriteSizes = new Vector2(1500, 2000);

    [Header("�w�i�摜�̃X�N���[���� (0���ŉ��A���Ɏ�O)")]
    [SerializeField]
    float[] scrollRates;

    [Header("�X�N���[������")]
    [Range(0.1f, 3.0f)]
    [SerializeField]
    float scrollDuration = 1.0f;

    float smoothTime;

    [Header("�X�N���[�����x�̏�� (����deltaTime���|����̂ő傫�߂Ɏw��)")]
    [Range(500.0f, 10000.0f)]
    [SerializeField]
    float scrollSpeedMax = 1000.0f;

    //�e�w�i�摜��RectTransform�B
    [HideInInspector]
    [SerializeField]
    RectTransform[] backgroundsRt;

    //�w�i�摜���B
    [SerializeField]
    int backgroundMax;

    //�e�w�i�摜���X�N���[�������ʁB
    [HideInInspector]
    [SerializeField]
    float[] backgroundScrollValues;

    //RectMask2D��L���ɂ�����ԂŎ��s����ƁA�X�N���[�����Ă���ʊO�ɐݒu�����摜����\���ɂȂ�d�l���ۂ��̂ŁA���s���ɗL�������Ă���B
    [HideInInspector]
    [SerializeField]
    RectMask2D parallaxBackgroundRectMask2D;

    //�X�N���[���o�ߎ��ԁB
    float scrollElapsedTime;

    //�X�N���[�������x�BSmoothDamp�ɕK�v�B
    [HideInInspector]
    [SerializeField]
    Vector2[] scrollVelocities;

    //�R���[�`���̊Ǘ��Ɏg�p�B
    Coroutine scroll;

    //�O�ɃX�N���[�����Ă΂ꂽ���̃v���C���[�̈ʒu�B
    Vector3 previousPlayerPosition = Vector3.zero;

    //�ꎞ�I�Ɏg�p�B
    Vector2 tempBackgroundsPosition;

    void Awake()
    {
        parallaxBackgroundRectMask2D.enabled = true;

        //SmoothDamp��smoothTime�ƁA�X�N���[���̒����������ɂ͈Ⴄ�̂ŁA���菬�����v�Z���Ă����B
        smoothTime = scrollDuration * 0.85f;
    }


    //�w�i�摜���X�N���[���������ꍇ�ɃR�����ĂԁB�����ɂ̓v���C���[�̈ʒu��n��(�ʒu���łȂ�)�B
    public void StartScroll(Vector3 playerPosition)
    {

        //1�摜���i�񂾎��A�X�N���[�����q����悤�ɗǂ������ɖ߂��Ă���B
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


        //���d���s�h�~�B
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
                //SmoothDamp��Velocity�̒l���Q�l�ɂ��Č��݂̑��x���o���ׁA���������Ă����Ȃ��Ǝ�����s���ɓ������c��B
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