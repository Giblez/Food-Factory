using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Camera mCamera;
    public InputAction holdCamAction;
    public InputAction zoomAction;
    public InputAction tempAction;
    
    public float CAM_ADJ_VAL = 0.5f;
    public float MAX_CAM_SIZE = 10.0f;
    public float MIN_CAM_SIZE = 1.0f;

    private Vector3 camInitPos;
    private bool cameraHeld;

    // Start is called before the first frame update
    void Start()
    {
        mCamera = GetComponent<Camera>();
        holdCamAction = InputSystem.actions.FindAction("UI/HoldCamera");
        zoomAction = InputSystem.actions.FindAction("UI/ScrollWheel");
        tempAction = InputSystem.actions.FindAction("UI/MoveHeldObject2");

        cameraHeld = false;
    }

    // Update is called once per frame
    void Update()
    {
        /* Initiate Camera movement */
        if (holdCamAction.ReadValue<float>() == 1.0f && cameraHeld == false)
        {
            /* Camera held */
            cameraHeld = true;

            /* Set the initial camera position to the mouse location */
            camInitPos = mCamera.ScreenToWorldPoint(Input.mousePosition);
            camInitPos.z = -10.0f;
        }

        /* Handle Camera movement based off original vector an mouse position */
        if (holdCamAction.ReadValue<float>() == 1.0f && cameraHeld == true)
        {
            Vector3 mousePosition = mCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = -10.0f;    
            mCamera.transform.position = mCamera.transform.position - (mousePosition - camInitPos);
        }

        /* Camera released */
        if (holdCamAction.ReadValue<float>() == 0.0f && cameraHeld == true)
        {
            /* Camera released */
            cameraHeld = false;
            Debug.Log("Good");
        }

        /* Handle Camera zooming */
        Vector2 scrollValue = zoomAction.ReadValue<Vector2>();
        if (scrollValue.y != 0)
        {
            /* Zoom in */
            if (scrollValue.y > 0)
            {
                if (mCamera.orthographicSize > MIN_CAM_SIZE)
                {
                    mCamera.orthographicSize = mCamera.orthographicSize - CAM_ADJ_VAL;
                }
            }

            /* Zoom out */
            else 
            {
                if (mCamera.orthographicSize < MAX_CAM_SIZE)
                {
                    mCamera.orthographicSize = mCamera.orthographicSize + CAM_ADJ_VAL;
                }
            }
        }
    }
}
