using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTPersuePlayer : BTNodeBase
{
    protected SpewerAi EnemyScript;
    protected GameObject EnemySelf;



    public BTPersuePlayer(GameObject EnemyAIRef)
    {
        EnemyScript = EnemyAIRef.GetComponent<SpewerAi>();
        EnemySelf = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {

        if (EnemyScript.PlayerEscaped || EnemyScript.PatrolActive)
        {
            return NodeStateOptions.Failed;
        }

        if (EnemyScript.SeenPlayer)
        {

            EnemyScript.PatrolActive = false;
            Debug.Log("OPlayer");   
            EnemyScript.SetDestination(EnemyScript.PlayerTarget);
            EnemyScript.HandlePlayerRange();
            Debug.Log("the devils take ");


            return NodeStateOptions.Running;
        }

        if (EnemyScript.AttackAreaOverride && EnemyScript.SeenPlayer)
        {
            EnemyScript.SetDestination(EnemyScript.PlayerTarget);
            return NodeStateOptions.Running;
        }

        if(EnemyScript.OutOfAttackRange)
        {
            EnemyScript.CanAttackPlayer = false;

            EnemyScript.SetDestination(EnemyScript.PlayerTarget);
            EnemyScript.HandlePlayerRange();
            Debug.Log("at midnight ");

            return NodeStateOptions.Running;
        }

        if (EnemyScript.PlayerEscaped)
        {
            //EnemyScript.SeenPlayer = false;
            EnemyScript.PatrolActive = true;

        }

        

        return NodeStateOptions.Failed;
    }



}

