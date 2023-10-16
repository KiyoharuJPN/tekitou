using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetComboResult : MonoBehaviour
{
    [SerializeField]
    Image comboText;
    [SerializeField]
    TextMeshProUGUI comboCountText;
    [SerializeField, Header("�\������")]
    float displayTime = 2f;
    [SerializeField, Header("���łɗv���鎞��")]
    float destoryTime = 0.1f;

    public void ComboSet(Sprite comboTextImage, int comboCount)
    {
        comboText.sprite = comboTextImage;

        string SpriteText = comboCount.ToString();
        comboCountText.text = "";
        foreach (var i in SpriteText)
        {
            int count = int.Parse(i.ToString());
            comboCountText.text += "<sprite=" + CountCheck(count) + ">";
        }

        //�R���{�����m�F���AspriteID��ύX���郍�[�J���֐�
        int CountCheck(int count)
        {
            if (comboCount >= 10 && comboCount < 20)
            {
                count += 10;
            }
            else if (comboCount >= 20 && comboCount < 50)
            {
                count += 20;
            }
            else if (comboCount >= 50 && comboCount < 100)
            {
                count += 30;
            }
            else if (comboCount >= 100)
            {
                count += 40;
            }
            return count;
        }

        StartCoroutine(ComboResultDisplay());   
    }

    IEnumerator ComboResultDisplay()
    {
        bool canText = false;
        bool canComboCount = false;
        this.gameObject.GetComponent<Canvas>().enabled = true;

        yield return new WaitForSeconds(displayTime);

        DOTween.ToAlpha(
            () => comboText.color,
            color => comboText.color = color,
            0f,
            displayTime).
            OnComplete(() => { canText = true; });
        DOTween.ToAlpha(
            () => comboCountText.color,
            color => comboCountText.color = color,
            0f,
            displayTime).
            OnComplete(() => { canComboCount = true; });

        while (true)
        {
            if (canComboCount && canText)
            {
                Destroy(this.gameObject);
            }
            yield return null;
        }
    }
}
