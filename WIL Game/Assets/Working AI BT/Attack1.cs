using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1 : BTNodeBase
{

    public BTAI RefAI;

    public Attack1(GameObject Ref)
    {
        RefAI = Ref.GetComponent<BTAI>();
    }

    public override NodeStateOptions RunLogicAndState()
    {
        if(RefAI.ShortAttackVar)
        {
            Debug.Log("Attack");
            RefAI.SAttack.HandleCooldown();
            return NodeStateOptions.Passed;
        }
        return NodeStateOptions.Failed;
    }
}
