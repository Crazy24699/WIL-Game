using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BTBaseEnemy : BaseEnemy
{
    [SerializeField] public int SpireAreaNumber;

    [Space(5)]
    //[SerializeField] protected float CurrentImmunityTime;
    [Space(3)]

    [Space(8)]
    protected float OutOfRangeTimer = 4.25f;
    [HideInInspector] public float CurrentRangeTime;

    [Space(5)]
    [SerializeField] public Transform WaypointPosition;
    [SerializeField] protected Transform WaypointParent;


    [Header("Vectors"), Space(5)]
    //Vectors
    [SerializeField] protected Vector3 PositionLock;
    protected Vector3 ViewLock;

    [HideInInspector] public BTNodeBase RootNode;
    [HideInInspector] public List<BTNodeBase> AllNodeChoices;

    [HideInInspector] public SpireObject SpireLoaction;
    public SpireParent SpireParentScript;

    protected override void CustomStartup()
    {
        CurrentRangeTime = OutOfRangeTimer;

        EnemyObjectRef = this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerTarget = PlayerRef.transform;

        WorldHandlerScript = FindObjectOfType<WorldHandler>();

        if (NavMeshRef == null)
        {
            NavMeshRef = GetComponent<NavMeshAgent>();
        }
        HealthStartup();
        
        if (MaxFollowDistance <= 0)
        {
            Debug.LogError("Max Follow Distance not set on: " + this.name);
            MaxFollowDistance = 10.58f;
        }
        PopulateSpire();

        StartupRan = true;
        Alive = true;

    }

    private void PopulateSpire()
    {
        
        List<SpireObject> SpireListChosen = SpireParentScript.SpireOptions.ToList();
        Debug.Log(SpireListChosen.Count);
        SpireLoaction = SpireListChosen[Random.Range(0, SpireListChosen.Count)];
        WaypointParent = SpireLoaction.transform.GetComponentInParent<SpireObject>().WaypointSpot;
        WaypointPosition = WaypointParent;
        Debug.Log("This");
    }


    public void DisableAttack()
    {
        Vector3 NewPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z) - (Vector3.forward * -2);
        transform.position = NewPosition;
        CanAttackPlayer = false;
        StartCoroutine(AttackCooldown(3.5f));
    }

    public void RemoveFromAttackList()
    {
        WorldHandlerScript.EnemiesAttacking.Remove(this.gameObject);
    }

    public void HandlePlayerRange()
    {

        CurrentPlayerDistance = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);
        OutOfAttackRange = CurrentPlayerDistance > MaxAttackDistance ? true : false;

        if (SeenPlayer)
        {
            Vector3 TargetDirection = PlayerRef.transform.position - this.transform.position;
            //Debug.DrawRay(this.transform.position, TargetDirection, Color.blue, 50);
            if (Physics.Raycast(this.transform.position, TargetDirection, 50, PlayerLayer)) 
            {
                if (CurrentRangeTime < OutOfRangeTimer) { CurrentRangeTime = OutOfRangeTimer; }

                //Debug.DrawRay(this.transform.position, TargetDirection, Color.red, 50);
                return;
            }
        }
       
        //Draw a raycast to the player, if it hits something before the player then the enemy can not see the player

        if (CurrentPlayerDistance >= MaxFollowDistance)
        {
            CurrentRangeTime -= Time.deltaTime;
            if (CurrentRangeTime <= 0)
            {
                CurrentRangeTime = 0;
                SeenPlayer = false;
                PlayerEscaped = true;
                PatrolActive = true;
                Debug.Log("Patrol Active");
            }
        }

        if (PlayerEscaped && CurrentRangeTime != OutOfRangeTimer)
        {
            CurrentRangeTime = OutOfRangeTimer;
        }
        //Debug.Log(CurrentPlayerDistance);

    }

   
    public void SetDestination(Transform ObjectLocation)
    {
        NavMeshRef.SetDestination(ObjectLocation.transform.position);
        CurrentTarget = ObjectLocation;
        NavMeshRef.isStopped = false;
    }

    public void NewPatrolPoint(SpireObject ChosenSpire)
    {

        StartCoroutine(MoveToNewPointDelay(ChosenSpire));
    }

    protected IEnumerator MoveToNewPointDelay(SpireObject ChosenSpire)
    {
        yield return new WaitForSeconds(1.25f);
        int RandomSpire = Random.Range(0, ChosenSpire.NeighboringSpires.Count);

        WaypointParent = ChosenSpire.NeighboringSpires[RandomSpire].transform;
        WaypointPosition = ChosenSpire.NeighboringSpires[RandomSpire].WaypointSpot;
        SpireLoaction = ChosenSpire.NeighboringSpires[RandomSpire].ThisSpire;

        SetDestination(ChosenSpire.NeighboringSpires[RandomSpire].WaypointSpot);
        //Debug.Log("Patrol");
    }

    protected void LockForAttack()
    {
        //ViewLock = transform.rotation.eulerAngles;
        NavMeshRef.isStopped = true;
        //PositionLock = transform.position;
        //IsAttacking = true;
    }

    public void ChangeLockState(bool Locked)
    {
        NavMeshRef.isStopped = Locked;

        //IsAttacking = Locked;
    }

    protected virtual void Die()
    {
        Alive = false;
        WorldHandlerScript.SetNextActive(this.gameObject);
    }

}
