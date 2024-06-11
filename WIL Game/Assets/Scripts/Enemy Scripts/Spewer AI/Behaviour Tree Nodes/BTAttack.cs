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
            return NodeStateOptions.Running;
        }

        if (EnemyScript.AttackPlayer && EnemyScript.CanAttackPlayer)
        {
            EnemyScript.Attack();

            return NodeStateOptions.Running;
        }
        


        return NodeStateOptions.Failed;
    }

}
