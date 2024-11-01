using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;

public class ConveyerBeltCorner : ConveyerBelt
{    
    public int collisionupVectMultiplier;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        /* Call parent's Start */
        base.Start();
    }

    protected override Vector3Int DetermineUpVector(bool isCollision)
    {
        Vector3Int upVect = new Vector3Int(0, 0, 0);

        /* If this was not the collided object */
        if (isCollision == false)
        {
            if (Mathf.Approximately(Mathf.Abs(gameObject.transform.up.x), 1.0f))
            {
                upVect.x = (int)gameObject.transform.up.x * upVectMultiplier;
            }
            if (Mathf.Approximately(Mathf.Abs(gameObject.transform.up.y), 1.0f))
            {
                upVect.y = (int)gameObject.transform.up.y * upVectMultiplier;
            }
        }

        /* If this is the collided object */
        else
        {
            upVect.x = (int)gameObject.transform.up.y * upVectMultiplier * collisionupVectMultiplier;
            upVect.y = (int)gameObject.transform.up.x * upVectMultiplier * collisionupVectMultiplier * -1;
        }
        return upVect;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
