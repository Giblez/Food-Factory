using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineBase : PrefabBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        /* Call parent's Start */
        base.Start();

        /* Set sorting layer */
        spriteRenderer.sortingLayerName = "Machine";
    }

    // Update is called once per frame
    void Update()
    {
    }
}
