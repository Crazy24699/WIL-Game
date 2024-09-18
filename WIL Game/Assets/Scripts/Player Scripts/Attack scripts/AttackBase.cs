using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : MonoBehaviour
{
    [SerializeField]protected PlayerAttacks PlayerAttackRef;
    [SerializeField] protected PlayerMovement PlayerMoveScript;
    public int AppliedDamage;

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
            
            if (Collision.GetComponent<BaseEnemy>() != null)
            {
                BaseEnemy EnemyBaseScript = Collision.GetComponent<BaseEnemy>();
                EnemyBaseScript.HandleHealth(-AppliedDamage);
                return;
            }
            if(Collision.GetComponent<EnemyBase>() != null)
            {
                EnemyBase EnemyBaseScript = Collision.GetComponent<EnemyBase>();
                EnemyBaseScript.HandleHealth(-AppliedDamage);
                return;
            }
            

        }
        if (Collision.CompareTag("Boss") && PlayerAttackRef != null && this.isActiveAndEnabled)
        {
            Debug.Log(Collision.name);
            BossBase EnemyBaseScript = Collision.GetComponent<BossBase>();

            EnemyBaseScript.HandleHealth(-AppliedDamage);

        }
    }
}
