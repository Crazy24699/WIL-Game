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
            Debug.Log(Collision.name + "   Hit Object");
            Destroy(this.gameObject);
        }
    }

}
