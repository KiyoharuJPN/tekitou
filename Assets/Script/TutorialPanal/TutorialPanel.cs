using Unity.VisualScripting;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField]
    TutorialScene manager;

    [SerializeField]//�N���A����Ɏg�p����G
    GameObject[] enemys;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            manager.TutorialStep();
            this.enabled = false;
            if(enemys.Length > 0)
            {
                foreach (GameObject enemy in enemys) 
                {
                    manager.enemylist.Add(enemy);
                }
            }
        }
    }
}
