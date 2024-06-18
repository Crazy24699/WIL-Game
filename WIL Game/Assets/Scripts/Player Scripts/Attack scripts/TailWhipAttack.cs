using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailWhipAttack : ProjectileBase
{

    private float CurrentLifeTime = 0.0f;
    

    protected override void CustomBehaviour()
    {
        Damage = 2;

        LifeTime = 10;

        CustomLifeTimer = true;

    }


    private void Update()
    {
        if (CurrentLifeTime <= 0)
        {
            CurrentLifeTime = 0;
            Destroy(gameObject);
        }
    }

}
