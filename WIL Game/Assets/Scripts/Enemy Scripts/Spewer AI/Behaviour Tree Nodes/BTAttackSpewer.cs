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

        if (EnemyScript.IsAttacking)
        {
            EnemyScript.EnforceLock();
            return NodeStateOptions.Running;
        }
        if(EnemyScript.CurrentPlayerDistance <= EnemyScript.OutOfAttackDistance)
        {
            EnemyScript.CanAttackPlayer = true;
            EnemyScript.AttackPlayer = true;
        }
        if (EnemyScript.AttackPlayer && EnemyScript.CanAttackPlayer && EnemyScript.SeenPlayer) 
        {
            EnemyScript.Attack();

            return NodeStateOptions.Running;
        }
        


        return NodeStateOptions.Failed;
    }

}
