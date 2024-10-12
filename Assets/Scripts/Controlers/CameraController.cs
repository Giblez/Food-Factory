using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Camera mCamera;
    public InputAction camHoldAction;
    public InputAction camMoveAction;
    public InputAction zoomAction;
    public float CAM_ADJ_VAL = 0.5f;
    public float MAX_CAM_SIZE = 10.0f;
    public float MIN_CAM_SIZE = 1.0f;

    private Vector3 camOffset;

    // Start is called before the first frame update
    void Start()
    {
        mCamera = GetComponent<Camera>();
        camHoldAction = InputSystem.actions.FindAction("UI/RightClick");
        camMoveAction = InputSystem.actions.FindAction("UI/Move");
        zoomAction = InputSystem.actions.FindAction("UI/ScrollWheel");
    }

    // Update is called once per frame
    void Update()
    {
        float cameraHold = camHoldAction.ReadValue<float>();
        if (cameraHold > 0f)
        {
            // TODO - Come back and edit this to make it vector position based instea of just pushing a standard amount
            Vector2 cameraMove = camMoveAction.ReadValue<Vector2>();
            Debug.Log(cameraMove.x + " " + cameraMove.y);
            if (cameraMove.x != 0 || cameraMove.y != 0)
            {
                camOffset = new Vector3(cameraMove.x/-10.0f, cameraMove.y/-10.0f, 0.0f);
                transform.position += camOffset;
            }
        }

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
