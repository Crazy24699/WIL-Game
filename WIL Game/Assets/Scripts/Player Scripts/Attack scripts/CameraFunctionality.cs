using Cinemachine;
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
    private bool AimInfoSet = false;
    public bool AimCameraActive = false;

    public float RotationSpeed;
    [SerializeField] private float MouseSensitivity = 100f;
    float LockoutTime;
    float CurrentLockoutTime;
    [SerializeField] private float X_Rotation;
    [SerializeField] private float Y_Rotation=180;
    [SerializeField] private AimMouse[] MouseAxisInfo;

    public Transform PlayerOrientation;
    public Transform Player;
    public Transform PlayerObject;

    public Transform FirePoint;
    public Transform MainCamera;
    public Transform FreeLookCamera;

    [SerializeField] private Transform AimCamera;
    [SerializeField] private Transform AimCamRotator;

    public float FrozeCam_X_Value;
    public float FrozeCam_Y_Value;

    public Vector3 ViewDirection;
    public Vector3 CameraView;
    public Vector3 PlayerVelocity;
    public Vector3 TransformDirection;
    [SerializeField] protected Vector3 MoveDirection;

    [SerializeField] private PlayerMovement PlayerMovementScript;
    public Cinemachine.CinemachineBrain Brain;
    public Cinemachine.CinemachineFreeLook FreeLockCamRef;

    // Start is called before the first frame update
    void Start()
    {
        CurrentLockoutTime = LockoutTime;

        PlayerMovementScript=transform.root.root.GetComponent<PlayerMovement>();
        CameraActive = true;
        MouseSensitivity = 120f;
        FreeLockCamRef = GetComponentInChildren<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            //FreeLockCamRef.enabled = !FreeLockCamRef.isActiveAndEnabled;
            //Brain.enabled = !Brain.isActiveAndEnabled;
            //FrozeCam_X_Value = FreeLockCamRef.m_XAxis.Value;
            HandleCameraLockstate(!LockView);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            FreeLockCamRef.m_XAxis.Value = FrozeCam_X_Value;
        }
        HandleAimCamera();

        //LockCamera();
        if (Input.GetKeyDown(KeyCode.U))
        {
            return;
            switch (LockView)
            {
                case false:
                    CameraView = MainCamera.transform.position;
                    LockView = true;
                    break;

                case true:
                    LockView = false;
                    break;
            }
        }


        if (PlayerMovementScript.Attacking || !PlayerMovementScript.CanMove || !CameraActive) { return; }

        RotateToView();
        HandleLockout();

    }

    

    public void HandleCameraLockstate(bool LockState)
    {
        LockView = LockState;
        if (LockView)
        {
            Brain.enabled = !LockView;

            FrozeCam_X_Value = FreeLockCamRef.m_XAxis.Value;
            FrozeCam_Y_Value = FreeLockCamRef.m_YAxis.Value;
            FreeLockCamRef.enabled = !LockView;
            return;
        }
        FreeLockCamRef.m_XAxis.m_InputAxisValue = 0;
        FreeLockCamRef.m_YAxis.m_InputAxisValue = 0;


        FreeLockCamRef.m_XAxis.Value = FrozeCam_X_Value;
        FreeLockCamRef.m_YAxis.Value = FrozeCam_Y_Value;

        StartCoroutine(RetakeDelay());


    }

    private IEnumerator RetakeDelay()
    {
        yield return new WaitForSeconds(0.75f);
        Brain.enabled = !LockView;
        FreeLockCamRef.enabled = !LockView;
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
                
            }
        }
        else if (!LockView)
        {
            CurrentLockoutTime = LockoutTime;
        }
    }

    private void HandleAimCamera()
    {
        //HandleCameraTransformValue();
        if(!CameraActive || PlayerMovementScript.Attacking) { return; }
        switch (AimCameraActive)
        {
            case true:
                AimInfoSet = false;
                AimCamera.transform.localRotation = Quaternion.Euler(0,0,0);
                AimCameraRotation();
                //HandleCameraTransformValue();
                break;

            case false:
                AimCamRotator.rotation = Quaternion.LookRotation(PlayerMovementScript.BaltoOrientation.right);
                X_Rotation = 0;
                Y_Rotation = 0;
                //AimCamera.localRotation = Quaternion.Euler(transform.parent.forward);
                //X_Rotation = PlayerMovementScript.BaltoOrientation.eulerAngles.x;
                //Y_Rotation = PlayerMovementScript.BaltoOrientation.eulerAngles.y;
                //Debug.Log("Hellish verses, at the alter we start to pray");
                break;

        }

    }

    private void HandleCameraTransformValue()
    {
        
        float X_Rotation=Mathf.Repeat(AimCamera.transform.localEulerAngles.x, 360);
        float Y_Rotation=Mathf.Repeat(AimCamera.transform.localEulerAngles.y, 360);
        float Z_Rotation=Mathf.Repeat(AimCamera.transform.localEulerAngles.z, 360);
        Quaternion UpdatedTransform = Quaternion.Euler(X_Rotation, Y_Rotation, Z_Rotation);
        AimCamera.transform.rotation = UpdatedTransform;
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
        

        if (!AimInfoSet)
        {
            PopulateNewAimInfo();
        }
        float MouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        X_Rotation -= MouseY;
        Y_Rotation += MouseX;
        X_Rotation = Mathf.Clamp(X_Rotation, -30, 30);
        Y_Rotation = Mathf.Clamp(Y_Rotation, -30, 30);

        AimCamera.transform.localRotation = Quaternion.Euler(X_Rotation, Y_Rotation, 0);
        FirePoint.localRotation = Quaternion.Euler(X_Rotation, Y_Rotation-90, 0);
    }

    private void PopulateNewAimInfo()
    {
        MouseAxisInfo[0].BaseRotation = Mathf.Repeat(AimCamera.transform.eulerAngles.x, 360);
        MouseAxisInfo[1].BaseRotation = Mathf.Repeat(AimCamera.transform.eulerAngles.y, 360);

        AimInfoSet = true;
    }

    private void AimTransition()
    {

    }

    [System.Serializable]
    private class AimMouse
    {
        public float BaseRotation;
        public float NewMinRotation;
        public float NewMaxRotation;

        public void HandleClamping()
        {

        }
    }
}


