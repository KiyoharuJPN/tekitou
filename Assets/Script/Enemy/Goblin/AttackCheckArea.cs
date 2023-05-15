using UnityEngine;

public class AttackCheckArea : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("111111111111111111111111111111111111");
        if (collision.transform.CompareTag("Player")){
            transform.GetComponentInParent<Goblin>().PlayerInAttackArea();
        }
    }
}
