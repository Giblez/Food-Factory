using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class ConveyerBelt : PrefabBase
{
    /* Public member variables */
    public Animator animator;

    // TODO - try and get these from the animator varable above in the future
    public float animLenSec;
    public int animKeys;
    public float lenOfKey;  

    public Vector3 beltMoveInc;
    public FoodBase foodOnBelt;
    private float perAlongSpline;
    private float perOffset;

    public SplineContainer splineContainer;
    private BezierKnot startKnot;
    private BezierKnot endKnot;

    /* References to the objects connected to this conveyer belt */
    public PrefabBase startConnector;
    public PrefabBase endConnector;

    // Start is called before the first frame update
    protected override void Start()
    {
        /* Call parent's Start */
        base.Start();

        /* Initiate our Spline and Knots */
        splineContainer = gameObject.GetComponent<SplineContainer>();
        BezierKnot[] Knots = splineContainer.Spline.ToArray();
        startKnot = Knots[0];
        endKnot = Knots[1];

        float3 startKnotPos;
        startKnotPos = splineContainer.transform.TransformPoint(startKnot.Position);
        float3 endKnotPos;
        endKnotPos = splineContainer.transform.TransformPoint(endKnot.Position);


        Debug.Log(startKnotPos.x + " " + startKnotPos.y + " " + startKnotPos.z);
        Debug.Log(endKnotPos.x + " " + endKnotPos.y + " " + endKnotPos.z);

        /* Set our belt movement transpose increment value */
        // beltMoveInc = animLenSec / 

        /* Retrieve and start our animation */
        animator = gameObject.GetComponent<Animator>();
        animator.Rebind();
        perAlongSpline = 0.0f;
        lenOfKey = animLenSec / animKeys;
        perOffset = 1.0f / animKeys;

        // TODO - remove this in the future
        SetFoodToBelt(foodOnBelt);
        // foodOnBelt = null;

        spriteRenderer.sortingLayerName = "Belt";
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
            // Debug.Log(worldPos.x + ", " + worldPos.y + ", " + perAlongSpline + ", " + perOffset);

            if (perAlongSpline == 1.0f)
            {
                // Debug.Log("hit end");
                // foodOnBelt = null;
                perAlongSpline = 0.0f;
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
