using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWebAttack : BTNodeBase
{
    private WebbinEnemy WebbinScript;
    private GameObject BossObjectRef;

    private string ChosenAttack;
    public bool BeyondAllAttacks;

    public BTWebAttack(GameObject EnemyAIRef)
    {
        WebbinScript = EnemyAIRef.GetComponent<WebbinEnemy>();
        BossObjectRef = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {
        //if(WebbinScript.CanAttack)
        //{
        //    ChosenAttack = null;
        //}

        BeyondAllAttacks = WebbinScript.BeyondMaxRange();
        if (!WebbinScript.EngagePlayer || !WebbinScript.CanAttack) { return NodeStateOptions.Failed; }

        Debug.Log("The fuckening");
        if (BeyondAllAttacks)
        {
            return NodeStateOptions.Failed;
        }

        if (WebbinScript.CanAttack && !WebbinScript.AttackChosen)
        {
            Debug.Log("here what we are going to do ");
            ChooseAttack();
            return NodeStateOptions.Passed;
        }
        else if (WebbinScript.CanAttack && WebbinScript.AttackChosen)
        {
            return NodeStateOptions.Running;
        }

        return NodeStateOptions.Failed;
    }

    private void ChooseAttack()
    {

        Debug.Log("Choosing attack");
        float DistanceValue = WebbinScript.CurrentPlayerDistance;

        if(WebbinScript.BashAttack.AttackCoodldownActive && !WebbinScript.WebAttack.AttackCoodldownActive)
        {
            WebbinScript.ChosenAttack = WebbinEnemy.AttackOptions.WebSpit;
            ChosenAttack = "WebSpit";
            WebbinScript.CurrentAttackDistance = WebbinScript.WebSpitRange;
            WebbinScript.AttackChosen = true;
            return;
        }
        if(WebbinScript.WebAttack.AttackCoodldownActive && !WebbinScript.BashAttack.AttackCoodldownActive)
        {
            ChosenAttack = "BashAttack";
            Debug.Log("dark");
            WebbinScript.ChosenAttack = WebbinEnemy.AttackOptions.RollBash;
            WebbinScript.CurrentAttackDistance = WebbinScript.BashAttackRange;
            WebbinScript.AttackChosen = true;
            return;
        }

        if (DistanceValue < WebbinScript.WebSpitRange && DistanceValue > WebbinScript.StoppingDistance)
        {
            ChosenAttack = "Webspit";
            Debug.Log("chosen web spit");
            WebbinScript.ChosenAttack = WebbinEnemy.AttackOptions.WebSpit;
            WebbinScript.CurrentAttackDistance = WebbinScript.WebSpitRange;
        }

        if (DistanceValue < WebbinScript.BashAttackRange && DistanceValue > WebbinScript.WebSpitRange)
        {
            ChosenAttack = "RollBash";
            Debug.Log("and i rolled a 1");
            WebbinScript.ChosenAttack = WebbinEnemy.AttackOptions.RollBash;
            WebbinScript.CurrentAttackDistance = WebbinScript.BashAttackRange;
        }
        Debug.Log(ChosenAttack);

        if (ChosenAttack!=null)
        {
            WebbinScript.AttackChosen = true;
        }
        return;
    }

}
