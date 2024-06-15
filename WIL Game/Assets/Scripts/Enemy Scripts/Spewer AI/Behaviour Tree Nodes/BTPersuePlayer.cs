using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTPersuePlayer : BTNodeBase
{
    protected EnemyBase EnemyScript;
    protected GameObject EnemySelf;



    public BTPersuePlayer(GameObject EnemyAIRef)
    {
        EnemyScript = EnemyAIRef.GetComponent<EnemyBase>();
        EnemySelf = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {

        if (EnemyScript.IsAttacking)
        {
            return NodeStateOptions.Running;
        }

        if (EnemyScript.SeenPlayer && !EnemyScript.PlayerEscaped)
        {
            EnemyScript.SetDestination(EnemyScript.PlayerTarget);
            EnemyScript.HandlePlayerRange();
            if(EnemyScript.CurrentPlayerDistance>EnemyScript.OutOfAttackDistance)
            {
                EnemyScript.CanAttackPlayer = false;
            }

            return NodeStateOptions.Passed;
        }
        else if(!EnemyScript.PlayerEscaped && !EnemyScript.SeenPlayer)
        {
            EnemyScript.PatrolActive = true;
            return NodeStateOptions.Failed;
        }

        if (EnemyScript.AttackPlayer && EnemyScript.SeenPlayer) 
        {
            EnemyScript.SetDestination(EnemyScript.PlayerTarget);
            return NodeStateOptions.Running;
        }
        return NodeStateOptions.Failed;
    }



}
