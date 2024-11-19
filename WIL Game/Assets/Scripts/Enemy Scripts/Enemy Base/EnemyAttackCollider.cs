using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    public int AttackDamage;

    private void OnTriggerEnter(Collider Collision)
    {
        if (!this.isActiveAndEnabled)
        {
            return;
        }
        Debug.Log(Collision.name);
        if (Collision.CompareTag("Player"))
        {
            Debug.Log("This Player");
            PlayerInteraction PlayerHealth = Collision.GetComponent<PlayerInteraction>();
            if (PlayerHealth == null)
            {
                PlayerHealth=Collision.transform.GetComponentInParent<PlayerInteraction>();
            }
            PlayerHealth.TakeHit(-AttackDamage, transform.position);
            Debug.Log("Takes Damage");

        }
    }
}
