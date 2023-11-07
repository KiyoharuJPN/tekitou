using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GetBuffUI : MonoBehaviour
{
    [SerializeField]
    Image buffImage;
    [SerializeField, Header("表示時間")]
    float displayTime = 2f;
    [SerializeField, Header("消滅に要する時間")]
    float destoryTime = 0.1f;

    public void BuffImageSet(Sprite buffImage)
    {
        this.buffImage.sprite = buffImage;

        StartCoroutine(BuffUIDisplay());
    }

    IEnumerator BuffUIDisplay()
    {
        bool getBuffUI = false;
        this.gameObject.GetComponent<Canvas>().enabled = true;

        yield return new WaitForSeconds(displayTime);
        DOTween.ToAlpha(
            () => buffImage.color,
            color => buffImage.color = color,
            0f,
            displayTime).
            OnComplete(() => { getBuffUI = true; });

        while (true)
        {
            if (getBuffUI)
            {
                Destroy(this.gameObject);
            }
            yield return null;
        }
    }
}
