using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraFunctionality : MonoBehaviour
{
    public InputActionReference PlayerActionMap;



    public GameObject CameraLock;

    public bool LockView = false;
    public bool CameraActive = true;
    private bool AimCameraReady = false;
    public bool AimCameraActive = false;

    public float RotationSpeed;
    [SerializeField] private float MouseSensitivity = 100f;
    float LockoutTime;
    float CurrentLockoutTime;
    private float X_Rotation;
    private float Y_Rotation=180;
    private float Max_X_Rotation = 30;
    private float Min_X_Rotation = -30;
    private float Max_Y_Rotation = -45+-90;
    private float Min_Y_Rotation = -135+-90;

    public Transform PlayerOrientation;
    public Transform Player;
    public Transform PlayerObject;
    public Transform FirePoint;
    public Transform MainCamera;
    [SerializeField] private Transform AimCamera;


    public Vector3 ViewDirection;
    public Vector3 PlayerVelocity;
    public Vector3 TransformDirection;
    [SerializeField] protected Vector3 MoveDirection;

    [SerializeField] private PlayerMovement PlayerMovementScript;    

    // Start is called before the first frame update
    void Start()
    {
        CurrentLockoutTime = LockoutTime;

        PlayerMovementScript=transform.root.root.GetComponent<PlayerMovement>();
        CameraActive = true;
        MouseSensitivity = 120f;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAimCamera();

        if (PlayerMovementScript.AttackLocked || !PlayerMovementScript.CanMove || !CameraActive) { return; }

        RotateToView();
        HandleLockout();

    }

    public void RotateToView()
    {
        ViewDirection = Player.position - new Vector3(MainCamera.position.x, Player.position.y, MainCamera.position.z);
        PlayerOrientation.forward = ViewDirection.normalized;

        MoveDirection = PlayerActionMap.action.ReadValue<Vector3>();
        MoveDirection = new Vector3(MoveDirection.z, 0, MoveDirection.x * -1);
        Vector3 InputDirection = PlayerOrientation.right * MoveDirection.x + PlayerOrientation.forward * MoveDirection.z;

        if (InputDirection != Vector3.zero)
        {
            PlayerObject.forward = Vector3.Slerp(PlayerObject.forward, InputDirection.normalized, Time.deltaTime * RotationSpeed);

        }

    }

    private void HandleLockout()
    {
        if (LockView && CameraLock.Equals(false))
        {
            CurrentLockoutTime -= Time.deltaTime;
            if (CurrentLockoutTime <= 0)
            {
                ChangeCamLockState(false);
            }
        }
        else if (!LockView)
        {
            CurrentLockoutTime = LockoutTime;
        }
    }

    protected void ChangeCamLockState(bool LockState)
    {

    }

    private void HandleAimCamera()
    {
        if(!CameraActive || PlayerMovementScript.AttackLocked) { return; }
        switch (AimCameraActive)
        {
            case true:
                AimCameraRotation();
                break;

            case false:
                AimCamera.rotation = Quaternion.Euler(PlayerMovementScript.BaltoOrientation.right);
                break;

        }
    }

    public void ChangeActiveCamera(bool AimActive)
    {
        AimCameraActive = AimActive;
        switch (AimCameraActive)
        {
            case true:
                AimCamera.gameObject.SetActive(AimActive);
                MainCamera.gameObject.SetActive(false);
                
                break;

            case false:
                MainCamera.gameObject.SetActive(true);
                AimCamera.gameObject.SetActive(AimActive);

                break;

        }
    }

    private void AimCameraRotation()
    {
        float MouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        X_Rotation -= MouseY;
        Y_Rotation += MouseX;
        X_Rotation=Mathf.Clamp(X_Rotation, Min_X_Rotation, Max_X_Rotation);
        Y_Rotation=Mathf.Clamp(Y_Rotation, Min_Y_Rotation, Max_Y_Rotation);

        AimCamera.transform.rotation = Quaternion.Euler(X_Rotation, Y_Rotation + 90, 0);
        FirePoint.rotation = Quaternion.Euler(X_Rotation, Y_Rotation + 90, 0);
    }

    private void AimTransition()
    {

    }
}
