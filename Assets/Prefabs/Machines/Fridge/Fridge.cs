using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NewBehaviourScript : PrefabBase
{
    /* Dictionary of items within the fridge */
    Dictionary<string, int> storedObjs;

    /* Coroutine to take item out of fridge */
    public float takeItemOutDelay;
    private IEnumerator takeItemOutCoroutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        /* Call parent's Start */
        base.Start();

        storedObjs = new Dictionary<string, int>();
        storedObjs.Add("RawChicken", 1);

        takeItemOutCoroutine = TakeItemOurOfFridge(takeItemOutDelay);
        StartCoroutine(takeItemOutCoroutine);
    }

    private IEnumerator TakeItemOurOfFridge(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            
            // TODO - make a world wide function to do this:
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
                    if (connectedBelt.foodOnBelt == null && storedObjs["RawChicken"] > 0)
                    {
                        RawChicken prefab = Instantiate((RawChicken)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Food/Ingredients/Meat/RawChicken/RawChickenPrefab.prefab", typeof(RawChicken)));
                        connectedBelt.SetFoodToBelt(prefab);
                        storedObjs["RawChicken"] -= 1;
                    }
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
    }
}
