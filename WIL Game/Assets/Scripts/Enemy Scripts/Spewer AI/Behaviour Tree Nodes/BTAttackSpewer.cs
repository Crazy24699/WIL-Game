using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAttackSpewer : BTNodeBase
{

    private EnemyBase EnemyScript;
    private GameObject EnemyObjectRef;

    public BTAttackSpewer(GameObject EnemyAIRef)
    {
        EnemyScript = EnemyAIRef.GetComponent<EnemyBase>();
        EnemyObjectRef = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {

        //if (EnemyScript.IsAttacking)
        //{
        //    EnemyScript.EnforceLock();
        //    return NodeStateOptions.Running;
        //}
        //if(EnemyScript.CurrentPlayerDistance <= EnemyScript.OutOfAttackDistance)
        //{
        //    EnemyScript.CanAttackPlayer = true;
        //    EnemyScript.AttackPlayer = true;
        //    Debug.Log("Bools set");
        //}
        //if (EnemyScript.AttackPlayer && EnemyScript.CanAttackPlayer && EnemyScript.SeenPlayer) 
        //{
        //    EnemyScript.Attack();
        //    Debug.Log("Attack Triggered");
        //    return NodeStateOptions.Running;
        //}

        if (!EnemyScript.OutOfAttackRange && EnemyScript.SeenPlayer)
        {
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
