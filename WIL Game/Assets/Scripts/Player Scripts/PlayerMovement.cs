using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody Rigidbody;
    public InputActionReference PlayerActionMap;

    
    public float MoveSpeed;
    public float JumpHeight;

    protected Vector3 MoveDirection;
    public Vector3 PlayerVelocity;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerVelocity=Rigidbody.velocity;
        if(PlayerActionMap.action.ReadValue<Vector3>().x != 0 || PlayerActionMap.action.ReadValue<Vector3>().y != 0)
        {
            
        }
        HandleBaseMovement();
    }

    public void HandleBaseMovement()
    {
        MoveDirection=PlayerActionMap.action.ReadValue<Vector3>();
        Rigidbody.velocity = new Vector3(MoveDirection.x * MoveSpeed, Rigidbody.velocity.y,MoveDirection.z * MoveSpeed);
    }
}
