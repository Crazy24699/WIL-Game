using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SeanAI : BaseEnemy
{
    [SerializeField]private bool Attacking = false;

    [SerializeField]private bool Retreat = false;
    private bool KeepAttackDistance = false;
    [SerializeField]private bool Stalking = false;

    //Stalking Floats
    [SerializeField] private float MaxStalkDistance;
    [SerializeField] private float StalkingRadius;
    [SerializeField] private float StalkingSpeed;
    private float CurrentStalkAngle;
    private float StalkTime=2.75f;
    private float StalkingTimer;

    [SerializeField] private float MinAttackDistance;

    private Vector3 IntialStalkingPosition;

    private void Start()
    {
        BaseStartup();
    }

    protected override void CustomStartup()
    {
        base.CustomStartup();
        MaxHealth = 10;
        BaseMoveSpeed = 15;
        EnemyObjectRef = this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerTarget = PlayerRef.transform;

        WorldHandlerScript = FindObjectOfType<WorldHandler>();


        StartupRan = true;
        Alive = true;
        CanAttack = true;
        //remove this   ;
        RetreatPosition = transform.position;
        StalkingTimer = StalkTime;
    }


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
        
    }

    private void Update()
    {
        if (!StartupRan) { return; }

        if(Input.GetKeyDown(KeyCode.L))
        {
            //NavMeshRef.isStopped = true;

        }
        RetreatPosition.y = transform.position.RoundVector(2).y;

        CurrentPosition = transform.position.RoundVector(2);
        CurrentPlayerDistance = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);

        RotateToTarget();
        HandleBehaviour();
        //if (KeepAttackDistance)
        //{
            
        //    //KeepOrbitDistance();
        //    return;
        //}

        //HandleRetreating();
        //if(Retreat) { return; }
        //Attack();

        //KeepChosenDistance();
        //HandleOrbit();

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


        if (!OnAttackingList)
        {
            Orbiting = true;
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
            if (CurrentPlayerDistance > MaxAttackDistance) 
            {
                NavMeshRef.isStopped = false;
                Debug.Log("SetDestination");
                NavMeshRef.SetDestination(PlayerTarget.transform.position);
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
            NavMeshRef.isStopped = true;
            NavMeshRef.velocity = Vector3.zero;
            CanAttack = false;
            Debug.Log("attacked");

            StartCoroutine(RetreatDelay());
            return;
        }
        NavMeshRef.isStopped = false;
        NavMeshRef.SetDestination(PlayerTarget.transform.position);
        Debug.Log("182");

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
        //if(Attacking) { return; }
        //if(CurrentPlayerDistance<=MinStalkDistance)
        //{
        //    CanAttack = true;
        //    StopCoroutine(AttackCooldown());
        //    Attack();
        //    Vector3 RandomPoint = Random.insideUnitCircle;

        //    NavMeshRef.isStopped = false;
        //    RandomPoint += PlayerTarget.transform.position;
        //    Retreat = true;
        //    NavMeshRef.SetDestination(Vector3.zero);
        //}

        if (CurrentPlayerDistance < MaxStalkDistance - 10) 
        {
            NavMeshRef.isStopped = false;
            PlayerDirection = this.transform.position - PlayerTarget.transform.position;
            Vector3 NewPosition = (PlayerDirection * 5) + transform.position;
            NavMeshRef.SetDestination(NewPosition);
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

        NavMeshRef.isStopped = false;
        NavMeshRef.SetDestination(RetreatPosition);

        /*
        if (RetreatPosition==Vector3.zero)
        {
            Vector3 RandomPoint = Random.insideUnitCircle;
            Vector3 RetreatDirection = new Vector3(RandomPoint.x, 0, RandomPoint.y).normalized;

            RandomRetreatPosition = PlayerTarget.transform.position + RetreatDirection * 20;
            RetreatPosition = RandomRetreatPosition.RoundVector(2);

        }

        RetreatPosition.y = transform.position.RoundVector(2).y;
        */

        //NavMeshRef.isStopped = false;
        //NavMeshRef.SetDestination(RetreatPosition);
        //
        if (CurrentPosition == RetreatPosition)
        {
            Debug.Log("Cunt nugget");
            //RetreatPosition = Vector3.zero;
            Retreat = false;
            Stalking = true;
            StartCoroutine(AttackCooldown());
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

    public IEnumerator RetreatDelay()
    {
        IntialStalkingPosition = transform.position.RoundVector(2);
        Debug.Log("retreat");
        yield return new WaitForSeconds(1.25f);
        Retreat = true;
        Stalking = false;
    }

}
