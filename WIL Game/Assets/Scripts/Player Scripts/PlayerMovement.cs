using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody Rigidbody;
    public InputActionReference PlayerActionMap;

    public Camera MainCamera; 

    public float BaseMoveSpeed;
    public float SpeedMultiplier;
    public float CurrentSpeed;

    protected float TurnSmoothingVel;
    public float TurnTime = 0.1f;

    public float JumpHeight;

    protected Vector3 MoveDirection;
    public Vector3 PlayerVelocity;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        CurrentSpeed = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if(PlayerActionMap.action.ReadValue<Vector3>().x != 0 || PlayerActionMap.action.ReadValue<Vector3>().y != 0)
        {
            
        }

        HandleBaseMovement();


        if (MoveDirection.magnitude >= 0.1f)
        {


        }

    }

    public void HandleBaseMovement()
    {
        MoveDirection=PlayerActionMap.action.ReadValue<Vector3>();

        PlayerVelocity = Rigidbody.velocity;

        Vector3 Direction = new Vector3(MoveDirection.x, 0, MoveDirection.z).normalized;
        float NewAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + MainCamera.transform.eulerAngles.y;

        if (Direction.magnitude >= 0.1f)
        {
            CurrentSpeed = 10;
            float Angel = Mathf.SmoothDampAngle(transform.eulerAngles.y, NewAngle, ref TurnSmoothingVel, TurnTime);
            transform.rotation = Quaternion.Euler(0, Angel, 0);

        }
        else
        {
            CurrentSpeed = 0;
        }
        Vector3 LookDirection = Quaternion.Euler(0, NewAngle, 0) * Vector3.forward;

        Rigidbody.velocity = new Vector3(LookDirection.x * CurrentSpeed, 0 ,LookDirection.z * CurrentSpeed);
    }
}
