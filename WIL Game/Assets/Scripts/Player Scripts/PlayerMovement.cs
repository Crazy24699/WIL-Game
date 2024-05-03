using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

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

    [SerializeField]protected Vector3 MoveDirection;
    public Vector3 PlayerVelocity;


    //public CharacterController controller;
    //public Transform cam;
    //public float speed = 6f;
    //public float turnSmoothTime = 0.1f;
    //float turnSmoothVelocity;



    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        CurrentSpeed = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (PlayerActionMap.action.ReadValue<Vector3>().x != 0 || PlayerActionMap.action.ReadValue<Vector3>().y != 0)
        {

        }

        HandleBaseMovement();

        //float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");
        //Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        //if (direction.magnitude >= 0.1f)
        //{
        //    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        //    transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        //    controller.Move(moveDir.normalized * speed * Time.deltaTime);

        //}
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
        LookDirection = LookDirection.normalized;

        Rigidbody.velocity = new Vector3(LookDirection.x * CurrentSpeed, 0 ,LookDirection.z * CurrentSpeed);
    }
}
