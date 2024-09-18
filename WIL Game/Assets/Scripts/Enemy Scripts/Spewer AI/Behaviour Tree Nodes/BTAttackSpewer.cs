using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAttackSpewer : BTNodeBase
{

    private SpewerAi EnemyScript;
    private GameObject EnemyObjectRef;

    public BTAttackSpewer(GameObject EnemyAIRef)
    {
        EnemyScript = EnemyAIRef.GetComponent<SpewerAi>();
        EnemyObjectRef = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {
        
        if (!EnemyScript.OutOfAttackRange && EnemyScript.SeenPlayer)
        {
            EnemyScript.RotateToTarget();
            //EnemyScript.NavMeshRef.isStopped = true;
            EnemyScript.Attack();
            return NodeStateOptions.Passed;
        }

        return NodeStateOptions.Failed;
    }

    private void HandleAttack()
    {
        EnemyScript.ChangeLockState(EnemyScript.IsAttacking);

        float CurrentDistance = EnemyScript.CurrentPlayerDistance;
        float MaxAttackDistance = EnemyScript.MaxAttackDistance;
        if (CurrentDistance <= MaxAttackDistance)
        {
            EnemyScript.CanAttackPlayer = true;
        }
        else if(CurrentDistance > MaxAttackDistance)
        {
            EnemyScript.CanAttackPlayer = false;
        }

    }

}
