using Unity.VisualScripting;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]
    TutorialScene manager;

    [SerializeField]//�N���A����Ɏg�p����G
    GameObject[] enemys;

    //�v���C���[���͈͓��ɂ��邩�H
    public bool isPlayer = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        isPlayer = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayer = false;
    }
}
