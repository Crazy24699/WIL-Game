using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generator guy
public class Enim2PH : EnemyBase
{


    protected override void CustomStartup()
    {
        MaxHealth = 30;
        BaseMoveSpeed = 16;

        CreateBehaviourTree();
        UpdateBehaviour();

    }

    private void CreateBehaviourTree()
    {

    }

    private void UpdateBehaviour()
    {

    }

}
