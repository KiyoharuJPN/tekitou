using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class TutorialText : MonoBehaviour
{
    [SerializeField, Header("������1�̃e�L�X�g�{�b�N�X")]
    TextMeshProUGUI textBox_1;
    [SerializeField, Header("������2�̃e�L�X�g�{�b�N�X")]
    TextMeshProUGUI textBox_2;

    [SerializeField, Header("�w�i")]
    GameObject backGround;

    [System.Serializable]
    struct TutorialTextContent
    {
        [SerializeField, Header("������1")]
        public string text_1;
        [SerializeField, Header("������2")]
        public string text_2;
        [SerializeField, Header("�A���ŕ\��������p�l���ԍ��O")]
        public int series_1;
        [SerializeField, Header("�A���ŕ\��������p�l���ԍ���")]
        public int series_2;
    }

    [SerializeField, Header("�e�L�X�g�R���e���c")]
    TutorialTextContent[] tutorialContent;


    //���O�̃e�L�X�g�R���e���c�ԍ�
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
        if (tutorialContent[num].text_1 != "")
        {
            textBox_1.text = tutorialContent[num].text_1;
        }
        else textBox_1.text = "";
        if (tutorialContent[num].text_2 != "")
        {
            textBox_2.text = tutorialContent[num].text_2;
        }
        else textBox_2.text = "";
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
            1f
            );
    }
    void TextDown()
    {
        backGround.transform.DOMoveY(
            -200f,
            1f
            );
    }

}
