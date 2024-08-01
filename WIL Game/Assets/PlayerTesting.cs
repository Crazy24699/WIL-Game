using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTesting : MonoBehaviour
{
    public int CycleNum;
    public int CycleCount;

    public float AttackWaitTime;
    public float ElapsedTime;

    public bool CycleAttacks;
    public bool CanAttack=true;

    public enum AllAttacks
    {
        None,
        SlashAttack,
        TailWhip,
        BiteAttack
    }
    public AllAttacks CurrentAttack;
    public AllAttacks NextAttack;

    public PlayerAttacks PlayerAttackScript;
    // Start is called before the first frame update
    void Start()
    {
        PlayerAttackScript = GetComponent<PlayerAttacks>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            CycleAttacks = true;
            ElapsedTime = AttackWaitTime;
            CycleCount = CycleNum;
        }
        if (CycleAttacks)
        {
            //CycleAllAttacks();
        }
    }

    private void CycleAllAttacks()
    {
        ElapsedTime -=Time.deltaTime;
        if (ElapsedTime <= 0 && CycleCount > 0 && NextAttack==AllAttacks.None) 
        {
            CanAttack = PlayerAttackScript.AttackAnimation.GetBool("IsAttacking");
            int RandomAttack = Random.Range(0, 3);
            switch (RandomAttack)
            {
                case 0:

                    if (CanAttack)
                    {
                        NextAttack = AllAttacks.SlashAttack;
                        break;
                    }
                    PlayerAttackScript.SlashAttack();
                    CurrentAttack = AllAttacks.SlashAttack;
                    break;

                case 1:
                    if (CanAttack)
                    {
                        NextAttack = AllAttacks.BiteAttack;
                        break;
                    }
                    PlayerAttackScript.BiteAttack(); 
                    CurrentAttack = AllAttacks.BiteAttack;
                    break;

                case 2:
                    if (CanAttack)
                    {
                        NextAttack = AllAttacks.TailWhip;
                        break;
                    }
                    PlayerAttackScript.TailWhipAttack();
                    CurrentAttack = AllAttacks.TailWhip;
                    break;
            }
            ElapsedTime = AttackWaitTime;
            CycleCount--;
            return;

        }
        else if (NextAttack != AllAttacks.None)
        {
            switch (NextAttack)
            {
                case AllAttacks.None:
                    break;

                case AllAttacks.SlashAttack:
                    PlayerAttackScript.SlashAttack();
                    break;

                case AllAttacks.TailWhip:
                    PlayerAttackScript.TailWhipAttack();
                    break;

                case AllAttacks.BiteAttack:
                    PlayerAttackScript.BiteAttack();

                    break;

            }
            CurrentAttack = NextAttack;
            NextAttack = AllAttacks.None;
        }
    }

}
