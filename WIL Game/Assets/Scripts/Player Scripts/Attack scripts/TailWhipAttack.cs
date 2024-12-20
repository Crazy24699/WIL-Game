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
        //Vector3 CurrentRotation = transform.rotation.eulerAngles;
        //CurrentRotation *= -1;
        //transform.rotation = Quaternion.Euler(CurrentRotation);
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
            if (Collision.GetComponent<BaseEnemy>() != null)
            {
                
                Collision.GetComponent<BaseEnemy>().HandleHealth(-5);
                //return;
            }
            if(Collision.GetComponent<EnemyBase>() != null)
            {
                Collision.GetComponent<EnemyBase>().HandleHealth(-5);

            }
            Destroy(this.gameObject);
        }

        if (Collision.CompareTag("Boss"))
        {
            Debug.Log("aaaa");
            Collision.GetComponent<BossBase>().HandleHealth(-5);
            Destroy(this.gameObject);
        }
        if (ColliderExceptions.Contains(Collision.tag))
        {
            Destroy(gameObject);
        }

    }

}
