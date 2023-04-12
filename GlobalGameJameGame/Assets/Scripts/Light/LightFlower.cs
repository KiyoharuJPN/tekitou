using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlower : MonoBehaviour
{
    public List<GameObject> savePoints;
    public float maxDistance;
    private GameObject currentPoint;
    private SpriteRenderer spriteRenderer;
    private bool pointInRange;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPoint = float.MaxValue;
        //finds if any save points (Light flowers are within range and then gets the closest one
        foreach (GameObject point in savePoints)
        {
            
            if (Vector2.Distance(this.transform.position, point.transform.position) < maxDistance)
            {
                pointInRange = true;
                if (Vector2.Distance(this.transform.position, point.transform.position) < distanceToPoint)
                {
                    distanceToPoint = Vector2.Distance(this.transform.position, point.transform.position);
                }
            }
        }

        if(pointInRange)
        {
            //color gets stronger as the object approaches the savepoint
            spriteRenderer.color = new Color(1 / distanceToPoint, 0, 0);
        }
        else
        {
            //if there are no points in range sets it to black
            spriteRenderer.color = Color.black;
        }
       

    }
}
