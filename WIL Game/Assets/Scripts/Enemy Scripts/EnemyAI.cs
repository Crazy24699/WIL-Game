using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : EnemyBase
{
    [SerializeField] protected float KnockbackPower;


    private void Start()
    {
        BaseStartup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyKnockback(float KnockbackPower)
    {

    }

}
