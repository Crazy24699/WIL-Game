using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TrashBag : BaseEnemy
{

    [SerializeField] private Vector3 TargetedPosition;

    [SerializeField] private bool HasTargetPosition = false;
    private bool ApplySlowdown = false;

    private float TravelDistance = 5;
    [SerializeField] private float CurrentDistance;
    [SerializeField] private float AttackDistance;
    [SerializeField] private float MinFollowDistance;
    private float RepathingDelay = 1.35f;
    private float ObstacleCheckRadius = 1.75f;
    private float NextRepathTimer;

    [SerializeField] private Collider ObjectCollider;
    [SerializeField] private Collider ObjectCollider2;
    [SerializeField] private Animator TrshBag_Animations;

    [SerializeField] private bool Attacking;

    [SerializeField] private GameObject WebShot;
    [SerializeField] private Transform FirePoint;


    private void Start()
    {
        //Debug.LogError("Remove this when the enemy is done");
        BaseStartup();
        TargetedPosition.y = transform.position.y;
    }

    protected override void CustomStartup()
    {
        MaxHealth = 8;
        CurrentHealth = MaxHealth;
        BaseMoveSpeed = 30;
        ExtraRotSpeed = 145;

        TrshBag_Animations = transform.GetComponentInChildren<Animator>();
        NavMeshRef.enabled = true;
    }

    public override void Attack()
    {
        
        //TargetedPosition = Vector3.zero;
        CanAttack = false;
        NavMeshRef.enabled = false;
        Debug.Log("Look what eve become");
        

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
        TravelDistance = TargetDistance.RoundFloat(2) * 2;
        Vector3 CalculatedPosition = transform.position + (Direction.normalized * TravelDistance);
        
        TargetedPosition = new Vector3(CalculatedPosition.x+5, TargetedPosition.y, CalculatedPosition.z+5).RoundVector(2);
        

        Debug.DrawRay(transform.position, Direction*20,Color.blue,500);

    }

    private void Update()
    {
        if (!StartupRan) { return; }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireProjectile();
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            CanAttack = true;
            
        }
        if (SeenPlayer)
        {
            CurrentDistance = Vector3.Distance(PlayerRef.transform.position, transform.position).RoundFloat(2);
            MaintainDistance();
        }

        RotateToTarget();

        HandleAttackSequence();
        HandleAttack();
        ////add a check here for on the attack list
        //if (!CanAttack && !Attacking)
        //{

        //}
    }

    [SerializeField]
    private void FireProjectile()
    {
        GameObject SpawnedWebShot = Instantiate(WebShot, FirePoint.transform.position, Quaternion.identity);
        SpawnedWebShot.GetComponent<ProjectileBase>().LifeStartup(transform.up, 125f);
    }

    private void HandleAttackSequence()
    {
        if (!StartupRan) { return; }

        if (Attacking)
        {
            this.gameObject.transform.position = Vector3.MoveTowards(transform.position, TargetedPosition, 20f * Time.deltaTime);
            return;
        }

        if (SeenPlayer)
        {

            //add a check for the attack list
            if(CanAttack )
            {
                if (CurrentDistance > AttackDistance)
                {
                    if (!NavMeshRef.hasPath && !NavMeshRef.pathPending)
                    {
                        NavMeshRef.SetDestination(PlayerRef.transform.position);
                        Debug.Log("be dangerous");
                    }
                }
                Attacking = CurrentDistance <= AttackDistance; 
                
                return;
            }

            if (CurrentDistance.RoundFloat(2) > MinFollowDistance)
            {
                NavMeshRef.enabled = true;
                NavMeshRef.SetDestination(PlayerRef.transform.position);
            }
            else if (CurrentDistance.RoundFloat(2) <= MinFollowDistance + 1 && CanAttack)
            {

                if (NavMeshRef.enabled)
                {
                    NavMeshRef.ResetPath();

                    NavMeshRef.enabled = false;
                    Attacking = true;
                }
                

            }
        }

        if(ApplySlowdown)
        {
            NavMeshRef.speed -= 15 * Time.deltaTime;
        }

        if (!Attacking)
        {
            RotateToTarget();
        }
    }

    private void MaintainDistance()
    {
        if (CanAttack || Attacking) { return; }
        if (CurrentDistance < MinFollowDistance)
        {

            if (Time.time >= NextRepathTimer)
            {
                NextRepathTimer = Time.time + RepathingDelay;
                Vector3 PlayerDirection = (transform.position - PlayerRef.transform.position).normalized;
                Vector3 WithdrawPosition = PlayerRef.transform.position + PlayerDirection * (MinFollowDistance + 2);

                if (IsPathInvalid(WithdrawPosition))
                {
                    //WithdrawPosition = FindAlternatePosition();
                }
                if (!NavMeshRef.enabled) { NavMeshRef.enabled = true; }

                //NavMeshRef.SetDestination(WithdrawPosition);
            }

        }
        else if (CurrentDistance > MaxFollowDistance)
        {
            //Debug.Log("old firend");
            //if (!NavMeshRef.enabled) { NavMeshRef.enabled = true; }
            //NavMeshRef.SetDestination(PlayerRef.transform.position);
        }
        if (ApplySlowdown && CurrentDistance > MinFollowDistance + 2.5f)
        {
            NavMeshRef.speed = BaseMoveSpeed;
            ApplySlowdown = false;
            NavMeshRef.isStopped = false;
        }
        if (CurrentDistance > MinFollowDistance+2.5f)
        {
            if (NavMeshRef.isStopped) { NavMeshRef.isStopped = false; }
            Vector3 FollowDirection = (transform.position - PlayerRef.transform.position).normalized ;
            //Debug.DrawRay(PlayerRef.transform.position, FollowDirection * 15,Color.blue);

            Vector3 FollowPosition = (FollowDirection * 15) + PlayerRef.transform.position;
            NavMeshRef.SetDestination(FollowPosition);
        }
        if (CurrentDistance <= MinFollowDistance && !NavMeshRef.isStopped)
        {
            ApplySlowdown = true;
            NavMeshRef.isStopped = true;
            //NavMeshRef.enabled = false;
            //Debug.Log("out of here");
            //NavMeshRef.enabled = true;
            //NavMeshRef.ResetPath();
        }
    }

    private bool IsPathInvalid(Vector3 TargetPosition)
    {
        // Use a sphere cast to detect if the target position is blocked
        return Physics.SphereCast(transform.position, ObstacleCheckRadius, TargetPosition - transform.position, out RaycastHit Hit) &&
               Hit.collider.gameObject != PlayerRef; 
    }

    private Vector3 FindAlternatePosition()
    {
        Vector3 BestPosition = transform.position;
        float FurthestPathPosition = 0f;

        for (int i = 0; i < 8; i++)
        {
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
        CurrentDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);

        if (CanAttack)
        {
            if (CurrentDistance <= AttackDistance)
            {
                HasTargetPosition = TargetedPosition != Vector3.zero;
                CalculatePosition();
                Debug.Log("Dance");
                Attack();
            }
        }
        if (!CanAttack && Attacking)
        {
            //NavMeshRef.SetDestination(TargetedPosition);
            RigidbodyRef.useGravity = false;
            ObjectCollider.enabled = false;
            ObjectCollider2.enabled = false;
        }

        if (Attacking)
        {
            CurrentPosition=transform.position.RoundVector(2);
            if(TargetedPosition==CurrentPosition)
            {
                NavMeshRef.enabled = true;
                NavMeshRef.isStopped = false;
                Attacking = false;
                StartCoroutine(AttackCooldown());
                //Debug.Log("monser");
            }
            
        }
    }

    private IEnumerator AttackCooldown()
    {
        Debug.Log("600 deaths at my command");
        yield return new WaitForSeconds(10.35f);
        CanAttack = true;
    }

}
