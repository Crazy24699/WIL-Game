using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class PlayerAttacks : MonoBehaviour
{
    public string Name;

    public int Damage;
    public int AttackSpeed;

    private bool AttackOneActive;
    private bool AttackTwoActive;
    private bool AttackThreeActive;
    [SerializeField] protected bool IsAttacking = false;

    [Space(5)]
    public GameObject FreeLookCam;
    public GameObject TailSlashObject;
    public GameObject TailFirePoint;

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
        SetActiveAttack(AllAttacks.TailWhip, AttackTypes.Secondary);
        SetActiveAttack(AllAttacks.BiteAttack, AttackTypes.Third);
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
        AttackAnimation.SetBool("IsAttacking", IsAttacking);
    }

    public void PerformAttack(AllAttacks SetAttck)
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

    private void SlashAttackFunction()
    {
        PlayAttackAnim("SlashAttackTrigger");
        IsAttacking = true;
        Debug.Log("rise");
    }

    private void TailWhipFunction()
    {
        PlayAttackAnim("TailWhip");
        Debug.Log("We");
        HandleMovementState(false);
        HandleCameraState(false);
        GameObject SpawnedSlash = Instantiate(TailSlashObject, TailFirePoint.transform.position, TailFirePoint.transform.rotation);
        SpawnedSlash.GetComponent<ProjectileBase>().LifeStartup(TailFirePoint.transform.forward, 200);
        IsAttacking = true;
    }


    private void BiteAttackFunction()
    {
        PlayAttackAnim("BiteAttack");
        HandleMovementState(false);
        StartCoroutine(AttackBoxLifetime(2.5f, Attacks[0]));
        IsAttacking = true;
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

        StartCoroutine(AttackReset());
    }

    public IEnumerator AttackReset()
    {
        yield return new WaitForSeconds(0.2f);
        if (NextAttack != AllAttacks.None) 
        {
            CurrentAttack = NextAttack;
            PerformAttack(CurrentAttack);
            NextAttack = AllAttacks.None;
            Debug.Log("None");
            yield return null;
        }
        yield return new WaitForSeconds(1.75f);
        IsAttacking = false;
        PlayerInteractionScript.CanTakeDamage = true;
        HandleCameraState(true);
        HandleMovementState(true);
    }

    protected void HandleMovementState(bool LockMovement)
    {
        //PlayerMoveScript.enabled = LockMovement;
        //PlayerMoveScript.Rigidbody.velocity = Vector3.zero;
    }

    protected void HandleCameraState(bool LockCamera)
    {
        CinemachineBrainScript.enabled = LockCamera;
        CamFunctionScript.enabled = LockCamera;
        FreeLookCam.SetActive(LockCamera);
    }

}
