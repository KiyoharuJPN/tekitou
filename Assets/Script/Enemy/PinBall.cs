using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinBall : MonoBehaviour
{
    public static void _PinBall(Rigidbody2D rb, float speed, int num)
    {
        rb.velocity = new Vector2(speed, speed);
    }
}
