using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : PrefabBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        /* Call parent's Start */
        base.Start();

        /* Set sorting layer */
        spriteRenderer.sortingLayerName = "UI";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
