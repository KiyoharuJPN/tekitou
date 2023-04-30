using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverText : MonoBehaviour
{
    public float moveHeight = 10,speed = 0.2f;

    Vector2 startPos;
    float distance;
    bool toUp = true;

    private void Start()
    {
        distance = 0;
        startPos = this.transform.position;
    }
    private void Update()
    {
        if(this.transform.position.y >= startPos.y+(moveHeight / 2))
        {
            toUp = false;
        }else if(this.transform.position.y <= startPos.y-(moveHeight / 2))
        {
            toUp = true;
        }

        if (toUp)
        {
            //Debug.Log(distance);
            distance += speed;
            if (distance > moveHeight / 2) distance = moveHeight / 2;
            this.transform.position = new Vector2(startPos.x, startPos.y + distance);
        }
        else if(!toUp)
        {
            //Debug.Log(distance);
            distance -= speed;
            if (distance < -(moveHeight / 2)) distance = -(moveHeight / 2);
            this.transform.position = new Vector2(startPos.x, startPos.y + distance);
        }
    }
}
