using UnityEngine;

public class EnemyBirdAttackCheckArea : MonoBehaviour
{
    private void Update()
    {
        if (transform.GetComponentInParent<Enemy_Bird>().GetIsBlowing())
            gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (transform.GetComponentInParent<Enemy_Bird>().PlayerInAttackArea())
            {
                gameObject.SetActive(false);
            }
        }
    }
}
