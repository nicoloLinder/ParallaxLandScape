using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    #region Variables

    #region PublicVariables

    #endregion

    #region PrivateVariables

    static InputManager inputManager;

    bool isFingerDown;
    bool isFingerMoving;
    bool inputJustEnded;

    //  Finger variables

    Vector2 startFingerPosition;
    Vector2 currentFingerPosition;
    Vector2 currentMovementVector;

    //  Time variables

    float touchDownTime;
    float timeSinceLastInput;

    const float tapTime = 0.2f;
    const float holdTime = 0.5f;
    const float minimumFingerMovement = 0.1f;

    //  Accelerometer

    Vector3 wantedDeadZone;
    Matrix4x4 calibrationMatrix;

    static float range = 5;
    static float sensibility = 100;
    static float speed = 3;

    List<Vector3> pastAcceleration;

    #endregion

    #endregion

    #region Properties

    public static InputManager instance
    {
        get
        {
            if (!inputManager)
            {
                inputManager = FindObjectOfType(typeof(InputManager)) as InputManager;

                if (!inputManager)
                {
                    Debug.LogWarning("There was no active InputManager script on a GameObject in your scene, a new GameObject with and InputManager was created");

                    GameObject inputManagerGameObject = new GameObject("InputManager");
                    inputManager = inputManagerGameObject.AddComponent<InputManager>();
                }
            }

            return inputManager;
        }
    }

    public static Vector2 StartFingerPosition { get { return instance.startFingerPosition; } }
    public static Vector2 StartWorldFingerPosition { get { return Camera.main.ScreenToWorldPoint(StartFingerPosition); } }
    public static Vector2 CurrentWorldFingerPosition { get { return Camera.main.ScreenToWorldPoint(CurrentFingerPosition); } }
    public static Vector2 CurrentFingerPosition { get { return instance.currentFingerPosition; } }
    public static Vector2 CurrentMovementVector { get { return instance.currentMovementVector; } set { instance.currentMovementVector = value; } }

    public static bool IsFingerDown { get { return instance.isFingerDown; } }
    public static bool IsFingerMoving { get { return instance.isFingerMoving; } }
    public static bool InputJustEnded { get { return instance.inputJustEnded; } }

    public static float FingerDownTime { get { return instance.touchDownTime; } }

    public static Vector3 acceleration
    {
        get
        {

            Vector3 mean = Vector3.zero;

            if(instance.pastAcceleration == null){
                instance.pastAcceleration = new List<Vector3>() {
                    instance.calibrationMatrix.MultiplyVector(Input.acceleration),
                    instance.calibrationMatrix.MultiplyVector(Input.acceleration),
                    instance.calibrationMatrix.MultiplyVector(Input.acceleration),
                    instance.calibrationMatrix.MultiplyVector(Input.acceleration),
                    instance.calibrationMatrix.MultiplyVector(Input.acceleration)
                };
            }else{
                instance.pastAcceleration.RemoveAt(0);
                instance.pastAcceleration.Add(instance.calibrationMatrix.MultiplyVector(Input.acceleration));
            }


            for (int i = 0; i < instance.pastAcceleration.Count; i++)
            {
                mean += instance.pastAcceleration[0];
            }

            return (mean / instance.pastAcceleration.Count)/5;

        }
    }

    #endregion

    #region MonoBehabiourMethods

    void Start()
    {

    }

    void Update()
    {
        if (!GetTouchInputDown() && !GetTouchInput() && !GetTouchInputUp() && isFingerDown)
        {
            startFingerPosition = Vector2.zero;
            currentFingerPosition = Vector2.zero;
            isFingerDown = false;
            isFingerMoving = false;
        }

        inputJustEnded = false;

        timeSinceLastInput += Time.deltaTime;

        //  Input started
        if (GetTouchInputDown())
        {
            isFingerDown = true;
            startFingerPosition = currentFingerPosition = GetScreenTouchPosition();
            touchDownTime = 0;
        }
        //  Input happening
        else if (GetTouchInput())
        {
            isFingerMoving = (Vector2.Distance(currentFingerPosition, GetScreenTouchPosition()) > 0.0001f);
            currentMovementVector = GetScreenTouchPosition() - currentFingerPosition;
            currentFingerPosition = GetScreenTouchPosition();
            touchDownTime += Time.deltaTime;
        }
        //  Input just ended
        else if (GetTouchInputUp())
        {
            currentFingerPosition = GetScreenTouchPosition();
            isFingerDown = false;
            isFingerMoving = false;
            inputJustEnded = true;
            timeSinceLastInput = 0;
        }


    }

    //void LateUpdate()
    //{
    //    //  Input not present

    //}

    #endregion

    #region Methods

    #region PublicMethods

    public void SetRange(float _range)
    {
        range = _range;
    }

    public void SetSensibility(float _sensibility)
    {
        sensibility = _sensibility;
    }

    /// <summary>
    /// Checks is a Tap has happened in the current frame;
    /// </summary>
    /// <returns>The tap.</returns>

    public static bool Tap()
    {
        return !instance.isFingerDown && instance.touchDownTime <= tapTime && instance.inputJustEnded;
    }

    /// <summary>
    /// Checks if a Hold has happened in the current frame;
    /// </summary>
    /// <returns>The hold.</returns>

    public static bool Hold()
    {
        return !instance.isFingerDown && instance.touchDownTime >= holdTime && instance.inputJustEnded;
    }

    /// <summary>
    /// Checks if a holding event is currently happening;
    /// </summary>
    /// <returns>The holding.</returns>

    public static bool Holding()
    {
        return instance.isFingerDown && instance.touchDownTime >= holdTime;
    }

    /// <summary>
    /// Calculates the vector between the touch starting position and the touch end position;
    /// </summary>
    /// <returns>The movement vector.</returns>

    public static Vector2 FingerMovementVector()
    {
        return instance.currentFingerPosition - instance.startFingerPosition;
    }

    public static bool FingerMoved()
    {
        return CurrentMovementVector.sqrMagnitude > Mathf.Pow(minimumFingerMovement, 2);
    }

    public static Vector2 Direction()
    {
        Vector2 direction = Vector2.zero;

#if UNITY_EDITOR || UNITY_STANDALONE

        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

#elif UNITY_IOS || UNITY_ANDROID

        direction = acceleration;


#endif

        return direction;
    }

    public static void CalibrateAccelerometer()
    {
        instance.wantedDeadZone = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0f, 0f, -1f), instance.wantedDeadZone);
        //create identity matrix ... rotate our matrix to match up with down vec
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, rotateQuaternion, new Vector3(1f, 1f, 1f));
        //get the inverse of the matrix
        instance.calibrationMatrix = matrix.inverse;

    }

    #endregion

    #region PrivateMethods


    /// <summary>
    /// Check if the touch input has started
    /// </summary>
    /// <returns><c>true</c>, if touch input has just started, <c>false</c> otherwise.</returns>
    bool GetTouchInputDown()
    {

        bool isTouchInputDown = false;

#if UNITY_EDITOR || UNITY_STANDALONE

        isTouchInputDown = Input.GetMouseButtonDown(0);

#elif UNITY_IOS || UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            isTouchInputDown = Input.touches[0].phase == TouchPhase.Began;
        }

#endif

        return isTouchInputDown;
    }

    bool GetTouchInputUp()
    {

        bool isTouchInputUp = false;

#if UNITY_EDITOR || UNITY_STANDALONE

        isTouchInputUp = Input.GetMouseButtonUp(0);

        //#elif UNITY_IOS || UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            isTouchInputUp = Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled;
        }

#endif

        return isTouchInputUp;
    }

    /// <summary>
    /// Check if the touch input
    /// </summary>
    /// <returns><c>true</c>, if touch input is in progress, <c>false</c> otherwise.</returns>
    bool GetTouchInput()
    {

        bool isTouchInput = false;

#if UNITY_EDITOR || UNITY_STANDALONE

        isTouchInput = Input.GetMouseButton(0);

#elif UNITY_IOS || UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            isTouchInput = Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary;
        }

#endif

        return isTouchInput;
    }

    /// <summary>
    /// Gets the screen touch position.
    /// </summary>
    /// <returns>The screen touch position.</returns>
    Vector2 GetScreenTouchPosition()
    {

        Vector2 screenTouchPosition = Vector2.negativeInfinity;

#if UNITY_EDITOR || UNITY_STANDALONE

        screenTouchPosition = Input.mousePosition;

#elif UNITY_IOS || UNITY_ANDROID

        if(Input.touchCount > 0){
            screenTouchPosition = Input.touches[0].position;
        }

#endif

        return screenTouchPosition;
    }

    #endregion

    #endregion
}
