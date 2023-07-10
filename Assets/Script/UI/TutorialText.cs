using UnityEngine;

public class TutorialText : MonoBehaviour
{
    SpriteRenderer sprite;
    public int objId;

    Animator animator;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        animator = GetComponent<Animator>();
        
        animator.enabled = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        sprite.enabled = true;
        if(objId != 6)
        {
            animator.enabled = true;
            animator.SetInteger("ObjID", objId);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        sprite.enabled = false;
        animator.enabled = false;
    }
}
