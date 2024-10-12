using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;


public class PlayerController : MonoBehaviour
{
    public InputAction placeObjAction;
    public InputAction tempAction;
    public InputAction tempAction2;
    public InputAction cancelObjAction;
    public PrefabBase objectInHand;
    public GameController gameController;
    
    /* Game objects */
    public Camera mCamera;
    public Grid mGrid;

    private Vector2 placedLoc;

    // Start is called before the first frame update
    void Start()
    {
        placeObjAction = InputSystem.actions.FindAction("UI/PlaceObject");
        tempAction = InputSystem.actions.FindAction("UI/Temp");
        tempAction2 = InputSystem.actions.FindAction("UI/Temp2");
        cancelObjAction = InputSystem.actions.FindAction("UI/CancelObject");
        objectInHand = null;
        placedLoc = new Vector2(Mathf.Infinity, Mathf.Infinity);
    }

    // Update is called once per frame
    void Update()
    {
        /* If the object is placed */
        if (placeObjAction.ReadValue<float>() == 1.0 && objectInHand != null)
        {
            if (objectInHand.GetType() == typeof(ConveyerBelt))
            {
                // Check if we should set rotation of conveyerbelt
            }
            Vector3 tPosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            tPosition.z = 0;
            Vector3Int cellPosition = mGrid.WorldToCell(tPosition);
            tPosition = mGrid.GetCellCenterWorld(cellPosition);
            Vector2 tVector2d = new Vector2(tPosition.x, tPosition.y);
            if (placedLoc != tVector2d)
            {
                placedLoc = tVector2d;
                objectInHand.gameObject.layer = LayerMask.NameToLayer("Game Object");

                // TODO - add this to a function in the future since its the same as summoning a conveyer belt below
                PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Machines/ConveyerBelt/ConveyerBeltPrefab.prefab", typeof(PrefabBase)));
                prefab.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
                gameController.conveyerBeltList.Add((ConveyerBelt)prefab);    
                objectInHand = prefab;  
            }
        }

        /* Temp action to summon a raw chicken */
        if (tempAction.ReadValue<float>() == 1.0f && objectInHand == null)
        {
            PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Food/Ingredients/Meat/RawChicken/RawChickenPrefab.prefab", typeof(PrefabBase)));
            prefab.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
            objectInHand = prefab;
        }

        /* Temp action to summon a conveyer belt */
        if (tempAction2.ReadValue<float>() == 1.0f && objectInHand == null)
        {
            PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Machines/ConveyerBelt/ConveyerBeltPrefab.prefab", typeof(PrefabBase)));
            objectInHand = prefab;
            objectInHand.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
            gameController.conveyerBeltList.Add((ConveyerBelt)objectInHand);
        }

        /* If the placement is cancelled */
        if (cancelObjAction.ReadValue<float>() == 1.0 && objectInHand != null)
        {
            gameController.RemoveConveyerBelt((ConveyerBelt)objectInHand);
            objectInHand.Destroy();
            objectInHand = null;
            placedLoc = new Vector2(Mathf.Infinity, Mathf.Infinity);
        }

        /* If there is an object in hand */
        if (objectInHand != null)
        {
            Vector3 tPosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            tPosition.z = 0;

            Vector3Int cellPosition = mGrid.WorldToCell(tPosition);
            objectInHand.transform.position = mGrid.GetCellCenterWorld(cellPosition);
        }
    }
}
