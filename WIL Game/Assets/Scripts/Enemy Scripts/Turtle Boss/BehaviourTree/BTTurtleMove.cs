using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTTurtleMove : BTNodeBase
{

    private TurtleBossAI BossScript;
    private GameObject BossObjectRef;

    private Transform TurtleDestination;
    private bool LocationChosen = false;
    public int Values = 20;

    public BTTurtleMove(GameObject EnemyAIRef)
    {
        BossScript = EnemyAIRef.GetComponent<TurtleBossAI>();
        BossObjectRef = EnemyAIRef;
    }


    public override NodeStateOptions RunLogicAndState()
    {
        

        if (BossScript.MoveToPlayer)
        {
            TurtleDestination = BossScript.PlayerRef.transform;
            BossScript.SetDestination(BossScript.PlayerRef.transform);
            return NodeStateOptions.Running;
        }

        if (BossScript.DistanceToPlayer > 60 && BossScript.BubbleAttackClass.AttackCooldownActive) 
        {
            BossScript.MoveToPlayer = true;
            BossScript.SetDestination(BossScript.PlayerRef.transform);
        }
        //Debug.Log("Stunned");
        return NodeStateOptions.Failed;
    }

}
