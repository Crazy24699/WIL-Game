using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBag : BaseEnemy
{

    [SerializeField] private Vector3 TargetedPosition;

    [SerializeField] private bool HasTargetPosition = false;
    
    [SerializeField] private float TravelDistance;
    [SerializeField] private float CurrentDistance;
    [SerializeField] private float AttackDistance;

    private void Start()
    {
        Debug.LogError("Remove this when the enemy is done");
        BaseStartup();
    }

    protected override void CustomStartup()
    {
        MaxHealth = 8;
        CurrentHealth = MaxHealth;
        BaseMoveSpeed = 30;
    }

    public override void Attack()
    {
        
        TargetedPosition = Vector3.zero;
        CanAttack = false;
        StartCoroutine(AttackCooldown());

    }

    private void CalculatePosition()
    {
        
        Vector3 Direction = PlayerRef.transform.position - transform.position;
        Direction = Direction.normalized;
        Direction.y = 0;

        float TargetDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);
        TravelDistance = TargetDistance;
        TargetedPosition = transform.position + Direction * TravelDistance*2;
        TargetedPosition.y = 0;
        TargetedPosition = TargetedPosition.RoundVector(2);
    }

    private void Update()
    {
        if (!StartupRan) { return; }

        
    }


    public void CreateBehaviourTree()
    {

    }

    private void HandleAttack()
    {
        CurrentPosition = transform.position.RoundVector(2);

        if (CanAttack)
        {
            HasTargetPosition = TargetedPosition != Vector3.zero;
            CalculatePosition();
            if (CurrentDistance <= AttackDistance)
            {
                Attack();
            }
        }
        if (!CanAttack && CurrentPosition!=TargetedPosition)
        {
            NavMeshRef.SetDestination(TargetedPosition);
        }
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(2.35f);
        CanAttack = true;
    }

}
