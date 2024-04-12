using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawFunctionality : MonoBehaviour
{
    protected PlayerAttacks PlayerAttackRef;

    public void Start()
    {
        PlayerAttackRef = FindObjectOfType<PlayerAttacks>();
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Enemy") && PlayerAttackRef != null) 
        {
            EnemyValuesFunctionality EnemyFunctionScript = Collision.GetComponent<EnemyValuesFunctionality>();

            EnemyFunctionScript.HandleHealth(PlayerAttackRef.Damage);

        }
    }
}
