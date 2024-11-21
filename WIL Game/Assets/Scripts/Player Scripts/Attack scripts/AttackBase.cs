using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBase : MonoBehaviour
{
    [SerializeField]protected PlayerAttacks PlayerAttackRef;
    [SerializeField] protected PlayerMovement PlayerMoveScript;
    [SerializeField] protected PlayerInteraction PlayerInteractionScript;
    public int AppliedDamage;

    [SerializeField] protected Collider AttackCollider;

    public void Start()
    {
        PlayerInteractionScript = this.transform.parent.root.root.GetComponent<PlayerInteraction>();
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
                PlayerInteractionScript.PlayHit(Collision.transform.position);
                return;
            }
            if(Collision.GetComponent<EnemyBase>() != null)
            {
                EnemyBase EnemyBaseScript = Collision.GetComponent<EnemyBase>();
                EnemyBaseScript.HandleHealth(-AppliedDamage);
                PlayerInteractionScript.PlayHit(Collision.transform.position);
                return;
            }

        }
        if (Collision.CompareTag("Boss") && PlayerAttackRef != null && this.isActiveAndEnabled)
        {
            Debug.Log(Collision.name);
            BossBase EnemyBaseScript = Collision.GetComponent<BossBase>();

            EnemyBaseScript.HandleHealth(-AppliedDamage);

            PlayerInteractionScript.PlayHit(Collision.transform.position);
        }

        if (Collision.CompareTag("LevelBarrier") && PlayerInteractionScript.PoweredUp) 
        {
            Destroy(Collision.gameObject);
            PlayerInteractionScript.PoweredUp = false;
            PlayerInteractionScript.PlayHit(Collision.transform.position);
        }

    }
}
