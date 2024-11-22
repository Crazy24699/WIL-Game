using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    private PlayerMovement PlayerMoveScript;
    public TextMeshProUGUI StoryText;
    public EndGame EndGameScript;

    public bool MenuActive;
    public bool CanTakeDamage = true;
    public bool InBlockerRange;
    public bool PoweredUp = false;
    public bool AtEnd = false;

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
    public GameObject HitParticle;
    public Image EndScreen;

    [SerializeField] private GameObject PlayerUI_Panel;
    [SerializeField] private GameObject PlayerMenu;
    [SerializeField] private GameObject PlayerStoryMenu;
    public GameObject EndStoryPanel;
    public TextMeshProUGUI EndStoryText;


    [SerializeField] public Sound[] PlayerSounds;
    [SerializeField] private AudioSource PlayerAudioSource;


    // Start is called before the first frame update
    void Awake()
    {
        PlayerMoveScript = GetComponent<PlayerMovement>();

        PlayerCamFunction = this.transform.root.GetComponentInChildren<CameraFunctionality>();

        PlayerAudioSource = this.GetComponent<AudioSource>();
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
        PlayerInputRef.StoryMenu.EndStory.performed += Context => ExitStory();

        PlayerInputRef.PlayerAttack.Healing.performed += Context => Heal();
        PlayerInputRef.PlayerAttack.Healing.Enable();
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

    public void PlayAttackSounds(string SoundEffectName)
    {
        bool SoundExists = PlayerSounds.Any(Snd => Snd.Name == SoundEffectName);
        if (!SoundExists) { Debug.LogError("Sound doesnt exist"); return; }

        Sound SoundClip = PlayerSounds.FirstOrDefault(Snd => Snd.Name == SoundEffectName);
        PlayerAudioSource.clip = SoundClip.SoundClip;


        PlayerAudioSource.Play();
    }



    public void HandleShardUpdate()
    {
        CurrentShardCount++;
        ShardCounter.text = CurrentShardCount.ToString() + "/3";
    }

    private void DecrimentShard()
    {
        if (CurrentShardCount<= 0) { return; }
        if (!InBlockerRange) { return; }
        CurrentShardBlocker.RemoveBlockLevel();
        CurrentShardCount--;
        ShardCounter.text = CurrentGemCount.ToString();
    }
    public void PlayHit(Vector3 ParticlePosition)
    {
        Instantiate(HitParticle, ParticlePosition, Quaternion.identity);
    }

    public void IncrimentGemCount()
    {
        CurrentGemCount++;
    }

    private void HandleGemUpdate()
    {
        if( CurrentGemCount <=0  ) { return; }
        if (!InBlockerRange) { return; }
        CurrentShardBlocker.RemoveBlockLevel();
        CurrentGemCount--;
        GemCounter.text = CurrentGemCount.ToString();
    }

    private void OnDestroy()
    {
        PlayerInputRef.PlayerInteraction.ShowMenu.performed -= Context => ChangeMenu();
        PlayerInputRef.PlayerInteraction.ShowMenu.Disable();

        PlayerInputRef.StoryMenu.EndStory.performed -= Context => ExitStory();
        PlayerInputRef.StoryMenu.EndStory.Disable();

        PlayerInputRef.PlayerAttack.Healing.performed -= Context => Heal();
        PlayerInputRef.PlayerAttack.Healing.Disable();
    }

    public void OnDisable()
    {
        InputRef.Disable();
        PlayerInputRef.PlayerInteraction.ShowMenu.Disable();

        PlayerInputRef.PlayerInteraction.ShowMenu.performed -= Context => ChangeMenu();
        PlayerInputRef.PlayerInteraction.ShowMenu.Disable();

        PlayerInputRef.StoryMenu.EndStory.performed -= Context => ExitStory();
        PlayerInputRef.StoryMenu.EndStory.Disable();

        PlayerInputRef.PlayerAttack.Healing.performed -= Context => Heal();
        PlayerInputRef.PlayerAttack.Healing.Disable();
    }

    private void DisableInput()
    {
        InputRef.Disable();
        PlayerInputRef.PlayerInteraction.ShowMenu.Disable();

        PlayerInputRef.PlayerAttack.Disable();

        PlayerInputRef.PlayerInteraction.ShowMenu.performed -= Context => ChangeMenu();
        PlayerInputRef.PlayerInteraction.ShowMenu.Disable();

        PlayerInputRef.StoryMenu.EndStory.performed -= Context => ExitStory();
        PlayerInputRef.StoryMenu.EndStory.Disable();

        PlayerInputRef.PlayerAttack.Healing.performed -= Context => Heal();
        PlayerInputRef.PlayerAttack.Healing.Disable();
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

    public void ExitStory()
    {
        if(AtEnd) { return; }
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
                Cursor.visible = true;
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
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    PlayerInputRef.PlayerInteraction.ShowMenu.Enable();
        //}
        DeathCheck();
        HandleInputTest();
        HandleEnvrionmentInteraction();

        if (PlayerStoryMenu.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {

            CurrentHealth = 5000;
            CurrentShardCount = 5000;
            

        }

    }

    public void StoryEnd()
    {
        AtEnd = true;
        PlayerMoveScript.CurrentSpeed = 0;
        PlayerMoveScript.PlayerVelocity = Vector3.zero;
        PlayerMoveScript.StopMoving();
        PlayerAnimator.SetBool("Is Moving", false);
        EndStoryPanel.SetActive(true);
        PlayerMoveScript.StopMoving();


        PlayerMoveScript.CanMove = false;
        WorldHandlerScript.ChangeInputMode(WorldHandler.GameModes.Story);
        Cursor.lockState = CursorLockMode.None ;

        PlayerHealthBar.transform.parent.gameObject.SetActive(false);
        DisableInput();
        PlayerHealthBar.enabled = false;
        ShardCounter.enabled = false;
        PlayerMoveScript.StopMoving();


        PlayerAnimator.SetTrigger("Game Finished");

    }

    public void DarkenScreen()
    {
        EndScreen.color=new Color(0,0,0,EndScreen.color.a+0.17f);
    }



    public void NextText()
    {
        EndGameScript.UpdateStoryText();
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
            //HandleGemUpdate();
            DecrimentShard();
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

    public void TakeHit(int DamageTaken, Vector3 ObjectHitPosition)
    {
        if (!CanTakeDamage) { return; }
        //hit effects
        //hit sound
        //Applied Knockback
        ObjectHitPosition.y = this.transform.position.y;
        Vector3 Direction = (transform.position - ObjectHitPosition).normalized;
        Direction.y = 0;
        Debug.DrawRay(ObjectHitPosition, Direction*20,Color.blue,300.0f);

        HandleHealth(DamageTaken);
        StartCoroutine(HandleStunLock());
        StartCoroutine(ImmunityTimer());
        PlayerMoveScript.ApplyKnockback(Direction);
        
    }

    private void Heal()
    {
        if (CurrentShardCount <= 0 || CurrentHealth>=MaxHealth || InBlockerRange) { return; }
        CurrentHealth += 10;
        if(CurrentHealth >= MaxHealth) {  CurrentHealth = MaxHealth; }
        HandleHealthChange();
        CurrentShardCount--;

    }

    public void HandleHealth(int HealthChange)
    {
        if (!CanTakeDamage) { return; }
        Debug.Log(HealthChange);
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
        CanTakeDamage = false;
        yield return new WaitForSeconds(1.75f);
        CanTakeDamage = true;
    }

    private IEnumerator HandleStunLock()
    {
        PlayerMoveScript.StunLocked = true;
        yield return new WaitForSeconds(0.35f);
        PlayerMoveScript.StunLocked = false;
    }

    private void FixedUpdate()
    {
        HandleHealthChange();
        ShardCounter.text = CurrentShardCount.ToString() + "/3";
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
            Destroy(Collision.gameObject);
        }

        if(Collision.CompareTag("Game End"))
        {
            EndGameScript = Collision.GetComponent<EndGame>();
        }

    }

}
