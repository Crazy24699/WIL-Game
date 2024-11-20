using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class PlayerAttacks : MonoBehaviour
{
    public int Damage;
    public int AttackSpeed;

    //May need to remove this
    [SerializeField] protected bool IsAttacking = false;
    [SerializeField] protected bool ChannelAttack = false;
    [SerializeField] private bool CanAttack = true;
    private bool Moving = false;

    [Space(5)]
    public GameObject FreeLookCam;
    public GameObject TailSlashObject;
    public GameObject TailFirePoint;
    [SerializeField] private GameObject ClawSlashBox;
    [SerializeField] private GameObject BiteBox;

    [Space(5)]
    public Animator AttackAnimation;

    [Space(5)]
    //PLAYER INPUT ACTIONS
    public PlayerInput PlayerInputRef;
    protected InputAction MainAttack;
    protected InputAction SecondAttack;
    protected InputAction ThirdAttack;
    [Space(5)]

    public Cinemachine.CinemachineBrain CinemachineBrainScript;

    public PlayerMovement PlayerMoveScript;
    public CameraFunctionality CamFunctionScript;
    protected PlayerInteraction PlayerInteractionScript;

    public enum AllAttacks
    {
        None,
        SlashAttack,
        TailWhip,
        BiteAttack
    }

    public AllAttacks CurrentAttack;
    public AllAttacks NextAttack;
    protected Dictionary<AllAttacks, bool> AttacksInUse = new Dictionary<AllAttacks, bool>();

    public enum AttackTypes
    {
        Primary,
        Secondary,
        Third
    }


    [SerializeField]
    private void LockAttack()
    {
        AttackAnimation.SetBool("IsAttacking", true);
    }

    [SerializeField]
    private void UnlockAttack()
    {
        AttackAnimation.SetBool("IsAttacking", false);
        //Debug.Log("from above ");
    }

    // Start is called before the first frame update
    void Awake()
    {

        //PopulateAttacks();
        PopulateScripts();
        NextAttack = AllAttacks.None;
        CanAttack = true;

        CurrentAttack = AllAttacks.None;

        PlayerInputRef.Enable();

        SetActiveAttack(AllAttacks.SlashAttack, AttackTypes.Primary);
        SetClawState(2);

        SetActiveAttack(AllAttacks.TailWhip, AttackTypes.Secondary);
        SetTailAttackState(2);

        SetActiveAttack(AllAttacks.BiteAttack, AttackTypes.Third);
        SetBiteState(2);

        PlayerInputRef.PlayerAttack.CancelAttack.performed += Context => HandleAttackCancel();

    }

    private IEnumerator AttackCanceler()
    {
        HandleCameraState(true);
        HandleMovementState(true);
        yield return new WaitForSeconds(0.45f);
        CanAttack = true;

    }

    private void PopulateScripts()
    {
        PlayerInputRef = new PlayerInput();

        PlayerMoveScript = FindObjectOfType<PlayerMovement>();
        PlayerInteractionScript = FindObjectOfType<PlayerInteraction>();
        CamFunctionScript = FindObjectOfType<CameraFunctionality>();
    }
    private void Update()
    {
        IsAttacking = AttackAnimation.GetBool("IsAttacking");
    }
    private void HandleAttackCancel()
    {
        if(!ChannelAttack) { return; }
        Debug.Log("The doom");
        CamFunctionScript.ChangeActiveCamera(false);
        CanAttack = false;
        UnlockMovement();
        StartCoroutine(AttackCanceler());
        ChannelAttack = false;
    }

    private void OnEnable()
    {
        PlayerInputRef.Enable();
    }

    private void OnDisable()
    {
        PlayerInputRef.Disable();
    }

    private void FixedUpdate()
    {
        
        
        if(IsAttacking)
        {
            
        }

        //PlayerMoveScript.CanMove = !IsAttacking;
    }

    private void SetActiveAttack(AllAttacks ChosenAttack, AttackTypes AttackBind)
    {
        
        switch (AttackBind)
        {
            case AttackTypes.Primary:
                PlayerInputRef.PlayerAttack.PrimaryAttack.performed += Context => PerformAttack(ChosenAttack);
                MainAttack = PlayerInputRef.PlayerAttack.PrimaryAttack;
                break;

            case AttackTypes.Secondary:
                PlayerInputRef.PlayerAttack.SecondaryAttack.performed += Context => AimProjectile();
                PlayerInputRef.PlayerAttack.SecondaryAttack.performed += Context => SetChannelAttack(true);
                
                PlayerInputRef.PlayerAttack.SecondaryAttack.canceled += Context => SetChannelAttack(false);
                PlayerInputRef.PlayerAttack.SecondaryAttack.canceled += Context => PerformAttack(ChosenAttack);
                SecondAttack = PlayerInputRef.PlayerAttack.SecondaryAttack;
                break;

            case AttackTypes.Third:
                PlayerInputRef.PlayerAttack.ThirdAttack.performed += Context => PerformAttack(ChosenAttack);
                ThirdAttack = PlayerInputRef.PlayerAttack.ThirdAttack;
                break;
            default:
                break;
        }
    }
   
    private bool CheckChaining()
    {
        if ( NextAttack == AllAttacks.None && CurrentAttack != AllAttacks.None)
        {
            Debug.Log("Fast");
            //NextAttack = AllAttacks.TailWhip;
            return true;
        }
        if ( NextAttack == AllAttacks.None && CurrentAttack == AllAttacks.None)
        {

            return false;
        }
        if (NextAttack != AllAttacks.None)
        {
            //Debug.Log("Faster");
            return false;
        }
        return false;
    }

    private void SetChannelAttack(bool State)
    {
        ChannelAttack = State;  
    }

    private void PerformAttack(AllAttacks SetAttck)
    {
        if (!PlayerMoveScript.Grounded || !CanAttack || ChannelAttack)
        {
            return;
        }

        if (IsAttacking)
        {
            bool CanChain = CheckChaining();

            if(!CanChain)
            {
                //Debug.Log("Can not");
                return;
            }
            //Debug.Log("it can");
        }
        
        //Debug.Log("This ran;");
        PlayerInteractionScript.CanTakeDamage = false;
        switch (SetAttck)
        {
            default:
            case AllAttacks.SlashAttack:
                SlashAttackFunction();
                break;

            case AllAttacks.TailWhip:
                TailWhipFunction();
                break;

            case AllAttacks.BiteAttack:
                BiteAttackFunction();
                break;

        }
        if (CurrentAttack == AllAttacks.None)
        {
            CurrentAttack = SetAttck;
        }
        else
        {
            NextAttack = SetAttck;
        }

    }

    private void HandleAttackChaining()
    {
        if(NextAttack!=AllAttacks.None)
        {
            CurrentAttack = NextAttack;
            PerformAttack(NextAttack);
            NextAttack = AllAttacks.None;
            
        }
    }

    private void AimProjectile()
    {
        if (!PlayerMoveScript.Grounded) { return; }

        //Debug.Log("call our names in the vally of the saints");
        CamFunctionScript.ChangeActiveCamera(true);
        PlayerMoveScript.CanMove = false;
        PlayerMoveScript.AttackLocked = true;
        //CamFunctionScript.AimCameraActive = true;

    }

    private void SlashAttackFunction()
    {
        PlayAttackAnim("SlashAttackTrigger");
        HandleMovementState(false);
        //Debug.Log("rise");
    }

    private void TailWhipFunction()
    {
        PlayAttackAnim("TailWhip");

        HandleMovementState(false);
        HandleCameraState(false);
        CamFunctionScript.ChangeActiveCamera(false);
        //CamFunctionScript.AimCameraActive = false;
    }



    public void SpawnTailProjectile()
    {
        GameObject SpawnedSlash = Instantiate(TailSlashObject, TailFirePoint.transform.position, TailFirePoint.transform.rotation);
        SpawnedSlash.GetComponent<ProjectileBase>().LifeStartup(TailFirePoint.transform.forward, 200 * 3);
    }

    [SerializeField ] private void NotAttacking()
    {
        PlayerMoveScript.Attacking = false;
        AttackAnimation.SetBool("IsAttacking", false);
        PlayerMoveScript.AttackLocked = false;
    }

    private void BiteAttackFunction()
    {
        //IsAttacking = true;
        PlayAttackAnim("BiteAttack");
        HandleMovementState(false);
        //StartCoroutine(AttackBoxLifetime(2.5f, Attacks[0]));
    }

    protected void PlayAttackAnim(string AnimName)
    {

        AttackAnimation.SetTrigger(AnimName);
        //AttackAnimation.SetBool(nameof(IsAttacking), true);
        //StartCoroutine(AttackReset());
        ResetAttack();
    }

    public void ResetAttack()
    {
        //Debug.Log("Reseting ");
        PlayerInteractionScript.CanTakeDamage = true;
        HandleCameraState(true);
        HandleMovementState(true);
    }

    //This was used for testing, remove when done
    [SerializeField]
    private void BiteState(int ActiveState)
    {
        bool Active = ActiveState == 1 ? true : false;
        Debug.Log(Active + "  " + ActiveState);
    }

    protected void HandleMovementState(bool LockMovement)
    {
        PlayerMoveScript.CanMove = LockMovement;
        PlayerMoveScript.RigidbodyRef.velocity = Vector3.zero;
    }

    public void SetClawState(int ActiveState)
    {
        bool Active = ActiveState == 1 ? true : false;
        ClawSlashBox.SetActive(Active);
        //IsAttacking = Active;
        if (ActiveState == 2)
        {
            return;
        }
        if (!Active) { CurrentAttack = AllAttacks.None; HandleAttackChaining(); ResetAttack(); }
    }


    [SerializeField]
    private void SetBiteState(int ActiveState)
    {
        Debug.Log("burry");
        bool Active = ActiveState == 1 ? true : false;
        IsAttacking = Active;
        //Debug.Log(Active + "  " + ActiveState+"Bite state");
        BiteBox.SetActive(Active);
        if (ActiveState == 2)
        {
            return;
        }
        if (!Active) { CurrentAttack = AllAttacks.None; HandleAttackChaining(); ResetAttack(); }
    }



    [SerializeField]
    private void SetTailAttackState(int ActiveState)
    {
        bool Active = ActiveState == 1 ? true : false;
        //IsAttacking = Active;
        if (ActiveState == 2)
        {
            return;
        }
        if (!Active) { CurrentAttack = AllAttacks.None; HandleAttackChaining(); ResetAttack(); }
    }

    protected void HandleCameraState(bool LockCamera)
    {
        CinemachineBrainScript.enabled = LockCamera;
        CamFunctionScript.enabled = LockCamera;
        PlayerMoveScript.CanMove = LockCamera;
        //Debug.Log(LockCamera + "      " + "and burn;");
        FreeLookCam.SetActive(LockCamera);
    }

    [SerializeField]
    private void LockMovement()
    {
        PlayerMoveScript.CanMove = false;
    }

    [SerializeField]
    private void UnlockMovement()
    {
        PlayerMoveScript.CanMove = true;
        PlayerMoveScript.Attacking = false;
        PlayerMoveScript.AttackLocked = false;

    }

    public void SetDodgeInfo(Vector2 Direction)
    {
        AttackAnimation.SetFloat("XInput", Direction.y);
        AttackAnimation.SetFloat("YInput", Direction.x);
        AttackAnimation.SetTrigger("DodgeInput");
    }

    public void EnableTakeDamage()
    {
        PlayerInteractionScript.CanTakeDamage = true;
    }

    public void DisableTakeDamage()
    {
        PlayerInteractionScript.CanTakeDamage = false;
    }

    [SerializeField]
    private void PlayTailWhipSound()
    {
        Debug.Log("fuck fuck fuyck");
        PlayerInteractionScript.PlayAttackSounds("Tail Whip");
    }

    [SerializeField]
    private void PlayBiteSound()
    {
        PlayerInteractionScript.PlayAttackSounds("Bite");
    }
    [SerializeField]
    private void PlaySlashSound()
    {
        PlayerInteractionScript.PlayAttackSounds("Slash");
    }
}
