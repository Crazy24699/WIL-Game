using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWebChoice : BTNodeBase
{
    private WebbinEnemy WebbinScript;
    private GameObject BossObjectRef;

    private string ChosenAttack;
    public bool BeyondAllAttacks;
    private bool CatchingPlayer = false;
    private bool SpeedActive = false;

    public BTWebChoice(GameObject EnemyAIRef)
    {
        WebbinScript = EnemyAIRef.GetComponent<WebbinEnemy>();
        BossObjectRef = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {
        BeyondAllAttacks = WebbinScript.BeyondMaxRange();
        CatchingPlayer = BeyondAllAttacks && !CatchingPlayer;


        if (CatchingPlayer)
        {
            if (!SpeedActive)
            {
                WebbinScript.HandleEnemySpeed(true);
                SpeedActive = true;
            }
            
            return NodeStateOptions.Running;
        }
        else if(CatchingPlayer && !BeyondAllAttacks)
        {
            CatchingPlayer = false;
            WebbinScript.HandleEnemySpeed(false);
        }

        return NodeStateOptions.Failed;
    }


}
