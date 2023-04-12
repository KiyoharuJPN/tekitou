using UnityEngine;

public class PlatformRuleMove : MonoBehaviour
{
    public float distance = 5, speed =0.01f;
    protected float checkDistance = 0;
    protected bool moveDirection = true;
    protected virtual void Update()
    {
        checkDistance += speed;
        if(checkDistance < distance)
        {
            if (moveDirection)
            {
                transform.position = new Vector2(transform.position.x + speed, transform.position.y);
            }else if (!moveDirection)
            {
                transform.position = new Vector2(transform.position.x - speed, transform.position.y);
            }
        }
        else
        {

            if (moveDirection && checkDistance != 0)
            {
                checkDistance = 0;
                moveDirection = false;
            }
            if (!moveDirection && checkDistance != 0)
            {
                checkDistance = 0;
                moveDirection = true;
            }
        }
    }

}
