using UnityEngine;

public class PlatformRuleMoveUD : PlatformRuleMove
{
    protected override void Update()
    {
        checkDistance += speed;
        if (checkDistance < distance)
        {
            if (moveDirection)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y + speed);
            }
            else if (!moveDirection)
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - speed);
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
