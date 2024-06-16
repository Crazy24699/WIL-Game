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
        if (KaraScript.CanPerformAction)
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

        float AttackRange = KaraScript.PlayerDistance;
        KaraScript.CheckDistance();

        switch (KaraScript.CloseRange)
        {
            case true:
                ChooseCloseRangeAttack(AttackRange);
                break;

            case false:

                if(!KaraScript.CoalAttack.AttackCooldownActive)
                {
                    KaraScript.ChosenAttack=KaraBossAI.AttackOptions.CoalBarrage;
                    break;
                }
                ChooseCloseRangeAttack(AttackRange);

                break;  
        }
        KaraScript.AttackChosen = true;
        KaraScript.RunChosenAttack();


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
                        return;
                    }
                    KaraScript.ChosenAttack = KaraBossAI.AttackOptions.HornSwipe;
                    break;

                case 1:
                    if (KaraScript.EarthAttack.AttackCooldownActive)
                    {
                        PickAlternateAttack(KaraBossAI.AttackOptions.HornSwipe);
                        return;
                    }
                    KaraScript.ChosenAttack = KaraBossAI.AttackOptions.EarthShaker;
                    break;
            }
        }
    }

    private void PickAlternateAttack(KaraBossAI.AttackOptions InvalidAttack)
    {
        if(KaraScript.EarthAttack.AttackCooldownActive && KaraScript.HornAttack.AttackCooldownActive)
        {
            KaraScript.ResetAttackLockout(5.5f);
            KaraScript.CanMove = true;
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
