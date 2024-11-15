using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppletProjectile : ProjectileBase
{
    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("DamageObject") || Collision.CompareTag("Player"))
        {
            //Deal Damage to the thing
            if (Collision.CompareTag("Player"))
            {

                Collision.transform.GetComponentInParent<PlayerInteraction>().HandleHealth(-5);
                Collision.transform.GetComponentInParent<PlayerInteraction>().TakeHit(-5, transform.position);
                Destroy(this.gameObject);
            }
            if (Collision.CompareTag("Ground"))
            {
                Destroy(this.gameObject);
            }
            Debug.Log(Collision.name + "   Hit Object");
            //Destroy(this.gameObject);
        }
    }

}
