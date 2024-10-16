using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;


public class PlayerController : MonoBehaviour
{
    /* Input Actions */
    public InputAction placeObjAction;
    public InputAction tempAction;
    public InputAction tempAction2;
    public InputAction tempAction3;
    public InputAction cancelObjAction;
    public InputAction rotateHeldObjectAction;
    public InputAction moveHeldObjectAction;

    /* Reference to Game Controller */
    public GameController gameController;
    /* Reference to the object held in hand */
    public PrefabBase objectInHand;
    /* Boolean to hold if the object has already spun for
    the button press */
    private bool objectInHandSpun;
    
    /* Game objects */
    public Camera mCamera;

    private Vector2 placedLoc2D;

    // Start is called before the first frame update
    void Start()
    {
        /* Initialize input mapping */
        placeObjAction = InputSystem.actions.FindAction("UI/PlaceObject");
        tempAction = InputSystem.actions.FindAction("UI/Temp");
        tempAction2 = InputSystem.actions.FindAction("UI/Temp2");
        tempAction3 = InputSystem.actions.FindAction("UI/Temp3");
        cancelObjAction = InputSystem.actions.FindAction("UI/CancelObject");
        rotateHeldObjectAction = InputSystem.actions.FindAction("UI/RotateHeldObject");
        moveHeldObjectAction = InputSystem.actions.FindAction("UI/MoveHeldObject");

        /* Initialize misc variables */
        objectInHand = null;
        placedLoc2D = new Vector2(Mathf.Infinity, Mathf.Infinity);
        objectInHandSpun = false;
    }

    // Update is called once per frame
    void Update()
    {
        /* If there is an object in hand map it to the mouse location */
        if (objectInHand != null)
        {
            /* Set the object position to the center of the current grid cell the mouse is over */
            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector3Int cellPosition = gameController.mGrid.WorldToCell(mousePosition);
            objectInHand.transform.position = gameController.mGrid.GetCellCenterWorld(cellPosition);
        }

        /* If the object is placed */
        if (placeObjAction.ReadValue<float>() == 1.0 && objectInHand != null)
        {
            /* Get the center of the current grid cell the mouse is over */
            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector3Int cellGridPosition = gameController.mGrid.WorldToCell(mousePosition);
            Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPosition);
            Vector2 cellWorldPosition2D = new Vector2(cellWorldPosition.x, cellWorldPosition.y);

            /* Check if there is any object already in the grid at the location */
            Collider2D cellCollider = Physics2D.OverlapPoint(cellWorldPosition2D, 1<<LayerMask.NameToLayer("Game Object"));

            /* Check if we are at a new cell position and there is no overlap */
            if ((placedLoc2D != cellWorldPosition2D) && (cellCollider == null))
            {
                objectInHand.cellGridPosition = cellGridPosition;
                /* Check if we should set rotation of conveyerbelt */
                if (objectInHand.GetType() == typeof(ConveyerBelt))
                {
                    ConveyerBelt conveyerBeltInHand = (ConveyerBelt) objectInHand;
                    Debug.Log("New Belt, Count: " + gameController.conveyerBeltList.Count);
                    Vector2 testMove = moveHeldObjectAction.ReadValue<Vector2>();
                    Debug.Log("Move: " + testMove.x + " " + testMove.y);
                    // /* If the mouse is moving right to left */
                    // if (testMove.x > testMove.y)
                    // {
                    //     float xLoc = 0.0f;
                    //     if (testMove.x < 0.0f)
                    //     {
                    //         xLoc = -1.0f;
                    //     }
                    //     else
                    //     {
                    //         xLoc = 1.0f;
                    //     }
                    //     objectInHand.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f*xLoc);
                    // }
                    // /* If the mouse is moving up and down */
                    // else
                    // {
                    //     float yLoc = 0.0f;
                    //     if (testMove.y < 0.0f)
                    //     {
                    //         yLoc = -1.0f;
                    //     }
                    //     else
                    //     {
                    //         yLoc = 1.0f;
                    //     }   
                    //     objectInHand.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f*yLoc);                 
                    // }
                }

                Debug.Log("New Belt");
                /* Place the object in hand onto the grid */
                objectInHand.gameObject.layer = LayerMask.NameToLayer("Game Object");

                /* Set the new placed location to the current cell */
                placedLoc2D = cellWorldPosition2D;

                // TODO - add this to a function in the future since its the same as summoning a conveyer belt below
                // TODO - need to figure out how to to instantiate the last summoned type
                PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Machines/ConveyerBelt/ConveyerBeltPrefab.prefab", typeof(PrefabBase)), 
                    cellWorldPosition, objectInHand.transform.rotation);
                objectInHand = null;
                prefab.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
                gameController.conveyerBeltList.Add((ConveyerBelt)prefab);    
                objectInHand = prefab;  
            }
        }

        /* Temp action to summon a fridge */
        if (tempAction.ReadValue<float>() == 1.0f && objectInHand == null)
        {
            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector3Int cellGridPosition = gameController.mGrid.WorldToCell(mousePosition);
            Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPosition);

            PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Machines/Fridge/FridgePrefab.prefab", typeof(PrefabBase)),
                cellWorldPosition, Quaternion.identity);
            prefab.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
            objectInHand = prefab;
        }

        /* Temp action to summon a conveyer belt */
        if (tempAction2.ReadValue<float>() == 1.0f && objectInHand == null)
        {
            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector3Int cellGridPosition = gameController.mGrid.WorldToCell(mousePosition);
            Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPosition);

            // TODO - need to add instantiate location to mouse
            PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Machines/ConveyerBelt/ConveyerBeltPrefab.prefab", typeof(PrefabBase)),
                cellWorldPosition, Quaternion.identity);
            gameController.conveyerBeltList.Add((ConveyerBelt)prefab);
            objectInHand = prefab;
            objectInHand.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
        }

        /* Temp action to summon a corner conveyer belt */
        if (tempAction3.ReadValue<float>() == 1.0f && objectInHand == null)
        {
            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector3Int cellGridPosition = gameController.mGrid.WorldToCell(mousePosition);
            Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPosition);

            // TODO - need to add instantiate location to mouse
            PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Machines/ConveyerBelt/ConveyerBeltCorner/ConveyerBeltCornerPrefab.prefab", typeof(PrefabBase)),
                cellWorldPosition, Quaternion.identity);
            gameController.conveyerBeltList.Add((ConveyerBelt)prefab);
            objectInHand = prefab;
            objectInHand.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
        }

        /* If the placement is cancelled */
        if (cancelObjAction.ReadValue<float>() == 1.0 && objectInHand != null)
        {
            if (objectInHand.GetType() == typeof(ConveyerBelt) ||
            objectInHand.GetType() == typeof(ConveyerBeltCorner))
            {
                gameController.RemoveConveyerBelt((ConveyerBelt)objectInHand);
            }
            objectInHand.Destroy();
            objectInHand = null;
            placedLoc2D = new Vector2(Mathf.Infinity, Mathf.Infinity);
        }

        /* Is there an object in hand */
        if (objectInHand != null)
        {
            /* Do we want to rotate the object held in hand */
            float rotVal = rotateHeldObjectAction.ReadValue<float>();
            if ((rotVal != 0.0f) && (objectInHandSpun == false))
            {
                objectInHandSpun = true;
                objectInHand.transform.Rotate(0, 0, 90*rotVal);
                Debug.Log(rotVal);
            }
            else
            {
                /* If the key has been lifted and the object has already spun,
                reset the spun boolean */
                if (rotVal == 0.0f && objectInHandSpun == true)
                {
                    objectInHandSpun = false;
                }
            }

        }
    }
}
