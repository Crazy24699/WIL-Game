using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : MonoBehaviour
{
    protected PlayerAttacks PlayerAttackRef;
    protected PlayerMovement PlayerMoveScript;

    [SerializeField] protected Collider AttackCollider;

    public void Start()
    {
        PlayerAttackRef = transform.GetComponentInParent<PlayerAttacks>();
        if(PlayerMoveScript == null)
        {
            PlayerMoveScript = transform.GetComponentInParent<PlayerMovement>();
        }
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
