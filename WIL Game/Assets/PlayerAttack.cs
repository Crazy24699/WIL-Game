
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public PlayerInput PlayerInputRef;
    protected InputAction InputRef;

    protected bool IsAttacking = false;


    // Start is called before the first frame update
    void Awake()
    {
        PlayerInputRef=new PlayerInput();


    }

    public void OnEnable()
    {
        InputRef = PlayerInputRef.PlayerAttack.ClawSlash;
        InputRef.Enable();
        PlayerInputRef.PlayerAttack.ClawSlash.Enable();

    }

    public void OnDisable()
    {
        InputRef.Disable();
        PlayerInputRef.PlayerAttack.Disable();
    }

    public void Attack(InputAction.CallbackContext InputCallBack)
    {

    }

    public void ClawAttack()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}