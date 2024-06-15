using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAttack : BTNodeBase
{

    private EnemyBase EnemyScript;
    private GameObject EnemyObjectRef;

    public BTAttack(GameObject AIObject)
    {
        EnemyScript = AIObject.GetComponent<EnemyBase>();
        EnemyObjectRef = AIObject;
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
