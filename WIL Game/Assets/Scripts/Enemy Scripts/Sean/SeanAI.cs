using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SeanAI : BTBaseEnemy
{
    [SerializeField]private bool Attacking = false;

    [SerializeField]private bool Retreat = false;
    private bool KeepAttackDistance = false;
    [SerializeField]private bool Stalking = false;
    private bool RetreatDelayActive = false;
    public bool AttackUp=false;

    //Stalking Floats
    [SerializeField] private float MaxStalkDistance;
    [SerializeField] private float StalkingRadius;
    [SerializeField] private float StalkingSpeed;
    private float CurrentStalkAngle;
    private float StalkTime=2.75f;
    private float StalkingTimer;
    [SerializeField] private float WaitTime;

    [SerializeField] private float MinAttackDistance;

    private Vector3 IntialStalkingPosition;
    private Vector3 CurrentVelocity;

    private Animator SeanAnimations;

    private bool IsMoving = false;
    [SerializeField] private bool TestOverride;

    private void Start()
    {
        if(TestOverride)
        {
            Debug.LogError("Remove this when the enemy is done");
            BaseStartup();
        }
    }

    protected override void CustomStartup()
    {

        SeanAnimations = GetComponentInChildren<Animator>();
        SeanAnimLink Link = transform.GetComponentInChildren<SeanAnimLink>();
        Link.SetLink(this);
        MaxHealth = 10;
        BaseMoveSpeed = NavMeshRef.speed ;
        base.CustomStartup();

        EnemyObjectRef = this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerTarget = PlayerRef.transform;

        WorldHandlerScript = FindObjectOfType<WorldHandler>();

        ImmunityTime = 0.57f;

        StartupRan = true;
        Alive = true;
        CanAttack = true;
        //remove this   ;
        RetreatPosition = transform.position;
        StalkingTimer = StalkTime;
        PatrolActive = true;
    }

    private void HandleMoveAnim()
    {

        if(IsAttacking|| TakingDamage) { return; }
        Debug.Log("past days gone again");
        if (CurrentVelocity.magnitude <= 2)
        {
            IsMoving = false;
            if (!Attacking || TakingDamage)
            {
                EnemyAudioManager.PlaySound(EnemySoundManager.SoundOptions.Silence);
            }
        }
        else if(CurrentVelocity.magnitude > 2)
        {
            IsMoving = true;
            EnemyAudioManager.PlaySound(EnemySoundManager.SoundOptions.Moving);
        }
        SeanAnimations.SetBool("Moving", IsMoving);
        
    }

    public void ImpactSound()
    {
        Debug.Log("attack set");
        EnemyAudioManager.PlaySound(EnemySoundManager.SoundOptions.Attack);
    }

    

    /*
    public override void Attack()
    {
        if(!CanAttack || !OnAttackingList) { return; }
        
        if(OnAttackingList && CanAttack && !Stalking) 
        { 
            Stalking = true; 
        }

        Attacking = true;
        Retreat = false;
        NavMeshRef.SetDestination(PlayerTarget.transform.position);
        Debug.Log("64");

        if (CurrentPlayerDistance <= MinAttackDistance)
        {
            NavMeshRef.isStopped = true;
            NavMeshRef.velocity = Vector3.zero;
            CanAttack = false ;
            //Debug.Log("god hunter");
            //Trigger Animation
            //StartCoroutine(AttackCooldown());
            StartCoroutine(RetreatDelay());
        }
        AttackUp = CanAttack;
    }
    */
    private void Update()
    {
        if (!StartupRan) { return; }
        CurrentVelocity = NavMeshRef.velocity;
        HandleMoveAnim();
        RetreatPosition.y = transform.position.RoundVector(2).y;

        CurrentPosition = transform.position.RoundVector(2);
        CurrentPlayerDistance = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);

        RotateToTarget();
        HandleOrbit();
        HandlePatrol();
        HandleBehaviour();
    }

    private void HandlePatrol()
    {
        if (SeenPlayer || PatrolOverrdide) { return; }

        Debug.Log("patroling");
        if (!NavMeshRef.hasPath && !NavMeshRef.pathPending)
        {
            SetDestination(WaypointPosition);
        }

    }


    private Vector3 HandleCirlcing()
    {
        float X_Coord = Mathf.Cos(CurrentStalkAngle) * StalkingRadius;
        float Z_Coord = Mathf.Sin(CurrentStalkAngle) * StalkingRadius;

        Vector3 PositionOffset = new Vector3(X_Coord, 0, Z_Coord);
        return PlayerTarget.transform.position + PositionOffset;
    }

    private void HandleBehaviour() 
    {

        OnAttackingList = SeenPlayer;
        if (!SeenPlayer)
        {
            PatrolActive = true;
        }
        else if(SeenPlayer)
        {
            PatrolActive = false;
        }

        if(PatrolActive) { return; }
        if (!OnAttackingList)
        {


            Orbiting = true;
            if (CheckIfSlotFree())
            {
                WorldHandlerScript.EnemiesAttacking.Add(this.gameObject);
            }
            Debug.Log("Orbiting");
            return;
        }

        if (Retreat)
        {
            HandleRetreating();
            return;
        }

        if (OnAttackingList)
        {
            if (CurrentPlayerDistance > MaxAttackDistance && !Stalking) 
            {
                NavMeshRef.isStopped = false;
                Debug.Log("SetDestination");
                NavMeshRef.SetDestination(PlayerTarget.transform.position);
                CurrentTarget = PlayerTarget;
                Debug.Log("143");

                return;
            }
            if(CurrentPlayerDistance < MaxAttackDistance && !CanAttack)
            {
                //NavMeshRef.isStopped = true;
                //NavMeshRef.velocity = Vector3.zero;
                //Debug.Log("Stopping");
            }

            if(CanAttack)
            {
                Debug.Log("look back");
                DoAttack();
                
                return;
                
            }
            else if (!CanAttack)
            {
                if (!Stalking) { return; }
                Debug.Log("Stalking Time");
                StalkingTime();
            }
        }
    }

    private void DoAttack()
    {
        if (CurrentPlayerDistance <= MinAttackDistance)
        {
            SeanAnimations.SetTrigger("AttackTrigger");
            NavMeshRef.isStopped = true;
            NavMeshRef.velocity = Vector3.zero;
            CanAttack = false;
            Debug.Log("attacked");

            //StartCoroutine(RetreatDelay());
            return;
        }
        NavMeshRef.isStopped = false;
        NavMeshRef.SetDestination(PlayerTarget.transform.position);
        Debug.Log("182");

    }

    public void RetreatWaitTime()
    {
        StartCoroutine(RetreatDelay());
    }

    private void StalkingTime()
    {
        if (StalkingTimer > 0)
        {
            StalkingTimer -= Time.deltaTime;
            if(NavMeshRef.isStopped) { NavMeshRef.isStopped = false; }

            Vector3 StalkingPosition = HandleCirlcing();
            Debug.Log("193");
            NavMeshRef.SetDestination(StalkingPosition);
            RigidbodyRef.velocity = Vector3.zero;
            Debug.Log("Huh 19");

            CurrentStalkAngle += StalkingSpeed * Time.deltaTime;

            if (CurrentStalkAngle >= 360f)
            {
                Debug.Log("Huh 190");
                CurrentStalkAngle = 0;
            }

            if (StalkingTimer <= 0)
            {
                Debug.Log("Huh 1950");
                RetreatPosition = StalkingPosition;
                StalkingTimer = StalkTime;
                Stalking = false;
            }
        }
    }

    private void KeepChosenDistance()
    {

        if (CurrentPlayerDistance < MaxStalkDistance - 10) 
        {
            NavMeshRef.isStopped = false;
            PlayerDirection = this.transform.position - PlayerTarget.transform.position;
            Vector3 NewPosition = (PlayerDirection * 5) + transform.position;
            NavMeshRef.SetDestination(NewPosition);
            if(RigidbodyRef.velocity.magnitude > 0)
            {
            }
                RigidbodyRef.velocity = Vector3.zero;
            Debug.Log("231");
        }
        else if(CurrentPlayerDistance > MaxStalkDistance - 10)
        {
            NavMeshRef.isStopped = true;
            NavMeshRef.velocity = Vector3.zero;
        }

    }

    

    private void HandleOrbit()
    {
        Debug.Log("Riviers has run dry");
        if (Orbiting)
        {
            RotateToTarget();
            KeepOrbitDistance();
            return;
        }
    }

    private void HandleRetreating()
    {
        if (!Retreat || Stalking) { return; }
        Debug.Log("huint uyou down");
        Vector3 RandomRetreatPosition = Vector3.zero;
        RetreatPosition=RetreatPosition.RoundVector(2);

        HandleRetreatLocation();

        NavMeshRef.isStopped = false;
        NavMeshRef.SetDestination(RetreatPosition);


        if (CurrentPosition == RetreatPosition)
        {
            //RetreatPosition = Vector3.zero;
            Retreat = false;
            Stalking = true;
            StartCoroutine(AttackCooldown());
        }
    }

    //Checks if the current retreat location is too far away from the player
    //if it is then the program will change the retreat position to be within
    //range of the player

    private void HandleRetreatLocation()
    {
        Vector2 PlayerPositionVect2 = PlayerTarget.transform.position;
        Vector2 RetreatPositionVect2 = RetreatPosition;

        float RetreatPositionDistance = Vector2.Distance(PlayerPositionVect2, RetreatPositionVect2);
        if (RetreatPositionDistance > MaxFollowDistance) 
        {
            UpdateRetretPosition = true;
            PathfindingRetreat();
            Debug.Log("The fuckening");
        }


    }

    private IEnumerator AttackCooldown()
    {
        //CanAttack = false;
        //Attacking = false;

        Debug.Log("see this challenge through   ");
        yield return new WaitForSeconds(3.25f);
        Debug.Log("CooldownRan");
        CanAttack = true;
    }

    private void HandleRetreatAndStalkingSwitch()
    {
        if (!RetreatDelayActive) { return; }

        //Timer logic goes here

    }
    
    public IEnumerator RetreatDelay()
    {
        IntialStalkingPosition = transform.position.RoundVector(2);
        RetreatPosition = IntialStalkingPosition;
        Debug.Log("retreat");
        yield return new WaitForSeconds(WaitTime);
        Retreat = true;
        Stalking = false;
    }

}
