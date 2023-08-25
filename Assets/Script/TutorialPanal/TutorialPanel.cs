using Unity.VisualScripting;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]
    TutorialScene manager;

    bool farstAreaIn = true;

    //プレイヤーが範囲内にいるか？
    public bool isPlayer = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (farstAreaIn)
        {
            farstAreaIn = false;
            manager.TutorialStep();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isPlayer = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayer = false;
    }
}
