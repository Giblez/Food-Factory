using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;


public class PlayerController : MonoBehaviour
{
    /* Input Actions */
    public InputAction placeObjAction;
    public InputAction selectObjAction;
    public InputAction deleteObjAction;
    public InputAction tempAction;
    public InputAction tempAction2;
    public InputAction tempAction3;
    public InputAction tempAction4;
    public InputAction cancelObjAction;
    public InputAction rotateHeldObjectAction;
    public InputAction moveHeldObjectAction;

    /* Reference to Game Controller */
    public GameController gameController;
    /* String holding the path to the asset in hand */
    private string pathToAssetInHand;
    /* Reference to the object held in hand */
    public PrefabBase objectInHand;
    /* Boolean to hold if the object has already spun for
    the button press */
    private bool objectInHandSpun;
    /* If the mouse click has occurred */
    private bool mousePressed;
    /* If the selecion box is out */
    private bool selectionBoxOut;
    /* If the cancel keybinding has occurred */
    private bool cancelPressed;
    /* If a button press to summon an object has occurred */
    private bool objSummonPressed;
    /* If the delete keybinding has occurred */
    private bool deletePressed;
    
    /* Selection Box stuff */
    /* Object to hold the Selection Box */
    private PrefabBase selectionBoxFab;
    /* Vector to hold the initial selection box mouse location */
    private Vector3 selectionBoxInitPos;

    /* Game objects */
    public Camera mCamera;

    private Vector2 placedLoc2D;

    // Start is called before the first frame update
    void Start()
    {
        /* Initialize input mapping */
        placeObjAction = InputSystem.actions.FindAction("UI/PlaceObject");
        selectObjAction = InputSystem.actions.FindAction("UI/SelectObject");
        deleteObjAction = InputSystem.actions.FindAction("UI/DeleteObject");
        tempAction = InputSystem.actions.FindAction("UI/Temp");
        tempAction2 = InputSystem.actions.FindAction("UI/Temp2");
        tempAction3 = InputSystem.actions.FindAction("UI/Temp3");
        tempAction4 = InputSystem.actions.FindAction("UI/Temp4");
        cancelObjAction = InputSystem.actions.FindAction("UI/CancelObject");
        rotateHeldObjectAction = InputSystem.actions.FindAction("UI/RotateHeldObject");
        moveHeldObjectAction = InputSystem.actions.FindAction("UI/MoveHeldObject");

        /* Initialize misc variables */
        pathToAssetInHand = "";
        objectInHand = null;
        placedLoc2D = new Vector2(Mathf.Infinity, Mathf.Infinity);
        objectInHandSpun = false;
        mousePressed = false;
        selectionBoxOut = false;
        selectionBoxInitPos = new Vector3(0.0f, 0.0f, 0.0f);
        cancelPressed = false;
        deletePressed = false;
        objSummonPressed = false;
        selectionBoxFab = null;
    }

    // Update is called once per frame
    void Update()
    {
        /*************************
        * Move Object with mouse *
        *************************/

        /* If there is an object in hand map it to the mouse location */
        if (objectInHand != null)
        {
            /* Set the object position to the center of the current grid cell the mouse is over */
            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector3Int cellPosition = gameController.mGrid.WorldToCell(mousePosition);
            objectInHand.transform.position = gameController.mGrid.GetCellCenterWorld(cellPosition);
        }


        /**************************
        * Place Object Keybinding *
        **************************/

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

                /* Place the object in hand onto the grid */
                objectInHand.gameObject.layer = LayerMask.NameToLayer("Game Object");

                /* Set the new placed location to the current cell */
                placedLoc2D = cellWorldPosition2D;

                // TODO - add this to a function in the future since its the same as summoning a conveyer belt below
                PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath(pathToAssetInHand, typeof(PrefabBase)), 
                    cellWorldPosition, objectInHand.transform.rotation);
                objectInHand = null;
                prefab.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
                objectInHand = prefab;  
            }
        }


        /***************************
        * Summon Object Keybinding *
        ***************************/

        /* Temp action to summon a fridge */
        if (tempAction.ReadValue<float>() == 1.0f && objSummonPressed == false)
        {
            /* Destroy the object in hand if one exists */
            if (objectInHand != null)
            {
                Destroy(objectInHand);
            }
            
            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector3Int cellGridPosition = gameController.mGrid.WorldToCell(mousePosition);
            Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPosition);

            pathToAssetInHand = "Assets/Prefabs/Machines/Fridge/FridgePrefab.prefab";
            PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath(pathToAssetInHand, typeof(PrefabBase)),
                cellWorldPosition, Quaternion.identity);
            prefab.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
            objectInHand = prefab;
            
            /* Cancel button pressed */
            cancelPressed = true;
            objSummonPressed = true;
        }

        /* Temp action to summon a conveyer belt */
        if (tempAction2.ReadValue<float>() == 1.0f && objSummonPressed == false)
        {
            // TODO - do the same for all summoned objects
            /* For conveyer belts no need to erase and re instantiate a new one */
            if (objectInHand == null || (objectInHand != null && objectInHand.tag != "ConveyerBelt")) 
            {
                /* Destroy the object in hand if one exists */
                if (objectInHand != null)
                {
                    Destroy(objectInHand);
                }

                Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                Vector3Int cellGridPosition = gameController.mGrid.WorldToCell(mousePosition);
                Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPosition);

                pathToAssetInHand = "Assets/Prefabs/Machines/ConveyerBelt/ConveyerBeltPrefab.prefab";
                PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath(pathToAssetInHand, typeof(PrefabBase)),
                    cellWorldPosition, Quaternion.identity);
                objectInHand = prefab;
                objectInHand.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
                objSummonPressed = true;
            }
        }

        /* Temp action to summon a corner conveyer belt */
        if (tempAction3.ReadValue<float>() == 1.0f && objSummonPressed == false)
        {
            /* Destroy the object in hand if one exists */
            if (objectInHand != null)
            {
                Destroy(objectInHand);
            }

            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector3Int cellGridPosition = gameController.mGrid.WorldToCell(mousePosition);
            Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPosition);

            pathToAssetInHand = "Assets/Prefabs/Machines/ConveyerBelt/ConveyerBeltCorner/ConveyerBeltCornerLeftPrefab.prefab";
            PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath(pathToAssetInHand, typeof(PrefabBase)),
                cellWorldPosition, Quaternion.identity);
            objectInHand = prefab;
            objectInHand.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
            objSummonPressed = true;
        }

        /* Temp action to summon a corner conveyer belt */
        if (tempAction4.ReadValue<float>() == 1.0f && objSummonPressed == false)
        {
            /* Destroy the object in hand if one exists */
            if (objectInHand != null)
            {
                Destroy(objectInHand);
            }

            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector3Int cellGridPosition = gameController.mGrid.WorldToCell(mousePosition);
            Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPosition);

            pathToAssetInHand = "Assets/Prefabs/Machines/ConveyerBelt/ConveyerBeltCorner/ConveyerBeltCornerRightPrefab.prefab";
            PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath(pathToAssetInHand, typeof(PrefabBase)),
                cellWorldPosition, Quaternion.identity);
            objectInHand = prefab;
            objectInHand.gameObject.layer = LayerMask.NameToLayer("Held In Hand");
            objSummonPressed = true;
        }

        if (objSummonPressed == true &&
                tempAction.ReadValue<float>() == 0.0f &&
                tempAction2.ReadValue<float>() == 0.0f &&
                tempAction3.ReadValue<float>() == 0.0f &&
                tempAction4.ReadValue<float>() == 0.0f)

        {
            objSummonPressed = false;
        }


        /***************************
        * Select Object Keybinding *
        ***************************/

        /* Instantiate Selection Box */
        if (selectObjAction.ReadValue<float>() == 1.0f && objectInHand == null && 
            mousePressed == false && selectionBoxOut == false)
        {
            /* Set mouse pressed to true so we dont keep entering here */
            mousePressed = true;
            selectionBoxOut = true;

            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0.0f;
            selectionBoxInitPos = mousePosition;

            /* Instantiate the selection box */
            selectionBoxFab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Misc/SelectionBox/SelectionBoxPrefab.prefab", typeof(PrefabBase)), 
                mousePosition, Quaternion.identity);
            selectionBoxFab.transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);
        }

        /* Update Selection box based off original vector and mouse position */
        if (selectObjAction.ReadValue<float>() == 1.0f && selectionBoxOut == true 
            && selectionBoxFab != null)
        {
            // TODO - come back and look maybe fix to make pixel perfect in the future
            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            float x = mousePosition.x - selectionBoxInitPos.x;
            float y = (mousePosition.y - selectionBoxInitPos.y);
            /* scale needs to flip for y since it grows downward */
            selectionBoxFab.transform.localScale = new Vector3(x*3.0f, y*-3.0f, 1.0f);  
            selectionBoxFab.transform.position =  new Vector3(mousePosition.x-(x/2.0f), mousePosition.y-(y/2.0f), 0.0f);         
        }

        /* Selection released */
        if (selectObjAction.ReadValue<float>() == 0.0f && mousePressed == true)
        {
            /* Clear previous selections */
            GameObject[] prevSelectionMarkers = GameObject.FindGameObjectsWithTag("SelectionMarker");
            foreach (GameObject obj in prevSelectionMarkers)
            {
                Destroy(obj);
            }

            /* Construct Point and Size vectors for overlap box */
            Vector2 point = new Vector2(selectionBoxFab.transform.position.x, selectionBoxFab.transform.position.y);
            Vector2 size = new Vector2(Mathf.Abs(selectionBoxFab.transform.localScale.x/3.0f), 
                Mathf.Abs(selectionBoxFab.transform.localScale.y/3.0f));

            /* Retrieve all objects witin the Selection Box */
            Collider2D[] cellColliders = Physics2D.OverlapBoxAll(point, size, selectionBoxFab.transform.eulerAngles.z, 
                1<<LayerMask.NameToLayer("Game Object"));
            foreach (Collider2D collider in cellColliders)
            {
                /* Instantiate the new selection */
                PrefabBase prefab = Instantiate((PrefabBase)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Misc/SelectionMarker/SelectionMarkerPrefab.prefab", typeof(PrefabBase)), 
                    collider.gameObject.transform.position, Quaternion.identity);
            }             

            /* Mouse press released */
            mousePressed = false;
            selectionBoxOut = false;
            selectionBoxFab = null;
        }


        /***************************
        * Cancel Object Keybinding *
        ***************************/

        /* If the placement is cancelled */
        if (cancelObjAction.ReadValue<float>() == 1.0 && objectInHand != null
            && cancelPressed == false)
        {
            Destroy(objectInHand);
            objectInHand = null;
            placedLoc2D = new Vector2(Mathf.Infinity, Mathf.Infinity);

            /* Cancel button pressed */
            cancelPressed = true;
        }

        /* If the selected object is cancelled */
        if (cancelObjAction.ReadValue<float>() == 1.0 && objectInHand == null
            && cancelPressed == false)
        {
            /* Clear previous selections */
            GameObject[] prevSelectionMarkers = GameObject.FindGameObjectsWithTag("SelectionMarker");
            foreach (GameObject obj in prevSelectionMarkers)
            {
                Destroy(obj);
            }

            /* Cancel button pressed */
            cancelPressed = true;
        }

        /* Cancel object button released */
        if (cancelObjAction.ReadValue<float>() == 0.0 && cancelPressed == true)
        {
            cancelPressed = false;
        }


        /***************************
        * Delete Object Keybinding *
        ***************************/

        if (deleteObjAction.ReadValue<float>() == 1.0 && deletePressed == false)
        {
            /* Clear previous selections */
            GameObject[] prevSelectionMarkers = GameObject.FindGameObjectsWithTag("SelectionMarker");
            foreach (GameObject obj in prevSelectionMarkers)
            {
                Vector3 selectPosition = obj.transform.position;
                selectPosition.z = 0;
                Vector3Int cellGridPosition = gameController.mGrid.WorldToCell(selectPosition);
                Vector3 cellWorldPosition = gameController.mGrid.GetCellCenterWorld(cellGridPosition);
                Vector2 cellWorldPosition2D = new Vector2(cellWorldPosition.x, cellWorldPosition.y);

                /* Get the object tied to this Selection Marker */
                Collider2D cellCollider = Physics2D.OverlapPoint(cellWorldPosition2D, 1<<LayerMask.NameToLayer("Game Object"));
                if (cellCollider != null)
                {
                    /* Destroy the object beneath */
                    Destroy(cellCollider.gameObject);
                }
                
                /* Destroy the Select Marker */
                Destroy(obj);
            }

            /* Delete button pressed */
            deletePressed = true;
        }

        /* Delete button released */
        if (deleteObjAction.ReadValue<float>() == 0.0 && deletePressed == true)
        {
            deletePressed = false;
        }


        /***************************
        * Rotate Object Keybinding *
        ***************************/

        /* Is there an object in hand */
        if (objectInHand != null)
        {
            /* Do we want to rotate the object held in hand */
            float rotVal = rotateHeldObjectAction.ReadValue<float>();
            if ((rotVal != 0.0f) && (objectInHandSpun == false))
            {
                objectInHandSpun = true;
                objectInHand.transform.Rotate(0, 0, 90*rotVal);
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