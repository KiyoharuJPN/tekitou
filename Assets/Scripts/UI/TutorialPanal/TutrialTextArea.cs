using UnityEngine;

public class TutrialTextArea : MonoBehaviour
{
    [SerializeField,Header("ÉGÉäÉAî‘çÜ")]
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
