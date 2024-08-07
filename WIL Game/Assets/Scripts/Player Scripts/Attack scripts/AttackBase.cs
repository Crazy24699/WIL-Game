using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : MonoBehaviour
{
    [SerializeField]protected PlayerAttacks PlayerAttackRef;
    [SerializeField] protected PlayerMovement PlayerMoveScript;


    [SerializeField] protected Collider AttackCollider;

    public void Start()
    {
        PlayerAttackRef = FindObjectOfType<PlayerAttacks>();
        if(PlayerMoveScript == null)
        {
            PlayerMoveScript = transform.GetComponentInParent<PlayerMovement>();
        }
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Enemy") && PlayerAttackRef != null && this.isActiveAndEnabled)
        {
            Debug.Log("Benieth the starts");
            EnemyBase EnemyBaseScript = Collision.GetComponent<EnemyBase>();

            EnemyBaseScript.HandleHealth(PlayerAttackRef.Damage);
            EnemyBaseScript.ApplyKnockback();
            EnemyBaseScript.DistableAttack();
        }
        if (Collision.CompareTag("Boss") && PlayerAttackRef != null && this.isActiveAndEnabled)
        {
            Debug.Log(Collision.name);
            BossBase EnemyBaseScript = Collision.GetComponent<BossBase>();

            EnemyBaseScript.HandleHealth(-PlayerAttackRef.Damage);

        }
    }
}
