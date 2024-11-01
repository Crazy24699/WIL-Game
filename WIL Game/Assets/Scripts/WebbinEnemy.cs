using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebbinEnemy : BaseEnemy
{

    private void Start()
    {
        BaseStartup();
    }

    protected override void CustomStartup()
    {
        MaxHealth = 8;
        CurrentHealth = MaxHealth;
        BaseMoveSpeed = 30;
    }

}
