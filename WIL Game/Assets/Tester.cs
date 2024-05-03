using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Tester : MonoBehaviour
{
    public Rigidbody RigidBody;
    public Transform cam;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    Transform cameraObject;
    Rigidbody playerRigidbody;
    public float movementSpeed = 7;
    public float rotationSpeed = 15;
    Vector3 moveDirection;

    public InputActionReference PlayerActionMap;

    // Update is called once per frame
    void Update()
    {


        //HandleBaseMovement();

        //float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");
        //Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        //if (direction.magnitude >= 0.1f)
        //{
        //    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        //    transform.rotation = Quaternion.Euler(0f, angle, 0f);

        //    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        //    RigidBody.velocity = new Vector3(moveDir.x * speed, 0, moveDir.z * speed);

        //}


        HandleMovement();
        HandleRotation();


    }


    private void HandleMovement()
    {
        
        moveDirection = cameraObject.forward * PlayerActionMap.action.ReadValue<Vector3>().x;
        Vector3 Horizontal= new Vector3(PlayerActionMap.action.ReadValue<Vector3>().x, 0, PlayerActionMap.action.ReadValue<Vector3>().z);
        moveDirection = moveDirection + cameraObject.right * Horizontal.x;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection =moveDirection *movementSpeed;
        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;

    }

    public void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameraObject.forward * PlayerActionMap.action.ReadValue<Vector3>().y;
        targetDirection = targetDirection + cameraObject.right * PlayerActionMap.action.ReadValue<Vector3>().x;
        targetDirection.Normalize();
        targetDirection.y = 0;

    }

}

