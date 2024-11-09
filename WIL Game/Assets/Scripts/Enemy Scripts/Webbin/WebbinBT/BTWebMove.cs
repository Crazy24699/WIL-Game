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
        //if (WebbinScript.CanAttack) { return NodeStateOptions.Failed; }

        Debug.Log(BeyondAllAttacks);

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

}
