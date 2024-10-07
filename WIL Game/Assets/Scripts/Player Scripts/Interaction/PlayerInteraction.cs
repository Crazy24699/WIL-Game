using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionReference ActionInput;

    public PlayerInput PlayerInputRef;
    protected InputAction InputRef;
    protected PlayerAttacks PlayerAttacks;

    public bool MenuActive;
    public bool CanTakeDamage = true;

    public int MaxHealth;
    public int CurrentHealth;
    private int OldHealth;

    private WorldHandler WorldHandlerScript;


    public GameObject[] HeartImages;
    public GameObject PauseScreen;
    public GameObject DeathScreen;

    // Start is called before the first frame update
    void Awake()
    {

        Cursor.lockState = CursorLockMode.Locked;
        MenuActive = false ;
        PlayerInputRef = new PlayerInput();

        CurrentHealth = MaxHealth;

        WorldHandlerScript = FindObjectOfType<WorldHandler>();

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

    public void ChangeMenu()
    {
        switch (MenuActive)
        {
            default:
            case false:
                Cursor.lockState = CursorLockMode.None;
                PauseScreen.SetActive(true);
                Time.timeScale = 0;
                MenuActive = true;
                break;


            case true:
                Cursor.lockState = CursorLockMode.Locked;
                PauseScreen.SetActive(false);
                Time.timeScale = 1;
                MenuActive = false;
                break;
        }
    }

    public void ChangeMenuState(InputAction.CallbackContext InputCallBack)
    {
        
        switch (MenuActive)
        {
            default:
            case false:
                Cursor.lockState = CursorLockMode.None;
                PauseScreen.SetActive(true);
                Time.timeScale = 0;
                MenuActive = true ;
                break;


            case true:
                Cursor.lockState = CursorLockMode.Locked;
                PauseScreen.SetActive(false);
                Time.timeScale = 1;

                MenuActive = false ;
                break;
        }
    }

    private void Update()
    {
        DeathCheck();
        HandleInputTest();
    }

    private void DeathCheck()
    {
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            DeathScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
    }

    private void HandleInputTest()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            HandleHealth(-1);
        }
    }

    public void HandleHealth(int HealthChange)
    {
        if(!CanTakeDamage)
        {
            return;
        }
        Debug.Log("Damaghe");
        CurrentHealth += HealthChange;
        DeathCheck();
        //HandleHeartChanges();
    }

    private void HandleHeartChanges()
    {
        if (HeartImages == null)
        {
            return;
        }

        if (OldHealth < CurrentHealth)
        {
            HeartImages[CurrentHealth].SetActive(false);
        }
        if(OldHealth> CurrentHealth)
        {
            HeartImages[CurrentHealth].SetActive(true);

        }
    }

    private void OnTriggerEnter(Collider Collision)
    {

        if (Collision.CompareTag("NextArea"))
        {
            if (Collision.name.Equals("Boss 1 Area"))
            {
                WorldHandlerScript.SetActiveArea("Boss 1 Area");
            }
            if (Collision.name.Equals("Final Boss Area"))
            {
                WorldHandlerScript.SetActiveArea("Final Boss Area");
            }
        }
    }

}
