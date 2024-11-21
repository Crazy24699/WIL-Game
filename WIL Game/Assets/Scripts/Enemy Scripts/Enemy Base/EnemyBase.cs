
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{

    #region Ints
    protected int MaxHealth = 2;
    [SerializeField] protected int CurrentHealth;
    protected int CurrentWayPoint = 0;
    [SerializeField] protected int SpireAreaNumber;
    #endregion

    #region Floats
    [Space(5)]
    [SerializeField] protected float ImmunityMaxTime;
    [SerializeField] protected float KnockbackPower;

    [SerializeField] protected float KnockbackTimer = 4f;
    protected float KnockbackTime;
    //[SerializeField] protected float CurrentImmunityTime;
    [Space(3)]
    [SerializeField] protected float BaseMoveSpeed;
    [SerializeField] protected float CurrentMoveSpeed;
    public float NextWaypointDistance = 5f;

    [Space(8)]
    public float CurrentPlayerDistance;
    public float MaxFollowDistance;
    public float MaxAttackDistance;

    public float OutOfRangeTimer;
    public float CurrentRangeTime;
    #endregion

    #region Gameobjects and transforms
    [Space(15)]
    protected GameObject EnemyObjectRef;
    protected GameObject PlayerRef;

    [Space(5)]
    [SerializeField] public Transform WaypointPosition;
    [SerializeField] protected Transform WaypointParent;
    /*[HideInInspector]*/ public Transform PlayerTarget;
    #endregion

    [Header("Booleans"), Space(5)]
    public bool IsAttacking;
    #region Bools
    protected bool CanTakeDamage = true;
    [SerializeField] protected bool ReduceKnockbackForce = true;
    /*[HideInInspector]*/public bool SeenPlayer = false;
    [HideInInspector] public bool PlayerEscaped = false;
     public bool OutOfAttackRange = false; 

    protected bool StartupRan = false;
    [SerializeField]protected bool Alive = false;
    public bool PatrolActive = false;

    public bool CanAttackPlayer;
    //if the Ai is currently attacking the player, used for making sure it cant move or chain attacks
    
    #endregion

    [Header("Vectors"), Space(5)]
    //Vectors
    [SerializeField]protected Vector3 PositionLock;
    protected Vector3 ViewLock;

    #region Scripts
    [HideInInspector] public BTNodeBase RootNode;
    [HideInInspector] public List<BTNodeBase> AllNodeChoices;

    //Patrol Scripts
    //[HideInInspector] public AIDestinationSetter DestinationSetterScript;
    //[HideInInspector] public Seeker AISeeker;

    public NavMeshAgent NavMeshRef;
    [HideInInspector]public SpireObject SpireLoaction;
    protected WorldHandler WorldHandlerScript;
    [SerializeField] protected Slider HealthBar;

    [Space(10)]
    protected Rigidbody Rigidbody;

    #endregion

    public IEnumerator BaseStartup()
    {
        //small delay to allow the world to start up
        yield return new WaitForSeconds(1);

        CurrentRangeTime = OutOfRangeTimer;

    
        Rigidbody = GetComponent<Rigidbody>();
        EnemyObjectRef = this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerTarget = PlayerRef.transform;

        WorldHandlerScript = FindObjectOfType<WorldHandler>();

        PopulateSpire();

        if (NavMeshRef == null)
        {
            NavMeshRef = GetComponent<NavMeshAgent>();
        }
        CustomStartup();
        HealthStartup();

        StartupRan = true;
        Alive = true;
    }

    private void PopulateSpire()
    {
        SpireAreaNumber = 0;
        
        List<SpireObject> SpireListChosen = WorldHandlerScript.AllSpires.ElementAt(0).Value;
        Debug.Log(SpireListChosen.Count);
        SpireLoaction = SpireListChosen[Random.Range(0,SpireListChosen.Count)];
        WaypointParent = SpireLoaction.transform.GetComponentInParent<SpireObject>().WaypointSpot;
        WaypointPosition = WaypointParent;

    }

    private void HealthStartup()
    {
        CurrentHealth = MaxHealth;

        CanTakeDamage = true;

        if (HealthBar == null)
        {
            HealthBar = transform.GetComponentInChildren<Slider>();
        }
        HealthBar.maxValue = MaxHealth;
    }

    protected virtual void CustomStartup()
    {

    }

    public int HandleHealth(int HealthChange)
    {
        if ((CurrentHealth > 0 || CurrentHealth <= MaxHealth) && CanTakeDamage)
        {
            CurrentHealth -= HealthChange;
            StartCoroutine(ImmunityTimer());
            Debug.Log(HealthChange+"son" + CurrentHealth);
            //Play health gained particle effect
            return CurrentHealth;
        }
        return 0;
    }


    #region Health and taking damage: Immunity Timer. Apply Knockback. Handle Force
    public IEnumerator ImmunityTimer()
    {
        CanTakeDamage = false;
        yield return new WaitForSeconds(ImmunityMaxTime);
        CanTakeDamage = true;
    }

    public void ApplyKnockback()
    {
        Vector3 ForceDirection = (PlayerRef.transform.position - transform.position).normalized;
        Rigidbody.AddForce(ForceDirection * -1 * KnockbackPower, ForceMode.Impulse);
        Debug.Log("Love gone wrong");
    }

    public void DisableAttack()
    {
        Vector3 NewPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z) - (Vector3.forward * -2);
        transform.position = NewPosition;
        CanAttackPlayer = false;
        StartCoroutine(AttackCooldown(3.5f));
    }

    protected void HandleForce()
    {
        //Debug.Log(Rigidbody.velocity.x + " " + Mathf.Abs(Rigidbody.velocity.y) +" "+Mathf.Abs(Rigidbody.velocity.z));
        if (Mathf.Abs(Rigidbody.velocity.x) <= 2 && Mathf.Abs(Rigidbody.velocity.y) <= 2 && Mathf.Abs(Rigidbody.velocity.z) <= 2)
        {

            ReduceKnockbackForce = false;
        }
        else if (Mathf.Abs(Rigidbody.velocity.x) > 2 || Mathf.Abs(Rigidbody.velocity.y) > 2 || Mathf.Abs(Rigidbody.velocity.z) > 2)
        {
            ReduceKnockbackForce = true;
        }
        if (!ReduceKnockbackForce)
        {
            return;
        }

        if (ReduceKnockbackForce)
        {
            KnockbackTime -= Time.deltaTime;
            float CurrentXVelocity = Mathf.Lerp(Rigidbody.velocity.x, 0.0f, 0.005f);
            float CurrentYVelocity = Mathf.Lerp(Rigidbody.velocity.y, 0.0f, KnockbackTime / KnockbackTimer);
            float CurrentZVelocity = Mathf.Lerp(Rigidbody.velocity.z, 0.0f, 0.005f);

            Rigidbody.velocity = new Vector3(CurrentXVelocity, CurrentYVelocity, CurrentZVelocity);
        }
    }
    #endregion
    public void HandlePlayerRange()
    {
        CurrentPlayerDistance = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);
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
        OutOfAttackRange = CurrentPlayerDistance > MaxAttackDistance ? true : false;
    }

    #region Pathfinding and controlls: SetDest. OnPathComplete. UpdatePath. HandlePatrol. NewPatrolPoint. 
    public void SetDestination(Transform ObjectLocation)
    {
        NavMeshRef.SetDestination(ObjectLocation.transform.position);

        NavMeshRef.isStopped = false;
    }

    public void NewPatrolPoint(SpireObject ChosenSpire)
    {
        int RandomSpire = Random.Range(0, ChosenSpire.NeighboringSpires.Count);

        WaypointParent = ChosenSpire.NeighboringSpires[RandomSpire].transform;
        WaypointPosition = ChosenSpire.NeighboringSpires[RandomSpire].WaypointSpot;
        SpireLoaction = ChosenSpire.NeighboringSpires[RandomSpire].ThisSpire;

        SetDestination(ChosenSpire.NeighboringSpires[RandomSpire].WaypointSpot);
        Debug.Log("Patrol");

    }

    #endregion

    public virtual void Attack()
    {

    }


    protected IEnumerator AttackCooldown(float AttackCooldown)
    {
        yield return new WaitForSeconds(AttackCooldown);
        CanAttackPlayer = true;
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

    protected void Die()
    {
        Alive = false;
        //WorldHandlerScript.SetNextActive(this.gameObject);
    }


}
