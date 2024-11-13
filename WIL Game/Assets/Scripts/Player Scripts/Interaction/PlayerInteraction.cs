using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionReference ActionInput;

    public PlayerInput PlayerInputRef;
    protected InputAction InputRef;
    protected PlayerAttacks PlayerAttacks;
    protected Animator PlayerAnimator;

    public bool MenuActive;
    public bool CanTakeDamage = true;
    public bool InBlockerRange;
    public bool PoweredUp = false;

    public int MaxHealth;
    public int CurrentHealth;
    private int OldHealth;
    public int CurrentGemCount;
    public int CurrentShardCount;

    private WorldHandler WorldHandlerScript;
    private Slider PlayerHealthBar;
    [SerializeField]private TextMeshProUGUI ShardCounter;
    [SerializeField] private TextMeshProUGUI GemCounter;
    public ShardBlocker CurrentShardBlocker;
    private CameraFunctionality PlayerCamFunction;

    //public GameObject[] HeartImages;
    public GameObject PauseScreen;
    public GameObject DeathScreen;

    [SerializeField] private GameObject PlayerUI_Panel;
    [SerializeField] private GameObject PlayerMenu;
    [SerializeField] private GameObject PlayerStoryMenu;


    [SerializeField] private Sound[] PlayerSounds;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerCamFunction = this.transform.root.GetComponentInChildren<CameraFunctionality>();
        Cursor.lockState = CursorLockMode.Locked;
        MenuActive = false ;
        PlayerInputRef = new PlayerInput();

        CurrentHealth = MaxHealth;

        WorldHandlerScript = FindObjectOfType<WorldHandler>();
        PlayerAnimator = transform.GetComponentInChildren<Animator>();
        PlayerHealthBar = transform.Find("Player UI").GetComponentInChildren<Slider>();
        PlayerHealthBar.maxValue = MaxHealth;
        WorldHandlerScript.PlayerInteractionScript = this;
    }

    public void OnEnable()
    {
        InputRef = PlayerInputRef.PlayerInteraction.ShowMenu;
        InputRef.Enable();

        PlayerInputRef.PlayerInteraction.ShowMenu.performed += Context => ChangeMenu();
        PlayerInputRef.PlayerInteraction.ShowMenu.Enable();
        PlayerInputRef.StoryMenu.EndStory.performed += Context => EndStory();
    }

    public AudioClip HandleAudioClip(string Name, bool Stop)
    {
        bool SoundExists = PlayerSounds.Any(Snd => Snd.Name == Name);
        if (!SoundExists) { Debug.LogError("Sound doesnt exist"); return null; }
        

        switch (Stop)
        {
            case true:
                Sound SoundClip = PlayerSounds.FirstOrDefault(Snd => Snd.Name == Name);

                return SoundClip.SoundClip;

            case false:

                break;
        }
        return null;
    }

    public void HandleShardUpdate()
    {
        CurrentShardCount++;
        if( CurrentShardCount>= 3 )
        {
            CurrentShardCount -= 3;

            CurrentGemCount++;
            GemCounter.text = CurrentGemCount.ToString();
        }
        ShardCounter.text = CurrentShardCount.ToString() + "/3";
    }

    private void HandleGemUpdate()
    {
        if( CurrentGemCount <=0  ) { return; }
        CurrentShardBlocker.RemoveBlockLevel();
        CurrentGemCount--;
        GemCounter.text = CurrentGemCount.ToString();
    }

    public void OnDisable()
    {
        InputRef.Disable();
        PlayerInputRef.PlayerInteraction.ShowMenu.Disable();
    }

    public void ChangeMenu()
    {
        //if(WorldHandlerScript.CurrentMode!=WorldHandler.GameModes.Gameplay) { return; }
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

    private void EndStory()
    {
        WorldHandlerScript.CurrentMode = WorldHandler.GameModes.Gameplay;
        HandleStoryState(false);
        WorldHandlerScript.ModeChange.Invoke();
    }

    public void ActivateUIPanel()
    {
        PlayerStoryMenu.SetActive(true);
        PlayerUI_Panel.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayerInputRef.PlayerInteraction.ShowMenu.Enable();
        }
        DeathCheck();
        HandleInputTest();
        HandleEnvrionmentInteraction();
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

    private void HandleEnvrionmentInteraction()
    {
        if(!InBlockerRange) { return; }

        if(Input.GetKeyDown(KeyCode.E))
        {
            HandleGemUpdate();
        }

    }

    public void HandleStoryState(bool StoryModeActive)
    {
        switch (StoryModeActive)
        {
            case true:
                PlayerStoryMenu.SetActive(true);
                PlayerUI_Panel.SetActive(false);
                PlayerCamFunction.HandleCameraLockstate(true);
                
                PlayerInputRef.PlayerInteraction.ShowMenu.Disable();
                PlayerInputRef.StoryMenu.Enable();
                break;

            case false:
                PlayerCamFunction.HandleCameraLockstate(false) ;
                PlayerStoryMenu.SetActive(false);
                PlayerUI_Panel.SetActive(true);

                PlayerInputRef.StoryMenu.Disable();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                StartCoroutine(ReactivateWaitTime());
                //PlayerInputRef.PlayerInteraction.ShowMenu.Enable();
                break;
        }
    }

    private IEnumerator ReactivateWaitTime()
    {
        yield return new WaitForSeconds(0.25f);
        PlayerInputRef.PlayerInteraction.ShowMenu.Enable();
    }

    private void HandleInputTest()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            HandleHealth(-1);
        }
        
    }

    public void TakeHit(int DamageTaken)
    {
        //hit effects
        //hit sound
        //Applied Knockback
        //immunity timer
        HandleHealth(DamageTaken);
    }

    public void HandleHealth(int HealthChange)
    {
        if(!CanTakeDamage)
        {
            return;
        }
        if (HealthChange <= 0)
        {
            PlayerAnimator.SetTrigger("TakeHit");
        }
        Debug.Log("Damage");
        CurrentHealth += HealthChange;
        DeathCheck();
        HandleHealthChange();
    }

    private IEnumerator ImmunityTimer()
    {
        yield return new WaitForSeconds(2.75f);
    }

    private void HandleHealthChange()
    {
        PlayerHealthBar.value = CurrentHealth;
        return;
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
        if (Collision.CompareTag("BarrierBreakPowerup"))
        {
            PoweredUp = true;
        }
    }

}
