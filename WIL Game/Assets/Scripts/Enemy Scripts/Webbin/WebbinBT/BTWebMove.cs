using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWebMove : BTNodeBase
{
    private WebbinEnemy WebbinScript;
    private GameObject BossObjectRef;

    private string ChosenAttack;
    public bool BeyondAllAttacks;
    private bool CatchingPlayer = false;
    private bool SpeedActive = false;

    public BTWebMove(GameObject EnemyAIRef)
    {
        WebbinScript = EnemyAIRef.GetComponent<WebbinEnemy>();
        BossObjectRef = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {
        if (!WebbinScript.EngagePlayer) { return NodeStateOptions.Failed; }
        BeyondAllAttacks = WebbinScript.BeyondMaxRange();
        CatchingPlayer = BeyondAllAttacks && !CatchingPlayer;

        if (WebbinScript.AttackChosen && WebbinScript.CurrentPlayerDistance < WebbinScript.CurrentAttackDistance)  
        {
            Debug.Log("king and queen");
            WebbinScript.HandleMovingState(false);
            return NodeStateOptions.Failed; 
        }

        Debug.Log(BeyondAllAttacks);

        if(WebbinScript.CurrentPlayerDistance>WebbinScript.StoppingDistance)
        {
            Debug.Log("What the fuck");
            if (WebbinScript.NavMeshRef.speed <= 0)
            {
                WebbinScript.HandleMovingState(true);
            }
            WebbinScript.NavMeshRef.SetDestination(WebbinScript.PlayerRef.transform.position);
            return NodeStateOptions.Running;
        }

        StoppingDistance();
        if (WebbinScript.CurrentPlayerDistance > WebbinScript.CurrentAttackDistance)
        {
            Debug.Log("deal me in and let me play a hand");
            WebbinScript.NavMeshRef.SetDestination(WebbinScript.PlayerRef.transform.position);
            return NodeStateOptions.Running;
        }
        if (CatchingPlayer)
        {
            if (!SpeedActive)
            {
                WebbinScript.HandleEnemySpeed(true);
                SpeedActive = true;
            }
            WebbinScript.NavMeshRef.SetDestination(WebbinScript.PlayerRef.transform.position);
            return NodeStateOptions.Running;
        }
        else if (CatchingPlayer && !BeyondAllAttacks)
        {
            CatchingPlayer = false;
            WebbinScript.HandleEnemySpeed(false);
        }

        return NodeStateOptions.Failed;
    }

    private void StoppingDistance()
    {

        if (WebbinScript.CurrentPlayerDistance <= WebbinScript.StoppingDistance)
        {
            Debug.Log("Unleased now essense of love from above");
            WebbinScript.HandleMovingState(false);

            Debug.Log("Dance");
        }
        else if (WebbinScript.CurrentPlayerDistance > WebbinScript.MaxAttackDistance)
        {
            if (WebbinScript.NavMeshRef.speed < WebbinScript.BaseMoveSpeed)
            {
                WebbinScript.NavMeshRef.speed = WebbinScript.BaseMoveSpeed;
            }
            WebbinScript.HandleMovingState(true);
            Debug.Log("ive made our hellbent mistake");
            WebbinScript.NavMeshRef.SetDestination(WebbinScript.PlayerRef.transform.position);

        }
    }

}
