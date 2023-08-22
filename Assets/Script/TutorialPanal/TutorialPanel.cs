using Unity.VisualScripting;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]
    TutorialScene manager;

    [SerializeField]//クリア判定に使用する敵
    GameObject[] enemys;

    //プレイヤーが範囲内にいるか？
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
