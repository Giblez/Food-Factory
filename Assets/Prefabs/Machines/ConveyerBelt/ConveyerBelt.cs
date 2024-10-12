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
        float3 splinePos = SplineUtility.EvaluatePosition(splineContainer.Spline, perAlongSpline);
        /* Transform that to a world position */
        float3 worldPos = splineContainer.transform.TransformPoint(splinePos);
        foodOnBelt.transform.position = worldPos;
    }

    public void ClearFoodFromBelt()
    {
        foodOnBelt = null;
    }

    /* Moves the current food item on the conveyer belt along */
    public void MoveFoodAlongBelt()
    {
        if (foodOnBelt != null)
        {
            perAlongSpline += perOffset;
            /* Get the position along the spline */
            float3 splinePos = SplineUtility.EvaluatePosition(splineContainer.Spline, perAlongSpline);
            /* Transform that to a world position */
            float3 worldPos = splineContainer.transform.TransformPoint(splinePos);
            /* Set position of the food to the world position */
            foodOnBelt.transform.position = worldPos;

            if (perAlongSpline == 1.0f)
            {
                perAlongSpline = 0.0f;
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
    }
}
