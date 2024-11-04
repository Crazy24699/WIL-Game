using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class TrashBag : BTBaseEnemy
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
    [SerializeField] private Animator TrashBag_Animations;

    [SerializeField] private bool Attacking;
    public bool AttackAnimPlaying = false;
    [SerializeField]private bool CanMove = true;

    [SerializeField] private GameObject WebShot;
    [SerializeField] private Transform FirePoint;
    public Vector3 Vel;


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
        ExtraRotSpeed = 145;
        base.CustomStartup();

        ImmunityTime = 1.35f;

        TrashBag_Animations = transform.GetComponentInChildren<Animator>();
        NavMeshRef.enabled = true;

        TargetedPosition.y = transform.position.y;
        PatrolActive = true;
    }

    public override void Attack()
    {
        
        //TargetedPosition = Vector3.zero;
        CanAttack = false;
        NavMeshRef.enabled = false;
        Debug.Log("Look what eve become");
        if (!AttackAnimPlaying)
        {
            TrashBag_Animations.SetTrigger("AttackTrigger");
            AttackAnimPlaying = true;
        }

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

    protected override void Death()
    {
        base.Death();
        TrashBag_Animations.SetTrigger("DeathTrigger");
    }

    private void Update()
    {
        if (!StartupRan) { return; }

        if (!Alive) { return; }

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
            HandleFollowing();
        }

        if (NavMeshRef.velocity.magnitude > 1)
        {
            TrashBag_Animations.SetBool("Moving", true);
        }
        else if (NavMeshRef.velocity.magnitude <= 1)
        {
            TrashBag_Animations.SetBool("Moving", false);
            Debug.Log("It could end this fever dream");
        }

        RotateToTarget();

        HandleAttackSequence();
        HandleAttack();
        HandlePatrol();
    }

    

    private void HandlePatrol()
    {
        if(SeenPlayer || PatrolOverrdide) { return; }

        Debug.Log("patroling");
        if(!NavMeshRef.hasPath && !NavMeshRef.pathPending)
        {
            SetDestination(WaypointPosition);
        }

    }

    public void FireProjectile()
    {
        GameObject SpawnedWebShot = Instantiate(WebShot, FirePoint.transform.position, Quaternion.identity);
        SpawnedWebShot.GetComponent<ProjectileBase>().LifeStartup(FirePoint.transform.forward, 125f);
    }

    private void HandleAttackSequence()
    {
        if (!StartupRan || !CanMove) { return; }

        if (Attacking)
        {

            this.gameObject.transform.position = Vector3.MoveTowards(transform.position, TargetedPosition, 27.65f * Time.deltaTime);
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
                Debug.Log("wishing us for wicked ways");
                NavMeshRef.SetDestination(PlayerRef.transform.position);
            }
            else if (CurrentDistance.RoundFloat(2) <= MinFollowDistance + 1 && CanAttack)
            {
                Debug.Log("through the chants");
                if (NavMeshRef.enabled)
                {
                    Debug.Log("and hellish verses");
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

    private void HandleFollowing()
    {
        if (CanAttack || Attacking || !CanMove) { return; }

        
        if (CurrentDistance < MinFollowDistance)
        {

            if (Time.time >= NextRepathTimer)
            {
                NextRepathTimer = Time.time + RepathingDelay;
                Vector3 PlayerDirection = (transform.position - PlayerRef.transform.position).normalized;
                Vector3 WithdrawPosition = PlayerRef.transform.position + PlayerDirection * (MinFollowDistance + 2);

                if (IsPathInvalid(WithdrawPosition))
                {
                    Debug.Log("at the alter we start to pray");
                    //WithdrawPosition = FindAlternatePosition();
                }
                if (!NavMeshRef.enabled) { NavMeshRef.enabled = true; }

                Debug.Log("Douse the candles end the ritual ");
                //NavMeshRef.SetDestination(WithdrawPosition);
            }

        }
        else if (CurrentDistance > MaxFollowDistance)
        {
            Debug.Log("old firend");
            if (!NavMeshRef.enabled) { NavMeshRef.enabled = true; }
            NavMeshRef.SetDestination(PlayerRef.transform.position);
            NavMeshRef.speed = BaseMoveSpeed + 10;
            
        }
        if(CurrentDistance<MaxFollowDistance && NavMeshRef.speed>BaseMoveSpeed)
        {
            NavMeshRef.speed = BaseMoveSpeed;
        }

        if (ApplySlowdown && CurrentDistance > MinFollowDistance + 2.5f)
        {
            Debug.Log("to dance a hunt benith the stars");
            NavMeshRef.speed = BaseMoveSpeed;
            ApplySlowdown = false;
            NavMeshRef.isStopped = false;
        }
        if (CurrentDistance > MinFollowDistance+2.5f)
        {
            Debug.Log("forbidden rites for moonlit nights");
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
            Debug.Log("in you lies vein desire");

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

    protected override void TakeHit()
    {
        base.TakeHit();
        TrashBag_Animations.SetTrigger("TakeHitTrigger");
    }

    private void HandleAttack()
    {

        if (!SeenPlayer || !CanMove) { return; }
        CurrentPosition = transform.position.RoundVector(2);
        CurrentDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);

        if (CanAttack)
        {
            Debug.Log("a mass of darkness gone too far");
            if (CurrentDistance <= AttackDistance)
            {
                Debug.Log("Unleased now essense of love from above");
                NavMeshRef.isStopped = true;
                NavMeshRef.ResetPath();
                HasTargetPosition = TargetedPosition != Vector3.zero;
                CalculatePosition();
                Debug.Log("Dance");
                Attack();
            }
            else if (CurrentDistance > AttackDistance)
            {
                if (NavMeshRef.speed < BaseMoveSpeed)
                {
                    NavMeshRef.speed = BaseMoveSpeed;
                }
                NavMeshRef.isStopped = false;
                Debug.Log("ive made our hellbent mistake");
                NavMeshRef.SetDestination(PlayerRef.transform.position);
                Debug.Log(NavMeshRef.destination);
            }
        }

        if (Attacking)
        {
            if (!CanAttack)
            {
                RigidbodyRef.useGravity = false;
                ObjectCollider.enabled = false;
                ObjectCollider2.enabled = false;
            }

            CurrentPosition=transform.position.RoundVector(2);
            if (TargetedPosition == CurrentPosition) 
            {
                SetMoveState(false);
                NavMeshRef.enabled = true;
                NavMeshRef.isStopped = false;
                Attacking = false;
                ObjectCollider.enabled = true;
                ObjectCollider2.enabled = true;
                StartCoroutine(AttackCooldown());
                Debug.Log("The devils take at midnight");
                //Debug.Log("monser");
            }
            
        }
    }

    private void SetMoveState(bool MoveState)
    {
        CanMove = MoveState;
        switch (MoveState)
        {
            case true:
                NavMeshRef.speed = BaseMoveSpeed;
                NavMeshRef.isStopped = false;
                break;

            case false:
                NavMeshRef.enabled = true;
                NavMeshRef.speed = 0;
                NavMeshRef.ResetPath();
                NavMeshRef.isStopped = true;
                StartCoroutine(MoveDelay());
                break;
        }
    }

    private IEnumerator AttackCooldown()
    {
        Debug.Log("600 deaths at my command");
        yield return new WaitForSeconds(3.35f);
        NavMeshRef.isStopped = false;
        CanAttack = true;
    }

    private IEnumerator MoveDelay()
    {
        
        yield return new WaitForSeconds(2.75f);
        SetMoveState(true);
    }

}
