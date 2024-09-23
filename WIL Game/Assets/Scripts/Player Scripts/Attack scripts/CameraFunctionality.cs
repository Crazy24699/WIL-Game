using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraFunctionality : MonoBehaviour
{
    public InputActionReference PlayerActionMap;

    public Transform MainCamera;

    public GameObject CameraLock;

    public bool LockView = false;

    public float FinalMoveSpeed;
    public float CurrentMoveSpeed;
    public float Incrimenter;
    public float RotationSpeed;
    float LockoutTime;
    float CurrentLockoutTime;

    public Transform PlayerOrientation;
    public Transform Player;
    public Transform PlayerObject;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMovementScript.AttackLocked || !PlayerMovementScript.CanMove) { return; }

        RotateToView();
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

    public void RotateToView()
    {
        ViewDirection = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        PlayerOrientation.forward = ViewDirection.normalized;

        MoveDirection = PlayerActionMap.action.ReadValue<Vector3>();
        MoveDirection = new Vector3(MoveDirection.z, 0, MoveDirection.x * -1);
        Vector3 InputDirection = PlayerOrientation.right * MoveDirection.x + PlayerOrientation.forward * MoveDirection.z;

        if (InputDirection != Vector3.zero)
        {
            PlayerObject.forward = Vector3.Slerp(PlayerObject.forward, InputDirection.normalized, Time.deltaTime * RotationSpeed);

        }

    }

    protected void ChangeCamLockState(bool LockState)
    {

    }

}
