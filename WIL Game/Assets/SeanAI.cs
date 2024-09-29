using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SeanAI : BaseEnemy
{
    private bool Attacking;

    private void Start()
    {
        BaseStartup();
    }

    protected override void CustomStartup()
    {
        base.CustomStartup();
        MaxHealth = 10;
        BaseMoveSpeed = 15;
        EnemyObjectRef = this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerTarget = PlayerRef.transform;

        WorldHandlerScript = FindObjectOfType<WorldHandler>();


        StartupRan = true;
        Alive = true;
    }


    public override void Attack()
    {
        if(!CanAttack || !OnAttackingList) { return; }

        if (Attacking)
        {
            return;
        }
        NavMeshRef.SetDestination(PlayerTarget.transform.position);
    }

    private void Update()
    {
        if (!StartupRan) { return; }

        if(Orbiting)
        {
            RotateToTarget();
            KeepOrbitDistance();
            return;
        }
    }

}
