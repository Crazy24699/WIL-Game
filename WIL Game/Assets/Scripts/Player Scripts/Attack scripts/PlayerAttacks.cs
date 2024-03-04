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

    public List<PlayerAttacks> UnlockedAttacks;

    public PlayerInput PlayerInputRef;
    protected InputAction InputRef;

    protected bool IsAttacking = false;

    public enum AllAttacks
    {
        SlashAttack,
        TailWhip,
        Toppler
    }

    public AllAttacks CurrentAttack;

    public SlashAttack SlashAttackScript;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerInputRef = new PlayerInput();
        SlashAttackScript = FindObjectOfType<SlashAttack>();
        CurrentAttack = AllAttacks.SlashAttack;
    }

    public void OnEnable()
    {
        InputRef = PlayerInputRef.PlayerAttack.SlashAttack;
        InputRef.Enable();
        PlayerInputRef.PlayerAttack.SlashAttack.Enable();

        PlayerInputRef.PlayerAttack.SlashAttack.performed += PerformAttack;

    }

    public void OnDisable()
    {
        InputRef.Disable();
        PlayerInputRef.PlayerAttack.Disable();
    }

    public void PerformAttack(InputAction.CallbackContext InputCallBack)
    {
        switch (CurrentAttack)
        {
            default:
            case AllAttacks.SlashAttack:
                SlashAttackFunction();
                break;

        }
    }

    public void SlashAttackFunction()
    {
        SlashAttackScript.Attack();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
