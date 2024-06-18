using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{


    private void OnTriggerEnter(Collider Collision)
    {
        if (!this.isActiveAndEnabled)
        {
            return;
        }
        if (Collision.CompareTag("Player"))
        {
            PlayerInteraction PlayerHealth = Collision.GetComponent<PlayerInteraction>();
            if (PlayerHealth == null)
            {
                PlayerHealth=Collision.transform.GetComponentInParent<PlayerInteraction>();
            }
            PlayerHealth.HandleHealth(-1);

        }
    }
}
