using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionReference ActionInput;

    public PlayerInput PlayerInputRef;
    protected InputAction InputRef;

    public bool MenuActive;

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        MenuActive = false ;
        PlayerInputRef = new PlayerInput();
    }

    public void OnEnable()
    {
        InputRef = PlayerInputRef.PlayerInteraction.ShowMenu;
        InputRef.Enable();

        PlayerInputRef.PlayerInteraction.ShowMenu.Enable();
        PlayerInputRef.PlayerInteraction.ShowMenu.performed += ChangeMenuState;

    }

    public void OnDisable()
    {
        InputRef.Disable();
        PlayerInputRef.PlayerInteraction.ShowMenu.Disable();
    }

    public void ChangeMenuState(InputAction.CallbackContext InputCallBack)
    {
        
        switch (MenuActive)
        {
            default:
            case false:
                Cursor.lockState = CursorLockMode.None;
                MenuActive = true ;
                break;


            case true:
                Cursor.lockState = CursorLockMode.Locked;
                MenuActive = false ;
                break;
        }
    }


}
