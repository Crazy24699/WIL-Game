using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    #region Ints
    protected int MaxHealth = 100;
    [SerializeField] protected int CurrentHealth;
    protected int CurrentWayPoint = 0;
    #endregion

    #region Floats
    [Space(5)]
    [SerializeField] protected float ImmunityMaxTime;
    [SerializeField] protected float KnockbackPower;

    [SerializeField]protected float KnockbackTimer = 4f;
    protected float KnockbackTime;
    //[SerializeField] protected float CurrentImmunityTime;
    [Space(3)]
    [SerializeField]protected float BaseMoveSpeed;
    [SerializeField]protected float CurrentMoveSpeed;
    public float NextWaypointDistance = 5f;
    #endregion

    #region Gameobjects and transforms
    [Space(5)]
    protected GameObject EnemyRef;
    protected GameObject PlayerRef;

    [Space(5)]
    [SerializeField]protected Transform WaypointPosition;
    #endregion

    #region Bools
    [Space(5)]
    protected bool CanTakeDamage = true;
    [SerializeField] protected bool ReduceKnockbackForce = true;
    public bool SeenPlayer = false;
    public bool AtEndOfPath = false;
    #endregion

    #region Scripts
    [HideInInspector] public BTNodeBase RootNode;
    [HideInInspector] public List<BTNodeBase> AllNodeChoices;

    //Patrol Scripts
    [HideInInspector] public AIDestinationSetter DestinationSetterScript;
    [HideInInspector] public Seeker AISeeker;
    protected Path PathRef;
    [HideInInspector]public SpireObject SpireLoaction;
    public WorldHandler WorldHandlerScript;

    [Space(10)]
    protected Rigidbody Rigidbody;

    #endregion

    public void BaseStartup()
    {
        CurrentHealth = MaxHealth;
        Rigidbody = GetComponent<Rigidbody>();
        EnemyRef = this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");

        int RandomSpire=Random.Range(0, WorldHandlerScript.SpirePoints.Count);
        SpireLoaction = WorldHandlerScript.SpirePoints[RandomSpire];

        AISeeker = GetComponent<Seeker>();
        DestinationSetterScript = GetComponent<AIDestinationSetter>();
        SetDestination(WaypointPosition);
        AISeeker.StartPath(Rigidbody.position, WaypointPosition.position, OnPathCompletion);

        InvokeRepeating("UpdatePath", 0f, 1f);

        CanTakeDamage = true;
    }

    public int HandleHealth(int HealthChange)
    {
        if ((CurrentHealth > 0 || CurrentHealth <= MaxHealth) && CanTakeDamage)
        {
            CurrentHealth -= HealthChange;
            StartCoroutine(ImmunityTimer());

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
        Rigidbody.AddForce(Vector3.forward * -1 * KnockbackPower, ForceMode.Impulse);
    }

    protected void HandleForce()
    {
        //Debug.Log(Rigidbody.velocity.x + " " + Mathf.Abs(Rigidbody.velocity.y) +" "+Mathf.Abs(Rigidbody.velocity.z));
        if (Mathf.Abs(Rigidbody.velocity.x) <= 2 && Mathf.Abs(Rigidbody.velocity.y) <= 2 && Mathf.Abs(Rigidbody.velocity.z) <= 2)
        {
            
            ReduceKnockbackForce = false;
        }
        else if(Mathf.Abs(Rigidbody.velocity.x) > 2 || Mathf.Abs(Rigidbody.velocity.y) > 2 || Mathf.Abs(Rigidbody.velocity.z) > 2)
        {
            ReduceKnockbackForce = true;
        }
        if(!ReduceKnockbackForce)
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

    #region Pathfinding and controlls: SetDest. OnPathComplete. UpdatePath. HandlePatrol. NewPatrolPoint. 
    public void SetDestination(Transform ObjectLocation)
    {
        DestinationSetterScript.target = ObjectLocation;
    }

    protected void OnPathCompletion(Path CompletedPath )
    {
        if (!CompletedPath.error)
        {
            Debug.Log("Chance");
            PathRef = CompletedPath;
            CurrentWayPoint = 0;

            NewPatrolPoint();
        }
    }

    protected void UpdatePath()
    {
        if (AISeeker.IsDone())
        {
            AISeeker.StartPath(Rigidbody.position, WaypointPosition.position, OnPathCompletion);
        }
    }

    public void HandlePatrol()
    {
        if (PathRef == null)
        {
            return;
        }
        if (CurrentWayPoint >= PathRef.vectorPath.Count)
        {
            AtEndOfPath = true;
            return;
        }
        else
        {
            AtEndOfPath = false;
        }


        float Distance = Vector3.Distance(Rigidbody.position, PathRef.vectorPath[CurrentWayPoint]);
        if (Distance < NextWaypointDistance)
        {
            CurrentWayPoint++;
        }
    }

    public void NewPatrolPoint()
    {
        int RandomSpire = Random.Range(0, SpireLoaction.NeighboringSpires.Count);
        SetDestination(SpireLoaction.NeighboringSpires[RandomSpire].WaypointSpot);
    }

    #endregion 



}
