using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTKaraChoice : BTNodeBase
{
    private KaraBossAI KaraScript;
    private GameObject BossObjectRef;


    public BTKaraChoice(GameObject EnemyAIRef)
    {
        KaraScript = EnemyAIRef.GetComponent<KaraBossAI>();
        BossObjectRef = EnemyAIRef;
    }


    public override NodeStateOptions RunLogicAndState()
    {

        

        if (KaraScript.CanPerformAction && !KaraScript.AttackChosen && !KaraScript.PerformingAttack)
        {

            ChooseAttack();
            return NodeStateOptions.Running;
        }

        if (KaraScript.AllAttacksDown && KaraScript.CanPerformAction)
        {
            KaraScript.SetDestination(KaraScript.PlayerRef.transform);
            return NodeStateOptions.Running;
        }

        return NodeStateOptions.Failed;
    }

    private IEnumerator NextActionCooldown()
    {
        
        yield return new WaitForSeconds(2.5f);
        KaraScript.CanPerformAction = true;
    }

    private void ChooseAttack()
    {
        Debug.Log("Runner   "); 
        float AttackRange = KaraScript.PlayerDistance;
        KaraScript.CheckDistance();

        switch (KaraScript.CloseRange)
        {
            case true:
                ChooseCloseRangeAttack(AttackRange);
                Debug.Log("Forbiden rites");
                break;

            case false:

                if(!KaraScript.CoalAttack.AttackCooldownActive)
                {
                    KaraScript.ChosenAttack=KaraBossAI.AttackOptions.CoalBarrage;
                    break;
                }
                //ChooseCloseRangeAttack(AttackRange);

                break;  
        }
        KaraScript.AttackChosen = true;
        KaraScript.PerformingAttack = true;
        //KaraScript.RunChosenAttack();


    }

    private void ChooseCloseRangeAttack(float CurrentPlayerDistance)
    {
        if(KaraScript.CanPerformAction)
        {
            int RandomAttack = Random.Range(0, 2);
            switch (RandomAttack)
            {
                case 0:
                    if (KaraScript.HornAttack.AttackCooldownActive)
                    {
                        PickAlternateAttack(KaraBossAI.AttackOptions.HornSwipe);
                        //Debug.Log("Trying to rescue");
                        return;
                    }
                    KaraScript.ChosenAttack = KaraBossAI.AttackOptions.HornSwipe;
                    //Debug.Log("Pop cult crusifiction");
                    break;

                case 1:
                    if (KaraScript.EarthAttack.AttackCooldownActive)
                    {
                        PickAlternateAttack(KaraBossAI.AttackOptions.EarthShaker);
                        //Debug.Log("Now youir ink is bleeding red");
                        return;
                    }
                    //Debug.Log("the headlines are going dead");
                    KaraScript.ChosenAttack = KaraBossAI.AttackOptions.EarthShaker;
                    break;
            }
        }
    }

    private void PickAlternateAttack(KaraBossAI.AttackOptions InvalidAttack)
    {
        Debug.Log("Love bites       "+InvalidAttack);
        if(KaraScript.EarthAttack.AttackCooldownActive && KaraScript.HornAttack.AttackCooldownActive)
        {
            //KaraScript.ResetAttackLockout(5.5f);
            KaraScript.CanMove = true;
            Debug.Log("for moon lit nightss");

            return;
        }
        switch (InvalidAttack)
        {

            case KaraBossAI.AttackOptions.HornSwipe:

                break;

            case KaraBossAI.AttackOptions.EarthShaker:
                break;

        }
    }

}
