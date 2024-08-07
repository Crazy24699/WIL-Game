using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBasae : MonoBehaviour
{
    //#region Ints
    //protected int MaxHealth = 2;
    //[SerializeField] protected int CurrentHealth;
    //protected int CurrentWayPoint = 0;
    //#endregion

    //#region Floats
    //[Space(5)]
    //[SerializeField] protected float ImmunityMaxTime;
    //[SerializeField] protected float KnockbackPower;

    //[SerializeField] protected float KnockbackTimer = 4f;
    //protected float KnockbackTime;
    ////[SerializeField] protected float CurrentImmunityTime;
    //[Space(3)]
    //[SerializeField] protected float BaseMoveSpeed;
    //[SerializeField] protected float CurrentMoveSpeed;
    //public float NextWaypointDistance = 5f;

    //[Space(8)]
    //public float CurrentPlayerDistance;
    //public float OutOfFollowDistance;
    //public float OutOfAttackDistance;

    //public float OutOfRangeTimer;
    //public float CurrentRangeTime;
    //#endregion

    //#region Gameobjects and transforms
    //[Space(15)]
    //protected GameObject EnemyObjectRef;
    //protected GameObject PlayerRef;

    //[Space(5)]
    //[SerializeField] public Transform WaypointPosition;
    //[SerializeField] protected Transform WaypointParent;
    //[HideInInspector] public Transform PlayerTarget;
    //#endregion

    //#region Bools
    //[Space(5)]
    //protected bool CanTakeDamage = true;
    //[SerializeField] protected bool ReduceKnockbackForce = true;
    //public bool SeenPlayer = false;
    //public bool PlayerEscaped = false;
    //public bool AtEndOfPath = false;
    //protected bool StartupRan = false;
    //public bool Alive = true;
    //public bool PatrolActive = false;

    //public bool AttackPlayer;
    //public bool CanAttackPlayer;
    ////if the Ai is currently attacking the player, used for making sure it cant move or chain attacks
    //public bool IsAttacking;
    //#endregion

    ////Vectors
    //[SerializeField] protected Vector3 PositionLock;
    //protected Vector3 ViewLock;

    #region Scripts
    [HideInInspector] public BTNodeBase RootNode;
    [HideInInspector] public List<BTNodeBase> AllNodeChoices;

    //Patrol Scripts
    //[HideInInspector] public AIDestinationSetter DestinationSetterScript;
    //[HideInInspector] public Seeker AISeeker;

    //protected NavMeshAgent NavMeshRef;
    //public WorldHandler WorldHandlerScript;

    //[Space(10)]
    //protected Rigidbody Rigidbody;

    #endregion

    //public IEnumerator BaseStartup()
    //{
    //    //small delay to allow the world to start up
    //    yield return new WaitForSeconds(1);

    //    CurrentRangeTime = OutOfRangeTimer;

    //    CurrentHealth = MaxHealth;
    //    Rigidbody = GetComponent<Rigidbody>();
    //    EnemyObjectRef = this.gameObject;
    //    PlayerRef = GameObject.FindGameObjectWithTag("Player");
    //    WorldHandlerScript = FindObjectOfType<WorldHandler>();

    //    WaypointPosition = WaypointParent;

    //    if (NavMeshRef == null)
    //    {
    //        NavMeshRef = GetComponent<NavMeshAgent>();
    //    }
    //    CustomStartup();

    //    CanTakeDamage = true;
    //    StartupRan = true;
    //}


    //protected virtual void CustomStartup()
    //{

    //}

    //public int HandleHealth(int HealthChange)
    //{
    //    if ((CurrentHealth > 0 || CurrentHealth <= MaxHealth) && CanTakeDamage)
    //    {
    //        CurrentHealth -= HealthChange;
    //        StartCoroutine(ImmunityTimer());

    //        //Play health gained particle effect
    //        return CurrentHealth;
    //    }
    //    return 0;
    //}


    //#region Health and taking damage: Immunity Timer. Apply Knockback. Handle Force
    //public IEnumerator ImmunityTimer()
    //{
    //    CanTakeDamage = false;
    //    yield return new WaitForSeconds(ImmunityMaxTime);
    //    CanTakeDamage = true;
    //}

    //public void ApplyKnockback()
    //{
    //    Rigidbody.AddForce(Vector3.forward * -1 * KnockbackPower, ForceMode.Impulse);
    //    Debug.Log("Love gone wrong");
    //}

    //public void DistableAttack()
    //{
    //    Vector3 NewPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z) - (Vector3.forward * -2);
    //    transform.position = NewPosition;
    //    CanAttackPlayer = false;
    //    StartCoroutine(AttackCooldown(3.5f));
    //}

    //protected void HandleForce()
    //{
    //    //Debug.Log(Rigidbody.velocity.x + " " + Mathf.Abs(Rigidbody.velocity.y) +" "+Mathf.Abs(Rigidbody.velocity.z));
    //    if (Mathf.Abs(Rigidbody.velocity.x) <= 2 && Mathf.Abs(Rigidbody.velocity.y) <= 2 && Mathf.Abs(Rigidbody.velocity.z) <= 2)
    //    {

    //        ReduceKnockbackForce = false;
    //    }
    //    else if (Mathf.Abs(Rigidbody.velocity.x) > 2 || Mathf.Abs(Rigidbody.velocity.y) > 2 || Mathf.Abs(Rigidbody.velocity.z) > 2)
    //    {
    //        ReduceKnockbackForce = true;
    //    }
    //    if (!ReduceKnockbackForce)
    //    {
    //        return;
    //    }

    //    if (ReduceKnockbackForce)
    //    {
    //        KnockbackTime -= Time.deltaTime;
    //        float CurrentXVelocity = Mathf.Lerp(Rigidbody.velocity.x, 0.0f, 0.005f);
    //        float CurrentYVelocity = Mathf.Lerp(Rigidbody.velocity.y, 0.0f, KnockbackTime / KnockbackTimer);
    //        float CurrentZVelocity = Mathf.Lerp(Rigidbody.velocity.z, 0.0f, 0.005f);

    //        Rigidbody.velocity = new Vector3(CurrentXVelocity, CurrentYVelocity, CurrentZVelocity);
    //    }
    //}
    //#endregion
    //public void HandlePlayerRange()
    //{
    //    CurrentPlayerDistance = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);
    //    //Draw a raycast to the player, if it hits something before the player then the enemy can not see the player

    //    if (CurrentPlayerDistance >= OutOfFollowDistance)
    //    {
    //        CurrentRangeTime -= Time.deltaTime;
    //        if (CurrentRangeTime <= 0)
    //        {
    //            CurrentRangeTime = 0;
    //            SeenPlayer = false;
    //            PlayerEscaped = true;
    //        }
    //    }

    //    else if (CurrentPlayerDistance < OutOfFollowDistance)
    //    {
    //        CurrentRangeTime = OutOfRangeTimer;
    //    }

    //    if (PlayerEscaped)
    //    {
    //        CurrentRangeTime = OutOfRangeTimer;
    //    }
    //}

    //#region Pathfinding and controlls: SetDest. OnPathComplete. UpdatePath. HandlePatrol. NewPatrolPoint. 
    //public void SetDestination(Transform ObjectLocation)
    //{
    //    NavMeshRef.SetDestination(ObjectLocation.transform.position);
    //}


    //#endregion

    //public virtual void Attack()
    //{

    //}


    //protected IEnumerator AttackCooldown(float AttackCooldown)
    //{
    //    yield return new WaitForSeconds(AttackCooldown);
    //    CanAttackPlayer = true;
    //}

    //public IEnumerator TempAttackCooldownLock()
    //{
    //    yield return new WaitForSeconds(1f);
    //    IsAttacking = false;
    //}

    //protected void LockForAttack()
    //{
    //    ViewLock = transform.rotation.eulerAngles;
    //    PositionLock = transform.position;
    //    IsAttacking = true;
    //}

    //public void EnforceLock()
    //{
    //    EnemyObjectRef.transform.position = PositionLock;
    //    EnemyObjectRef.transform.rotation = Quaternion.Euler(ViewLock);
    //}



    //public virtual void OnTriggerEnter(Collider Collision)
    //{
    //    if (Collision.CompareTag("Player"))
    //    {
    //        SeenPlayer = true;
    //        PlayerTarget = Collision.gameObject.transform;
    //    }



    //}

}
