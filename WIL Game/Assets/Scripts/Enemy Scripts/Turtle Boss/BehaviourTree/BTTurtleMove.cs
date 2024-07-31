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
        if (BossScript.PerformingAttack)
        {
            BossScript.MoveToPlayer = false;
            TurtleDestination = BossObjectRef.transform;
            BossScript.SetDestination(TurtleDestination);
            LocationChosen = true;
            return NodeStateOptions.Running;
        }
        //Debug.Log("Dieeee");
        
        
        if (BossScript.BubbleAttackClass.AttackCooldownActive && BossScript.BubbleAttackClass.AttackCooldownActive && BossScript.DistanceToPlayer > 20) 
        {
            Debug.Log("So you can spit the truth a littlt");
            BossScript.MoveToPlayer = true;
        }
        else if (BossScript.DistanceToPlayer <= 10)
        {
            BossScript.MoveToPlayer = false;
            TurtleDestination = BossObjectRef.transform;
            BossScript.SetDestination(TurtleDestination);
            LocationChosen = true;
            return NodeStateOptions.Running;
        }

        if (BossScript.MoveToPlayer)
        {
            TurtleDestination = BossScript.PlayerRef.transform;
            BossScript.SetDestination(BossScript.PlayerRef.transform);
            return NodeStateOptions.Running;
        }

        if (!BossScript.MoveToPlayer) 
        {
            LocationChosen = false;
        }


        if (BossScript.DistanceToPlayer > 60 && BossScript.BubbleAttackClass.AttackCooldownActive) 
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
