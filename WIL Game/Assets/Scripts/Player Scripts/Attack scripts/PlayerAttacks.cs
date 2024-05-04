using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    public string Name;

    public int Damage;
    public int AttackSpeed;

    public Animator AttackAnimation;

    public PlayerInput PlayerInputRef;
    protected InputAction InputRef;
    public PlayerMovement PlayerMoveScript;

    [SerializeField]protected bool IsAttacking = false;

    public enum AllAttacks
    {
        None,
        SlashAttack,
        TailWhip,
        Toppler
    }

    public AllAttacks CurrentAttack;
    public AllAttacks NextAttack;

    public SlashAttack SlashAttackScript;

    // Start is called before the first frame update
    void Awake()
    {
        NextAttack = AllAttacks.None;
        PlayerInputRef = new PlayerInput();

        SlashAttackScript = FindObjectOfType<SlashAttack>();
        PlayerMoveScript = FindObjectOfType<PlayerMovement>();

        CurrentAttack = AllAttacks.SlashAttack;

        PlayerInputRef.Enable();

        SetActiveAttack(AllAttacks.SlashAttack);
    }

    public void SetActiveAttack(AllAttacks SetAttck)
    {
        switch (SetAttck)
        {
            case AllAttacks.SlashAttack:
                PlayerInputRef.PlayerAttack.SlashAttack.performed += PerformAttack;
                break;
            case AllAttacks.TailWhip:
                break;
            case AllAttacks.Toppler:
                break;
            default:
                break;
        }
    }

    public void PerformAttack(InputAction.CallbackContext InputCallBack)
    {

        if (IsAttacking)
        {
            Debug.Log("Ocean");
            return;
        }
        switch (CurrentAttack)
        {
            default:
            case AllAttacks.SlashAttack:
                SlashAttackFunction();
                break;

            case AllAttacks.TailWhip:
                TailWhipFunction();
                break;

        }


    }

    public void SlashAttackFunction()
    {
        PlayAttackAnim(0.5f, "SlashAttack");
        IsAttacking = true;
    }

    public void TailWhipFunction()
    {
        PlayAttackAnim(0.85f, "SlashAttack");
    }

    protected void PlayAttackAnim(float ResetTime, string AnimName)
    {
        AttackAnimation.SetBool(AnimName, true);
        StartCoroutine(AttackReset(ResetTime, AnimName));
    }

    public IEnumerator AttackReset(float ResetTime, string AnimName)
    {
        yield return new WaitForSeconds(ResetTime);
        AttackAnimation.SetBool(AnimName, false);
        IsAttacking = false;

        if (NextAttack != AllAttacks.None) 
        {
            
        }
    }

    protected void HandleMovementState(bool LockMovement)
    {

    }

    protected void HandleCameraState(bool LockCamera)
    {

    }

}
