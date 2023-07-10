using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonWallCheckArea : MonoBehaviour
{
    private void Update()
    {
        if (transform.GetComponentInParent<Enemy>().GetIsBlowing())
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("---------------------------------------------------------------------------");
        if (collision.CompareTag("Stage"))
            GetComponentInParent<Dragon>().TurnAround();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        if (collision.CompareTag("Stage") || GetComponentInParent<KingSlime>().GetSkillTurnAround())
        {
            GetComponentInParent<Dragon>().TurnAround();
        }
    }
}
