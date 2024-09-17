using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TailWhipAttack : ProjectileBase
{

    private float CurrentLifeTime = 0.0f;
    [SerializeField]private List<string> ColliderExceptions = new List<string>();

    protected override void CustomBehaviour()
    {
        Damage = 2;

        LifeTime = 10;
        CurrentLifeTime = LifeTime;

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

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Enemy"))
        {
            if (Collision.GetComponent<Enim2PH>() != null)
            {
                Collision.GetComponent<Enim2PH>().HandleHealth(-5);
                return;
            }

            Collision.GetComponent<EnemyBase>().HandleHealth(1);

        }

        if (Collision.CompareTag("Boss"))
        {
            Collision.GetComponent<BossBase>().HandleHealth(-1);
        }
        if (!ColliderExceptions.Contains(Collision.tag))
        {
            Destroy(gameObject);
        }

    }

}
