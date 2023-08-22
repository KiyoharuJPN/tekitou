using UnityEngine;

public class TutrialTextArea : MonoBehaviour
{
    [SerializeField,Header("エリア番号")]
    public int num;

    public TutorialText tutorialText;

    private void Start()
    {
        tutorialText = GameObject.Find("UI").GetComponentInChildren<TutorialText>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        tutorialText.TutorialAreaEnter(num);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        tutorialText.TutorialAreaExit(num);
    }
}
