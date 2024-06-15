using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTTurtleChoice : BTNodeBase
{

    private TurtleBossAI BossScript;
    private GameObject BossObjectRef;

    public BTTurtleChoice(GameObject EnemyAIRef)
    {
        BossScript = EnemyAIRef.GetComponent<TurtleBossAI>();
        BossObjectRef = EnemyAIRef;
    }


    public override NodeStateOptions RunLogicAndState()
    {

        

        return NodeStateOptions.Failed;
    }

}
