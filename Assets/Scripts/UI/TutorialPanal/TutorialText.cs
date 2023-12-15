using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class TutorialText : MonoBehaviour
{
    [SerializeField, Header("説明文1のテキストボックス")]
    TextMeshProUGUI textBox_1;
    [SerializeField, Header("説明文2のテキストボックス")]
    TextMeshProUGUI textBox_2;

    [SerializeField, Header("背景")]
    GameObject backGround;

    [System.Serializable]
    struct TutorialTextContent
    {
        [SerializeField, Header("説明文1")]
        public string text_1;
        [SerializeField, Header("説明文2")]
        public string text_2;
    }

    [SerializeField, Header("テキストコンテンツ")]
    TutorialTextContent[] tutorialContent;


    //直前のテキストコンテンツ番号
    public int textNum;

    private void Start()
    {
        TutorialAreaEnter(0);
        TextPop();
    }

    public void TutorialAreaEnter(int num)
    {
        if(num == -10)
        {
            TextDown();
            return;
        }

        TextChenge(num);
        TextPopCheck(num);
    }

    public void TutorialAreaExit(int num)
    {
        textNum = num;
    }

    void TextChenge(int num)
    {
        textBox_1.text = tutorialContent[num].text_1;
        textBox_2.text = tutorialContent[num].text_2;
    }

    void TextPopCheck(int num)
    {
        if(textNum == num + 1 || textNum == num - 1)
        {
            return;
        }

        TextPop();
    }

    void TextPop()
    {
        backGround.transform.DOMoveY(
            0f,
            0.5f
            );
    }
    void TextDown()
    {
        backGround.transform.DOMoveY(
            -200f,
            0.5f
            );
    }

}
