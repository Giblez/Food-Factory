using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;

public class ConveyerBeltCorner : ConveyerBelt
{    
    public int collisionRightVectMultiplier;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        /* Call parent's Start */
        base.Start();
    }

    protected override Vector3Int DetermineRightVector(bool isCollision)
    {
        Vector3Int retVect = new Vector3Int(0, 0, 0);

        /* If this was not the collided object */
        if (isCollision == false)
        {
            if (Mathf.Approximately(Mathf.Abs(gameObject.transform.right.x), 1.0f))
            {
                retVect.x = (int)gameObject.transform.right.x * rightVectMultiplier;
            }
            if (Mathf.Approximately(Mathf.Abs(gameObject.transform.right.y), 1.0f))
            {
                retVect.y = (int)gameObject.transform.right.y * rightVectMultiplier;
            }
        }

        /* If this is the collided object */
        else
        {
            retVect.x = (int)gameObject.transform.right.y * rightVectMultiplier * collisionRightVectMultiplier;
            retVect.y = (int)gameObject.transform.right.x * rightVectMultiplier * collisionRightVectMultiplier * -1;
        }
        return retVect;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
