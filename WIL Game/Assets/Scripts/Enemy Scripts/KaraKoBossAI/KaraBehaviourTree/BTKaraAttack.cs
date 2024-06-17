using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BTKaraAttack : BTNodeBase
{
    private KaraBossAI KaraScript;
    private GameObject BossObjectRef;

    public BTKaraAttack(GameObject EnemyAIRef)
    {
        KaraScript = EnemyAIRef.GetComponent<KaraBossAI>();
        BossObjectRef = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {
        if (KaraScript.AttackChosen && KaraScript.PerformingAttack && KaraScript.AttackChosen)
        {
            Debug.Log("Attacking");
            KaraScript.RunChosenAttack();
            return NodeStateOptions.Running;
        }

        return NodeStateOptions.Failed;
    }



}
