using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePointController : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    public AudioClip save;
   
    bool playerHasReached;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the player hasn't already made passed this checkpoint sets the respawn point to this position
        if (!playerHasReached)
        {
            EffectSoundSource.instance.PlaySE(save);
            player.RespawnPoint = this.transform.position;
        }
           
        
    }
}
