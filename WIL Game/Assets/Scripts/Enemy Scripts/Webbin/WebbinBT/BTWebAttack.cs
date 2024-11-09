using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWebAttack : BTNodeBase
{
    private WebbinEnemy WebbinScript;
    private GameObject BossObjectRef;

    private WebbinEnemy.AttackOptions[] AvalibleAttacks;
    private string ChosenAttack;
    public bool BeyondAllAttacks;

    public BTWebAttack(GameObject EnemyAIRef)
    {
        WebbinScript = EnemyAIRef.GetComponent<WebbinEnemy>();
        BossObjectRef = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {
        BeyondAllAttacks = WebbinScript.BeyondMaxRange();
        if (!WebbinScript.EngagePlayer) { return NodeStateOptions.Failed; }
        if (BeyondAllAttacks)
        {
            return NodeStateOptions.Failed;
        }

        if (WebbinScript.CanAttack && !WebbinScript.AttackChosen)
        {
            ChooseAttack();
            return NodeStateOptions.Passed;
        }
        else if (WebbinScript.CanAttack && WebbinScript.AttackChosen)
        {
            Debug.Log(ChosenAttack);
            //WebbinScript.RunChosenAttack();
            return NodeStateOptions.Running;
        }

        return NodeStateOptions.Failed;
    }

    private void ChooseAttack()
    {

        Debug.Log("Choosing attack");
        float Values = WebbinScript.CurrentPlayerDistance;
        if (Values > 100 && Values <= 215)
        {
            ChosenAttack = "Coal";
            WebbinScript.ChosenAttack = WebbinEnemy.AttackOptions.WebSpit;
        }
        if (Values > 40 && Values <= 100)
        {
            ChosenAttack = "GroundSlam";
            WebbinScript.ChosenAttack = WebbinEnemy.AttackOptions.RollBash;
        }

        WebbinScript.AttackChosen = true;
        return;
    }

}
