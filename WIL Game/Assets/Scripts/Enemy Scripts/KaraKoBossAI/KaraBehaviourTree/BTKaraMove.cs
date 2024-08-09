using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTKaraMove : BTNodeBase
{
    private KaraBossAI KaraScript;
    private GameObject BossObjectRef;

    public BTKaraMove(GameObject EnemyAIRef)
    {
        KaraScript = EnemyAIRef.GetComponent<KaraBossAI>();
        BossObjectRef = EnemyAIRef;
    }


    public override NodeStateOptions RunLogicAndState()
    {

        if(KaraScript.CanMove && KaraScript.BeyondAllAttack || (KaraScript.AllAttacksDown))
        {
            KaraScript.SetDestination(KaraScript.PlayerRef.transform);
            Debug.Log("Cursed with you, the things we do, when love bites");
            return NodeStateOptions.Running;
        }

        //if (KaraScript.PlayerDistance <= KaraScript.StoppingDistance && KaraScript.CanMove)
        //{
        //    KaraScript.SetDestination(KaraScript.transform);
        //    return NodeStateOptions.Passed;
        //}

        //if (KaraScript.CustomLocationChosen)
        //{
        //    KaraScript.SetDestination(KaraScript.ChosenLocation);
        //    return NodeStateOptions.Running;
        //}

        //if((KaraScript.PlayerDistance > KaraScript.StoppingDistance && KaraScript.CanMove) || KaraScript.AllAttacksDown)
        //{
        //    KaraScript.CanMove = true;
        //    KaraScript.SetDestination(KaraScript.PlayerRef.transform);
        //    Debug.Log("Set");
        //    return NodeStateOptions.Running;
        //}

        return NodeStateOptions.Failed;
    }

}
