using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SeanAI : BaseEnemy
{
    [SerializeField]private bool Attacking = false;

    [SerializeField]private bool Retreat = false;
    private bool KeepAttackDistance = false;
    [SerializeField]private bool Stalking = false;

    [SerializeField] private float MinAttackDistance;
    [SerializeField] private float MinStalkDistance;
    [SerializeField] private float MaxStalkDistance;


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
    }


    public override void Attack()
    {
        if(!CanAttack || !OnAttackingList) { return; }
        
        if(OnAttackingList && CanAttack) 
        { 
            Stalking = true; 
        }

        Attacking = true;
        Retreat = false;
        NavMeshRef.SetDestination(PlayerTarget.transform.position);
        if (CurrentPlayerDistance <= MinAttackDistance)
        {
            NavMeshRef.isStopped = true;
            NavMeshRef.velocity = Vector3.zero;
            Debug.Log("god hunter");
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

        CurrentPlayerDistance = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);

        RotateToTarget();
        if (KeepAttackDistance)
        {
            
            //KeepOrbitDistance();
            return;
        }

        HandleRetreating();
        if(Retreat) { return; }
        Attack();

        KeepChosenDistance();
        HandleOrbit();

    }

    private void KeepChosenDistance()
    {
        if(Attacking) { return; }
        if(CurrentPlayerDistance<=MinStalkDistance)
        {
            CanAttack = true;
            StopCoroutine(AttackCooldown());
            Attack();
            Vector3 RandomPoint = Random.insideUnitCircle;

            NavMeshRef.isStopped = false;
            RandomPoint += PlayerTarget.transform.position;
            Retreat = true;
            NavMeshRef.SetDestination(Vector3.zero);
        }

        if (CurrentPlayerDistance < MaxStalkDistance - 10) 
        {
            NavMeshRef.isStopped = false;
            PlayerDirection = this.transform.position - PlayerTarget.transform.position;
            Vector3 NewPosition = (PlayerDirection * 5) + transform.position;
            NavMeshRef.SetDestination(NewPosition);
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

        if(RetreatPosition==Vector3.zero)
        {
            RandomRetreatPosition = Random.insideUnitCircle;
            RetreatPosition = RandomRetreatPosition * 20 + transform.position;
            RetreatPosition = new Vector3(RetreatPosition.x, transform.position.y, RetreatPosition.z);
            RetreatPosition = RetreatPosition.RoundVector(2);
        }

        NavMeshRef.isStopped = false;
        NavMeshRef.SetDestination(RetreatPosition);

        if(CurrentPosition==RetreatPosition)
        {
            RetreatPosition = Vector3.zero;
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        CanAttack = false;
        Attacking = false;

        yield return new WaitForSeconds(2.25f);
        CanAttack = true;
    }

    public IEnumerator RetreatDelay()
    {
        
        yield return new WaitForSeconds(1.25f);
        Retreat = true;
    }

}
