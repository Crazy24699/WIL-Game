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

    public AttackBase[] Attacks;
    public PlayerMovement PlayerMoveScript;
    public Cinemachine.CinemachineBrain CinemachineBrainScript;
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
    }

    // Start is called before the first frame update
    void Awake()
    {
        PopulateAttacks();

        NextAttack = AllAttacks.None;
        PlayerInputRef = new PlayerInput();

        PlayerMoveScript = FindObjectOfType<PlayerMovement>();

        CurrentAttack = AllAttacks.SlashAttack;

        PlayerInputRef.Enable();

        PlayerInteractionScript = FindObjectOfType<PlayerInteraction>();

        SetActiveAttack(AllAttacks.SlashAttack,AttackTypes.Primary);
        DeactivateClawAttack();
        SetActiveAttack(AllAttacks.TailWhip, AttackTypes.Secondary);

        SetActiveAttack(AllAttacks.BiteAttack, AttackTypes.Third);
        DeactivateBite();
    }

    private void OnEnable()
    {
        PlayerInputRef.Enable();
    }

    private void OnDisable()
    {
        PlayerInputRef.Disable();
    }

    private void PopulateAttacks()
    {
        foreach (AllAttacks Attack in System.Enum.GetValues(typeof(AllAttacks)))
        {
            if (!Attack.Equals(AllAttacks.None))
            {
                AttacksInUse.Add(Attack, false);
            }
        }
    }

    public void SetActiveAttack(AllAttacks SetAttck, AttackTypes ChosenAttack)
    {

        if (AttacksInUse[SetAttck])
        {
            //Idicator on screen to show that the attack cant be used
            return;
        }

        //PlayerInputRef.PlayerAttack.PrimaryAttack.RemoveAction();
        switch (ChosenAttack)
        {
            case AttackTypes.Primary:
                PlayerInputRef.PlayerAttack.PrimaryAttack.performed += Context => PerformAttack(SetAttck);
                MainAttack = PlayerInputRef.PlayerAttack.PrimaryAttack;
                break;

            case AttackTypes.Secondary:
                PlayerInputRef.PlayerAttack.SecondaryAttack.performed += Context => PerformAttack(SetAttck);
                SecondAttack = PlayerInputRef.PlayerAttack.SecondaryAttack;
                break;

            case AttackTypes.Third:
                PlayerInputRef.PlayerAttack.ThirdAttack.performed += Context => PerformAttack(SetAttck);
                ThirdAttack = PlayerInputRef.PlayerAttack.ThirdAttack;
                break;
            default:
                break;
        }
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Victorious");
            //SetActiveAttack(AllAttacks.TailWhip, AttackTypes.Primary);
        }
        IsAttacking = AttackAnimation.GetBool("IsAttacking");
        switch (ProgramManager.ProgramManagerInstance.GamePaused)
        {
            case true:
                PlayerInputRef.Disable();
                break;

            case false:
                PlayerInputRef.Enable();
                break;
        }
    }

    private void PerformAttack(AllAttacks SetAttck)
    {

        if (IsAttacking && NextAttack == AllAttacks.None) 
        {
            Debug.Log("Ocean");
            NextAttack = SetAttck;
            return;
        }
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
        

    }

    #region PLAYER ATTACK TESTING
    public void TailWhipAttack()
    {
        //TailWhipFunction();
    }

    public void SlashAttack()
    {
        //SlashAttackFunction();
    }

    public void BiteAttack()
    {
        //BiteAttackFunction();
    }

    #endregion


    private void SlashAttackFunction()
    {
        PlayAttackAnim("SlashAttackTrigger");
        HandleMovementState(false);
        Debug.Log("rise");
    }

    private void TailWhipFunction()
    {
        PlayAttackAnim("TailWhip");

        HandleMovementState(false);
        HandleCameraState(false);
    }

    public void SpawnTailProjectile()
    {
        GameObject SpawnedSlash = Instantiate(TailSlashObject, TailFirePoint.transform.position, transform.rotation);
        SpawnedSlash.GetComponent<ProjectileBase>().LifeStartup(TailFirePoint.transform.forward, 200*3);
    }

    private void BiteAttackFunction()
    {
        PlayAttackAnim("BiteAttack");
        HandleMovementState(false);
        StartCoroutine(AttackBoxLifetime(2.5f, Attacks[0]));
    }

    private IEnumerator AttackBoxLifetime(float LifeTime, AttackBase AttackScript)
    {
        AttackScript.enabled = true;
        yield return new WaitForSeconds(LifeTime);
        AttackScript.enabled = false;
    }

    protected void PlayAttackAnim(string AnimName)
    {
        AttackAnimation.SetTrigger(AnimName);

        //StartCoroutine(AttackReset());
        ResetAttack();
    }

    public IEnumerator AttackReset()
    {
        yield return new WaitForSeconds(0.2f);
        if (NextAttack != AllAttacks.None) 
        {
            CurrentAttack = NextAttack;
            NextAttack = AllAttacks.None;
            PerformAttack(CurrentAttack);
            Debug.Log("None");
            yield return null;
        }
        yield return new WaitForSeconds(1.75f);


    }

    public void ResetAttack()
    {
        PlayerInteractionScript.CanTakeDamage = true;
        HandleCameraState(true);
        HandleMovementState(true);
    }

    #region ClawAttack
    public void ActivateClawAttack()
    {
        ClawSlashBox.SetActive(true);
    }

    public void DeactivateClawAttack()
    {
        ClawSlashBox.SetActive(false);
    }
    #endregion
    
    #region Bite Attack
    public void ActivateBite()
    {
        BiteBox.SetActive(true);
    }

    public void DeactivateBite()
    {
        BiteBox.SetActive(false);
    }
    #endregion



    protected void HandleMovementState(bool LockMovement)
    {
        PlayerMoveScript.enabled = LockMovement;
        PlayerMoveScript.Rigidbody.velocity = Vector3.zero;
    }

    protected void HandleCameraState(bool LockCamera)
    {
        CinemachineBrainScript.enabled = LockCamera;
        CamFunctionScript.enabled = LockCamera;
        FreeLookCam.SetActive(LockCamera);
    }

}
