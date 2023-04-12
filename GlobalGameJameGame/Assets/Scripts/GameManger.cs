using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    [SerializeField]
    PlayerController player;
    [SerializeField]
    BoxCollider2D killBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //called when player falls below ground, resets their position
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
            player.rigidbody.position = player.RespawnPoint;
        
    }
}
