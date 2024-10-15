using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class ConveyerBelt : PrefabBase
{
    /* Animation member variables */
    public Animator animator;
    private float perAlongSpline;
    private float perOffset;
    public int animKeys;
    public Vector3 beltMoveInc;
    public FoodBase foodOnBelt;

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

    }

    /* Sets the input food item to be the item on the belt */
    public void SetFoodToBelt(FoodBase food)
    {
        foodOnBelt = food;
        perAlongSpline = 0.0f;
        /* Get the position along the spline */
        float3 splinePos = splineContainer.EvaluatePosition(splineContainer.Spline, perAlongSpline);
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
            
            /* Dont wait till next iteraition to move food to next belt if needed */
            if (perAlongSpline >= 1.0f)
            {
                // TODO - this is the exact same as the fridge code, see if it can be combined to a common function
                /* Food is at end of conveyer belt, wait unless there is a conveyer belt attached,
                if so, move food to next belt */
                Vector3Int cellGridPositionT = new Vector3Int(cellGridPosition.x, cellGridPosition.y-1, cellGridPosition.z);
                Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPositionT);
                Vector2 cellWorldPosition2D = new Vector2(cellWorldPosition.x, cellWorldPosition.y);

                /* First check if there is an object connected to the fridge (TODO - for now just checking the one below) */
                Collider2D cellCollider = Physics2D.OverlapPoint(cellWorldPosition2D, 1<<LayerMask.NameToLayer("Game Object"));
                if (cellCollider != null)
                {
                    /* Then make sure the object is a conveyer belt */
                    if (cellCollider.gameObject.tag == "ConveyerBelt")
                    {
                        ConveyerBelt connectedBelt = cellCollider.gameObject.GetComponent<ConveyerBelt>();

                        /* Then make sure there is currently nothing on the conveyer belt */
                        if (connectedBelt.foodOnBelt == null)
                        {
                            /* Move the food on the belt to the next belt */
                            connectedBelt.SetFoodToBelt(foodOnBelt);
                            ClearFoodFromBelt();
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
