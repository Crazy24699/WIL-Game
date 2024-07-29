using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack2 : BTNodeBase
{
    public BTAI RefAI;

    public Attack2(GameObject Ref)
    {
        RefAI = Ref.GetComponent<BTAI>();
    }

    public override NodeStateOptions RunLogicAndState()
    {
        if(RefAI.LongAttackVar)
        {
            Debug.Log("Moving");
            return NodeStateOptions.Passed;
        }
        return NodeStateOptions.Failed;
    }
}
