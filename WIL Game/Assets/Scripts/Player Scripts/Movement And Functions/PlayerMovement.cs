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
    public GameObject HitParticle;


    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        CurrentSpeed = 0;
    }

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.L))
        {
            Instantiate(HitParticle, transform.position, Quaternion.identity);
        }
        if (PlayerAnimations.GetBool("IsAttacking"))
        {
            Rigidbody.velocity = Vector3.zero;
            Debug.Log("laced with poison");
            return;
        }
        MovePlayer();
        switch (MoveDirection.magnitude)
        {
            case <= 0.1f:
                PlayerAnimations.SetBool("Is Moving", false);
                break;

            case >= 1:
                PlayerAnimations.SetBool("Is Moving", true);
                break;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SpeedMultiplier = 2.5f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            SpeedMultiplier = 0;
        }
    }

    protected void MovePlayer()
    {
        PlayerVelocity = new Vector3(Rigidbody.velocity.x, Rigidbody.velocity.y, Rigidbody.velocity.z);
        MoveDirection = PlayerActionMap.action.ReadValue<Vector3>();

        if(MoveDirection.magnitude <= 0.1f)
        {
            Rigidbody.velocity = Vector3.zero;
            return;
        }

        if (PlayerVelocity.magnitude > BaseMoveSpeed) 
        {
            Vector3 VelocityCap = PlayerVelocity.normalized * BaseMoveSpeed;
            Rigidbody.velocity = new Vector3(VelocityCap.x, 0
                , VelocityCap.z);
        }

        MoveDirection = PlayerOrientation.forward * MoveDirection.z + PlayerOrientation.right * MoveDirection.x;

        Rigidbody.AddForce(new Vector3(MoveDirection.x, MoveDirection.y, MoveDirection.z) * (BaseMoveSpeed+SpeedMultiplier) * 10f, ForceMode.Force);
        CurrentSpeed = Rigidbody.velocity.magnitude;
    }

}
