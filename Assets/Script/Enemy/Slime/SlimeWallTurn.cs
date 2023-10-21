using UnityEngine;

public class SlimeWallTurn : MonoBehaviour
{
    private void Update()
    {
        if (transform.GetComponentInParent<Enemy>().GetIsBlowing())
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
            GetComponentInParent<Enemy>().TurnAround();
        Debug.Log("is wall");
    }
    
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Stage"))
    //        GetComponentInParent<Slime>().TurnAround();
    //}
}
