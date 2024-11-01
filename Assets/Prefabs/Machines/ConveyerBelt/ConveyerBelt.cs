using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class ConveyerBelt : MachineBase
{
    /* Animation member variables */
    public Animator animator;
    private float perAlongSpline;
    private float perOffset;
    public int animKeys;
    public Vector3 beltMoveInc;
    public FoodBase foodOnBelt;
    public int upVectMultiplier;

    /* Spline member variable */
    public SplineContainer splineContainer;

    /* References to the objects connected to this conveyer belt */
    public PrefabBase startConnector;
    public PrefabBase endConnector;

    // Start is called before the first frame update
    protected override void Start()
    {
        /* Call parent's Start */
        base.Start();

        /* Set sorting layer */
        spriteRenderer.sortingLayerName = "Belt";

        /* Retrieve any needed components */
        splineContainer = gameObject.GetComponent<SplineContainer>();

        /* Retrieve and start our animation */
        animator = gameObject.GetComponent<Animator>();
        animator.Rebind();
        perAlongSpline = 0.0f;
        perOffset = 1.0f / animKeys;

        /* Add self to game controller conveyer belt list */
        gameController.conveyerBeltList.Add(this);
        Debug.Log("New Belt, Count: " + gameController.conveyerBeltList.Count);    
    }

    protected virtual Vector3Int DetermineUpVector(bool isCollision)
    {
        /* Determine the up vector to determine where our collided object might be */
        Vector3Int upVect = new Vector3Int(0, 0, 0);
        if (Mathf.Approximately(Mathf.Abs(gameObject.transform.up.x), 1.0f))
        {
            // TODO - am i forgetting mult by -1
            upVect.x = (int)gameObject.transform.up.x * upVectMultiplier;
        }
        if (Mathf.Approximately(Mathf.Abs(gameObject.transform.up.y), 1.0f))
        {
            upVect.y = (int)gameObject.transform.up.y * upVectMultiplier;
        }

        return upVect;
    }

    /* Sets the input food item to be the item on the belt */
    public void SetFoodToBelt(FoodBase food)
    {
        foodOnBelt = food;
        perAlongSpline = 0.0f;
        /* Get the position along the spline */
        float3 splinePos = splineContainer.EvaluatePosition(splineContainer.Spline, perAlongSpline);
        foodOnBelt.transform.position = splinePos;
    }

    public void ClearFoodFromBelt()
    {
        foodOnBelt = null;
        perAlongSpline = 0.0f;
    }

    /* Moves the current food item on the conveyer belt along */
    public void MoveFoodAlongBelt()
    {
        if (foodOnBelt != null)
        {
            if (perAlongSpline < 1.0f)
            {
                perAlongSpline += perOffset;
                /* Get the position along the spline */
                float3 splinePos = splineContainer.EvaluatePosition(splineContainer.Spline, perAlongSpline);
                /* Set position of the food to the world position */
                foodOnBelt.transform.position = splinePos;
            }
            
            /* Food is at end of conveyer belt, wait unless there is a conveyer belt attached,
            if so, move food to next belt */
            else if (perAlongSpline >= 1.0f)
            {
                /* Determine the up vector for our object */
                Vector3Int upVect = this.DetermineUpVector(false);

                // TODO - this is the exact same as the fridge code, see if it can be combined to a common function
                /* Determine the cell connected to the end of the conveyer belt */
                Vector3Int cellGridPositionT = new Vector3Int(cellGridPosition.x+upVect.x, cellGridPosition.y+upVect.y, cellGridPosition.z);
                Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPositionT);
                Vector2 cellWorldPosition2D = new Vector2(cellWorldPosition.x, cellWorldPosition.y);

                /* First check if there is an object connected to the belt */
                Collider2D cellCollider = Physics2D.OverlapPoint(cellWorldPosition2D, 1<<LayerMask.NameToLayer("Game Object"));
                if (cellCollider != null)
                {
                    /* Then determine the type of object */
                    if (gameObject.tag == "ConveyerBelt")
                    {
                        ConveyerBelt connectedBelt = cellCollider.gameObject.GetComponent<ConveyerBelt>();

                        /* Determine the up vector for the collided object */
                        Vector3Int collidedUpVect = connectedBelt.DetermineUpVector(true);

                        Debug.Log("Here:");
                        Debug.Log(upVect.x + " " + upVect.y);
                        Debug.Log(collidedUpVect.x + " " + collidedUpVect.y);

                        /* Then make sure it is facing the same direction */
                        if ((collidedUpVect.x == upVect.x) && (collidedUpVect.y == upVect.y))
                        {
                            /* Then make sure there is currently nothing on the conveyer belt */
                            if (connectedBelt.foodOnBelt == null)
                            {
                                /* Move the food on the belt to the next belt */
                                connectedBelt.SetFoodToBelt(foodOnBelt);
                                ClearFoodFromBelt();
                            }
                        }
                    }
                    /* Default invalid collider */
                    else
                    {

                    }
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
    
    /* Override the OnDestroy to ensure proper cleanup */
    protected override void OnDestroy()
    {
        /* remove the belt from our Game Controller list */
        gameController.RemoveConveyerBelt(this);

        /* If there is an object on the belt, delete it too */
        if (foodOnBelt != null)
        {
            Destroy(foodOnBelt);
            ClearFoodFromBelt();
        }

        /* Call Parent On Destroy to delete the object */
        base.OnDestroy();
    }
}
