using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTTurtleMove : BTNodeBase
{

    private TurtleBossAI BossScript;
    private GameObject BossObjectRef;

    private Transform TurtleDestination;
    private bool LocationChosen = false;

    public BTTurtleMove(GameObject EnemyAIRef)
    {
        BossScript = EnemyAIRef.GetComponent<TurtleBossAI>();
        BossObjectRef = EnemyAIRef;
    }


    public override NodeStateOptions RunLogicAndState()
    {
        //Debug.Log("Dieeee");
        if (BossScript.BubbleAttackClass.AttackCooldownActive && BossScript.BubbleAttackClass.AttackCooldownActive)
        {
            BossScript.MoveToPlayer = true;
        }

        if (!BossScript.MoveToPlayer) 
        {
            LocationChosen = false;
        }

        if (BossScript.Distance > 6 && BossScript.BubbleAttackClass.AttackCooldownActive) 
        {
            BossScript.MoveToPlayer = true;
        }

        if(!LocationChosen && BossScript.MoveToPlayer)
        {
            TurtleDestination = BossScript.PlayerRef.transform;
            LocationChosen = true;
        }

        if (BossScript.MoveToPlayer && LocationChosen)
        {
            BossScript.SetDestination(TurtleDestination);
            Debug.Log("Dieeee");
            return NodeStateOptions.Running;
        }
        //Debug.Log("Stunned");
        return NodeStateOptions.Failed;
    }

}
