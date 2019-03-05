using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    #region Variables

    #region PublicVariables

    public Vector2 tiltFactor;

    #endregion

    #region PrivateVariables

    static CameraController cameraController;

    #endregion

    #endregion

    #region Properties

    public static CameraController Instance {
        get{
            if(!cameraController){
                cameraController = FindObjectOfType<CameraController>() as CameraController;
                if(!cameraController){
                    Debug.LogError("There was no active CameraController script on a GameObject in your scene");
                }
            }
            return cameraController;
        }
    }

    #endregion

    #region MonoBehaviourMethods

    private void Start()
    {
        InputManager.CalibrateAccelerometer();
    }

    void Update()
    {
        //EventManager.TriggerEvent(EventName.TILT_PHONE);

        if(Mathf.Abs(Input.mouseScrollDelta.x) > 0 || Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            EventManager.TriggerEvent(EventName.SCROLLING);
        }
        if(Input.GetKeyDown(KeyCode.B)){
            ScreenCapture.CaptureScreenshot(System.DateTime.Now.ToString("O"));
        }

        //if(InputManager.IsFingerMoving)
        //{
        //    EventManager.TriggerEvent(EventName.DRAGGING_FINGER);
        //}else if(Mathf.Abs(InputManager.CurrentMovementVector.x) > 0.1f){
        //    InputManager.CurrentMovementVector = Vector2.Lerp(InputManager.CurrentMovementVector, Vector2.zero, 0.05f);
        //    EventManager.TriggerEvent(EventName.DRAGGING_FINGER);
        //}
        else if(InputManager.Holding()){
            InputManager.CalibrateAccelerometer();
        }
    }

    #endregion

    #region Methods

    #region PublicMethods

    #endregion

    #region PrivateMethods

    #endregion

    #endregion

    #region Coroutines

    #endregion
}
