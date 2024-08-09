using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BTKaraAttack : BTNodeBase
{
    private KaraBossAI KaraScript;
    private GameObject BossObjectRef;

    private KaraBossAI.AttackOptions[] AvalibleAttacks;
    private string ChosenAttack;

    public BTKaraAttack(GameObject EnemyAIRef)
    {
        KaraScript = EnemyAIRef.GetComponent<KaraBossAI>();
        BossObjectRef = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {
        if (KaraScript.BeyondAllAttack)
        {
            Debug.Log("At midnight, youve become a beat that i cant tame, loves possion is a devils game");
            return NodeStateOptions.Failed;
        }

        if (KaraScript.CanAttack && !KaraScript.AttackChosen)
        {
            ChooseAttack();
            return NodeStateOptions.Passed;
        }
        else if( KaraScript.CanAttack && KaraScript.AttackChosen)
        {
            Debug.Log(ChosenAttack);
            KaraScript.RunChosenAttack();
            return NodeStateOptions.Running;
        }

        //if (KaraScript.AttackChosen && KaraScript.PerformingAttack && KaraScript.AttackChosen)
        //{
        //    Debug.Log("Attacking");
        //    KaraScript.RunChosenAttack();
        //    return NodeStateOptions.Running;
        //}

        return NodeStateOptions.Failed;
    }

    private void ChooseAttack()
    {

        float Values = KaraScript.PlayerDistance;
        if (Values > 60 && Values <= 215) 
        {
            ChosenAttack = "Coal";
        }
        if(Values > 40 && Values <= 60)
        {
            ChosenAttack = "GroundSlam";
        }
        if (Values >= 0 && Values < 40)
        {
            ChosenAttack = "Horn attack";
        }
        KaraScript.AttackChosen = true;
        return;
    }

}
