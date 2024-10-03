using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBase : PrefabBase
{
    public FoodType foodType;

    // Start is called before the first frame update
    protected override void Start()
    {
        /* Call parent's Start */
        base.Start();

        spriteRenderer.sortingLayerName = "Food";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
