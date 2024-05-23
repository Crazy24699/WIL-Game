using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody Rigidbody;
    public InputActionReference PlayerActionMap;

    public Transform MainCamera; 

    public float BaseMoveSpeed;
    public float SpeedMultiplier;
    public float CurrentSpeed;

    protected float TurnSmoothingVel;
    public float TurnTime = 0.1f;

    public float JumpHeight;

    public Animator PlayerAnimations;

    public Vector3 PlayerVelocity;
    [SerializeField] protected Vector3 MoveDirection;

    public Transform PlayerOrientation;



    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        CurrentSpeed = 0;
    }

    private void FixedUpdate()
    {
        switch (CurrentSpeed)
        {
            case <= 0.001f:
                //PlayerAnimations.SetBool("Is Moving", false);
                break;

            case > 1:
                PlayerAnimations.SetBool("Is Moving", true);
                break;
        }
        MovePlayer();
    }

    protected void MovePlayer()
    {
        PlayerVelocity = new Vector3(Rigidbody.velocity.x, Rigidbody.velocity.y, Rigidbody.velocity.z);

        if (PlayerVelocity.magnitude > BaseMoveSpeed)
        {
            Vector3 VelocityCap = PlayerVelocity.normalized * BaseMoveSpeed;
            Rigidbody.velocity = new Vector3(VelocityCap.x, 0
                , VelocityCap.z);
        }

        MoveDirection = PlayerActionMap.action.ReadValue<Vector3>();
        MoveDirection = PlayerOrientation.forward * MoveDirection.z + PlayerOrientation.right * MoveDirection.x;

        Rigidbody.AddForce(new Vector3(MoveDirection.x, MoveDirection.y, MoveDirection.z) * BaseMoveSpeed * 10f, ForceMode.Force);
        CurrentSpeed = Rigidbody.velocity.magnitude;
    }

}
