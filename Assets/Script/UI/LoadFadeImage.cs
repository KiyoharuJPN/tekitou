using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadFadeImage : MonoBehaviour
{
    [Header("�ŏ�����t�F�[�h�C�����������Ă��邩�ǂ���")] public bool firstFadeInComp;

    private Image img = null;
    private int frameCount = 0;
    private float timer = 0.0f;
    private bool fadeIn = false;
    private bool fadeOut = false;
    private bool compFadeIn = false;
    public bool compFadeOut = false;

    [SerializeField]
    float fadeTime = 1f;

    /// <summary>
    /// �t�F�[�h�C�����J�n����
    /// </summary>
    public void StartFadeIn()
    {
        if (fadeIn || fadeOut)
        {
            return;
        }
        fadeIn = true;
        compFadeIn = false;
        timer = 0.0f;
        img.color = new Color(0, 0, 0, 1);
        img.raycastTarget = true;
    }

    /// <summary>
    /// �t�F�[�h�C���������������ǂ���
    /// </summary>
    /// <returns></returns>
    public bool IsFadeInComplete()
    {
        return compFadeIn;
    }

    /// <summary>
    /// �t�F�[�h�A�E�g���J�n����
    /// </summary>
    public void StartFadeOut()
    {
        if (fadeIn || fadeOut)
        {
            return;
        }
        fadeOut = true;
        compFadeOut = false;
        timer = 0.0f;
        img.color = new Color(0, 0, 0, 0);
        img.raycastTarget = true;
    }

    /// <summary>
    /// �t�F�[�h�A�E�g�������������ǂ���
    /// </summary>
    /// <returns></returns>
    public bool IsFadeOutComplete()
    {
        return compFadeOut;
    }

    void Start()
    {
        img = GetComponent<Image>();
        if (firstFadeInComp)
        {
            FadeInComplete();
        }
        else
        {
            StartFadeIn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (frameCount > 2)
        {
            if (fadeIn)
            {
                FadeInUpdate();
            }
            else if (fadeOut)
            {
                FadeOutUpdate();
            }
        }
        ++frameCount;
    }

    //�t�F�[�h�C����
    private void FadeInUpdate()
    {
        //�t�F�[�h��
        if (timer < fadeTime)
        {
            img.color = new Color(0, 0, 0, 1 - timer);
        }
        //�t�F�[�h�����������Ƃ�
        else
        {
            FadeInComplete();
        }
        timer += Time.deltaTime;
    }
    //�t�F�[�h�A�E�g��
    private void FadeOutUpdate()
    {
        if (timer < fadeTime)
        {
            img.color = new Color(0, 0, 0, timer);
        }
        else
        {
            FadeOutComplete();
        }
        timer += Time.deltaTime;
    }

    //�t�F�[�h�C������
    private void FadeInComplete()
    {
        img.color = new Color(0, 0, 0, 0);
        img.raycastTarget = false;
        timer = 0f;
        fadeIn = false;
        compFadeIn = true;
    }

    //�t�F�[�h�A�E�g����
    private void FadeOutComplete()
    {
        img.color = new Color(0, 0, 0, 1);
        img.raycastTarget = false;
        timer = 0f;
        fadeOut = false;
        compFadeOut = true;
    }
}
