using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrashBag : BaseEnemy
{

    [SerializeField] private Vector3 TargetedPosition;

    [SerializeField] private bool HasTargetPosition = false;

    private float TravelDistance = 5;
    [SerializeField] private float CurrentDistance;
    [SerializeField] private float AttackDistance;
    [SerializeField] private float MinFollowDistance;
    private float RepathingDelay = 0.35f;
    private float ObstacleCheckRadius = 1.75f;
    private float NextRepathTimer;

    [SerializeField] private Collider ObjectCollider;
    [SerializeField] private Animator TrshBag_Animations;


    private void Start()
    {
        //Debug.LogError("Remove this when the enemy is done");
        BaseStartup();
        
    }

    protected override void CustomStartup()
    {
        MaxHealth = 8;
        CurrentHealth = MaxHealth;
        BaseMoveSpeed = 30;

        TrshBag_Animations = transform.GetComponentInChildren<Animator>();
        NavMeshRef.enabled = true;
    }

    public override void Attack()
    {
        
        //TargetedPosition = Vector3.zero;
        CanAttack = false;
        NavMeshRef.enabled = false;
        StartCoroutine(AttackCooldown());

    }

    private void HandleVision()
    {
        SeenPlayer = true;

        NavMeshRef.speed = BaseMoveSpeed + 5;
       

    }

    private void CalculatePosition()
    {
        
        Vector3 Direction = PlayerRef.transform.position - transform.position;
        Direction = Direction.normalized;
        Direction.y = 0;

        float TargetDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);
        Debug.Log("their final moments calling their captain in vein");
        TravelDistance = TargetDistance;
        TargetedPosition = transform.position + Direction.normalized * 10;
        TargetedPosition.y = transform.position.y;
        TargetedPosition = TargetedPosition.RoundVector(2);

    }

    private void Update()
    {
        if (!StartupRan) { return; }

        if(Input.GetKeyDown(KeyCode.M))
        {
            CanAttack = true;
            
        }
        HandleAttack();
        HandleAttackSequence();
    }

    private void HandleAttackSequence()
    {
        if (!StartupRan) { return; }

        if (SeenPlayer)
        {
            CurrentDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);
            if (CurrentDistance.RoundFloat(2) > MinFollowDistance)
            {
                NavMeshRef.enabled = true;
                NavMeshRef.SetDestination(PlayerRef.transform.position);
            }
            else if (CurrentDistance.RoundFloat(2) <= MinFollowDistance + 1)
            {
                if (NavMeshRef.enabled)
                {
                    NavMeshRef.enabled = false;
                }
                this.gameObject.transform.position = Vector3.MoveTowards(transform.position, TargetedPosition, 2.5f);

            }
        }
    }

    private void FixedUpdate()
    {

    }

    private void MaintainDistance()
    {
        if (CurrentDistance < MinFollowDistance)
        {
            if (Time.time >= NextRepathTimer)
            {
                NextRepathTimer = Time.time + RepathingDelay;
                Vector3 PlayerDirection = (transform.position - PlayerRef.transform.position).normalized;
                Vector3 BackUpPosition = PlayerRef.transform.position + PlayerDirection * (MinFollowDistance + 2);

                if (IsPathInvalid(BackUpPosition))
                {
                    BackUpPosition = FindAlternatePosition();
                }

            }
        }
    }

    private bool IsPathInvalid(Vector3 TargetPosition)
    {
        // Use a sphere cast to detect if the target position is blocked
        return Physics.SphereCast(transform.position, ObstacleCheckRadius, TargetPosition - transform.position, out RaycastHit Hit) &&
               Hit.collider.gameObject != PlayerRef; // Ensure it's not detecting Drone 2
    }

    private Vector3 FindAlternatePosition()
    {
        Vector3 BestPosition = transform.position;
        float FurthestPathPosition = 0f;

        for (int i = 0; i < 8; i++)
        {
            // Calculates the angle to spread points around the target position
            float Angle = i * (360f / 8);
            Vector3 DirectionalOffset = Quaternion.Euler(0, Angle, 0) * Vector3.forward;

            Vector3 AlternatePosition = PlayerRef.transform.position + DirectionalOffset * (MinFollowDistance + 2);

            if (NavMesh.SamplePosition(AlternatePosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                float DistanceToPlayer = Vector3.Distance(hit.position, PlayerRef.transform.position);

                if (DistanceToPlayer >= FurthestPathPosition && !IsPathInvalid(hit.position))
                {
                    BestPosition = hit.position;
                    FurthestPathPosition = DistanceToPlayer;
                }
            }
        }
        return BestPosition;
    }

    private void HandleAttack()
    {
        if (!SeenPlayer) { return; }
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
            //NavMeshRef.SetDestination(TargetedPosition);
            RigidbodyRef.useGravity = false;
        }
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(2.35f);
        CanAttack = true;
    }

}
