using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformController : MonoBehaviour
{
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(player == null)
        {
            player = collision.gameObject.GetComponent<PlayerController>();
        }

        //allows the player to jump whenever they make contact with a platform
        if (player.CanJump == false)
        {
            player.CanJump = true;
        }
       
    }
}
