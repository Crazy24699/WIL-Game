using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{
    #region Ints
    protected int MaxHealth = 2;
    protected int CurrentHealth;
    #endregion

    #region Floats
    [Space(5)]
    [SerializeField] protected float KnockbackPower;

    protected float KnockbackTimer = 4f;
    protected float KnockbackTime;
    //[SerializeField] protected float CurrentImmunityTime;
    [Space(3)]
    [SerializeField] protected float BaseMoveSpeed;
    [SerializeField] protected float ExtraRotSpeed;


    [Space(8)]
 public float CurrentPlayerDistance;
    protected float ImmunityTime;

    [SerializeField]protected float MaxFollowDistance;
    public float MaxAttackDistance;
    [HideInInspector]public float CurrentMoveSpeed;

    protected float CloseRangeSpeedMulti;
    public float PathfindAwayDistance;

    [HideInInspector] public float OrbitingDistance;
    #endregion

    #region Gameobjects and transforms
    [Space(15)]
    [HideInInspector] protected GameObject EnemyObjectRef;
    protected GameObject PlayerRef;
    [SerializeField]protected GameObject PolutionShardPrefab;

    [Space(5)]
    [HideInInspector] public Transform PlayerTarget;
    public Transform CurrentTarget;
    #endregion

    [HideInInspector] protected Vector3 PlayerDirection;
    [SerializeField]protected Vector3 RetreatPosition;
    [HideInInspector] protected Vector3 CurrentPosition;

    [Header("Booleans"), Space(5)]
    #region Bools
    [HideInInspector] protected bool CanTakeDamage = true;
    public bool SeenPlayer = false;
    public bool PatrolActive;
    public bool PlayerEscaped;
    [SerializeField] protected bool TutorialOverride;

    protected bool ReduceKnockbackForce;
    [HideInInspector] public bool OnAttackingList = false;

    protected bool CanAttack = true;
    protected bool Orbiting;

    protected bool UpdateRetretPosition;
    protected bool AdvancedRetreat = false;

    [SerializeField]protected bool StartupRan = false;
    [SerializeField] protected bool Alive = false;
    protected bool SoundPlaying = false;
    public bool Override = false;
    public bool PatrolOverrdide = false; [Header("Booleans"), Space(5)]
    public bool AttackAreaOverride = false;
    [SerializeField]protected bool TakingDamage = false;

    [HideInInspector] public bool IsAttacking;
    public bool OutOfAttackRange;
    [HideInInspector] public bool CanAttackPlayer;

    #endregion

    #region Scripts
    [HideInInspector] public NavMeshAgent NavMeshRef;
    protected WorldHandler WorldHandlerScript;
    [SerializeField] protected Slider HealthBar;
    protected Rigidbody RigidbodyRef;
    [SerializeField] protected LayerMask PlayerLayer;
    [SerializeField]private EnemyVision VisionLinkScript;
    protected EnemySoundManager EnemyAudioManager;
    #endregion
    
    public void BaseStartup()
    {
        EnemyAudioManager = GetComponent<EnemySoundManager>();
        if (EnemyAudioManager == null)
        {
            Debug.Log("There is no audi manager on enemy: " + this.gameObject.name);
        }

        CanAttack = true;
        RigidbodyRef = GetComponent<Rigidbody>();
        EnemyObjectRef = this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerTarget = PlayerRef.transform;

        WorldHandlerScript = FindObjectOfType<WorldHandler>();

        if (NavMeshRef == null)
        {
            NavMeshRef = GetComponent<NavMeshAgent>();
        }

        HealthBar = GetComponent<Slider>();
        if (HealthBar == null)
        {
            HealthBar = transform.GetComponentInChildren<Slider>();
        }

        CustomStartup();
        HealthStartup();

        BaseMoveSpeed = NavMeshRef.speed;

        StartupRan = true;
        Alive = true;
        if (GetComponent<EnemyVision>() != null)
        {
            VisionLinkScript = this.GetComponent<EnemyVision>();
            VisionLinkScript.Startup(this);
        }
    }

    protected virtual void CustomStartup()
    {

    }
    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.15f);

        EnemyAudioManager = GetComponent<EnemySoundManager>();
        if (EnemyAudioManager == null)
        {
            Debug.Log("There is no audi manager on enemy: " + this.gameObject.name);
        }

        CanAttack = true;
        RigidbodyRef = GetComponent<Rigidbody>();
        EnemyObjectRef = this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        PlayerTarget = PlayerRef.transform;

        WorldHandlerScript = FindObjectOfType<WorldHandler>();

        if (NavMeshRef == null)
        {
            NavMeshRef = GetComponent<NavMeshAgent>();
        }

        HealthBar = GetComponent<Slider>();
        if (HealthBar == null)
        {
            HealthBar = transform.GetComponentInChildren<Slider>();
        }

        CustomStartup();
        HealthStartup();

        BaseMoveSpeed = NavMeshRef.speed;

        StartupRan = true;
        Alive = true;
        if (GetComponent<EnemyVision>() != null)
        {
            VisionLinkScript = this.GetComponent<EnemyVision>();
            VisionLinkScript.Startup(this);
        }
    }


    private void LateUpdate()
    {
        if(Override && !StartupRan)
        {
            BaseStartup();
            Debug.Log("devils game");
        }

    }

    protected void CheckSoundPlayState()
    {
        SoundPlaying = EnemyAudioManager.SoundPlaying;
    }

    public virtual void ApplyKnockback()
    {
        Vector3 ForceDirection = (PlayerRef.transform.position - transform.position).normalized;
        RigidbodyRef.AddForce(ForceDirection * -1 * KnockbackPower, ForceMode.Impulse);
        Debug.Log("Love gone wrong");
    }

    protected virtual void HealthStartup()
    {
        if (MaxHealth <= 0)
        {
            MaxHealth = 12;
            Debug.LogError("Maxhealth not set for: " + this.gameObject.name);
        }
        HealthBar = transform.GetComponentInChildren<Slider>();
        CurrentHealth = MaxHealth;

        HealthBar.maxValue = MaxHealth;
        HealthBar.value = CurrentHealth;

    }

    protected virtual void TakeHit()
    {
        //Damage effect
        //Damage sound effect
        TakingDamage = true;
        EnemyAudioManager.PlaySound(EnemySoundManager.SoundOptions.TakeDamage);
        if (!StartupRan || TutorialOverride) { return; }
        StartCoroutine(ImmunityTimer());
        ApplyKnockback();
        Debug.Log("without a passion");
    }


    public virtual void HandleHealth(int ChangeValue)
    {
        if (!StartupRan) { return; }
        HealthBar.value = CurrentHealth;
        if ((CurrentHealth + ChangeValue) > MaxHealth)
        {
            return;
        }
        if (ChangeValue < 0)
        {
            TakeHit();
        }
        CurrentHealth += ChangeValue;

        if (CurrentHealth <= 0)
        {
            Death();
        }

        HealthBar.value = CurrentHealth;
    }
    
    protected virtual void Death()
    {
        Alive = false;
        Instantiate(PolutionShardPrefab, transform.position, Quaternion.identity);
    }

    public void RotateToTarget()
    {
        if(CurrentTarget == null) { return; }
        Vector3 TargetDirection = CurrentTarget.transform.position - this.transform.position;
        TargetDirection.y = 0.0f;
        Quaternion TargetRotation = Quaternion.LookRotation(TargetDirection);

        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, TargetRotation, (35f + ExtraRotSpeed) * Time.deltaTime);
    }

    protected virtual void HandleForce()
    {
        //Debug.Log(Rigidbody.velocity.x + " " + Mathf.Abs(Rigidbody.velocity.y) +" "+Mathf.Abs(Rigidbody.velocity.z));
        if (Mathf.Abs(RigidbodyRef.velocity.x) <= 2 && Mathf.Abs(RigidbodyRef.velocity.y) <= 2 && Mathf.Abs(RigidbodyRef.velocity.z) <= 2)
        {

            ReduceKnockbackForce = false;
        }
        else if (Mathf.Abs(RigidbodyRef.velocity.x) > 2 || Mathf.Abs(RigidbodyRef.velocity.y) > 2 || Mathf.Abs(RigidbodyRef.velocity.z) > 2)
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
            float CurrentXVelocity = Mathf.Lerp(RigidbodyRef.velocity.x, 0.0f, 0.005f);
            float CurrentYVelocity = Mathf.Lerp(RigidbodyRef.velocity.y, 0.0f, KnockbackTime / KnockbackTimer);
            float CurrentZVelocity = Mathf.Lerp(RigidbodyRef.velocity.z, 0.0f, 0.005f);

            RigidbodyRef.velocity = new Vector3(CurrentXVelocity, CurrentYVelocity, CurrentZVelocity);
        }
    }


    public virtual void Attack()
    {

    }

    protected void KeepOrbitDistance()
    {
        CurrentPlayerDistance = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);
        PlayerDirection = (this.transform.position - PlayerTarget.transform.position).normalized;
        //PlayerDirection = PlayerDirection.RoundVector(0);

        if (CurrentPlayerDistance <= PathfindAwayDistance && UpdateRetretPosition)
        {
            Debug.Log("Fuc Dunce");
            PathfindingRetreat();
            StartCoroutine(RetreatCooldown());

            return;
        }
        if (AdvancedRetreat)
        {
            if (CurrentPosition == RetreatPosition)
            {
                if (RigidbodyRef.velocity != Vector3.zero)
                {
                    RigidbodyRef.velocity = Vector3.zero;
                }

                AdvancedRetreat = false;
            }
            return;
        }

        if (CurrentPlayerDistance < OrbitingDistance)
        {
            Debug.Log(":the whispers");
            NavMeshRef.isStopped = false;
            RigidbodyRef.velocity = new Vector3(PlayerDirection.x, 0, PlayerDirection.z) * 20;
            if (CurrentPlayerDistance < MaxFollowDistance - 10)
            {
                UpdateRetretPosition = true;
                CloseRangeSpeedMulti = 2.5f;
                NavMeshRef.speed = BaseMoveSpeed * CloseRangeSpeedMulti;
                return;
            }

            CloseRangeSpeedMulti = 1f;
            UpdateRetretPosition = false;
            NavMeshRef.speed = BaseMoveSpeed * CloseRangeSpeedMulti;
        }
        else
        {
            Debug.Log("thriller");
            NavMeshRef.isStopped = true;
            if (RigidbodyRef.velocity != Vector3.zero)
            {

                RigidbodyRef.velocity = Vector3.zero;
            }
        }
    }

    protected void PathfindingRetreat()
    {
        CurrentPosition = transform.position.RoundVector(2);
        RetreatPosition = (PlayerDirection * 20) + CurrentPosition;
        RetreatPosition = RetreatPosition.RoundVector(2);

        NavMeshRef.SetDestination(RetreatPosition);
        
        AdvancedRetreat = true;
    }

    protected IEnumerator RetreatCooldown()
    {
        UpdateRetretPosition = false;
        yield return new WaitForSeconds(1.75f);
        UpdateRetretPosition = true;
    }

    protected IEnumerator AttackCooldown(float CooldownTime)
    {
        yield return new WaitForSeconds(CooldownTime);
        CanAttackPlayer = true;
    } 

    protected IEnumerator ImmunityTimer()
    {
        CanTakeDamage = false;
        //TakingDamage = false;
        if (ImmunityTime <= 0)
        {
            Debug.LogError("ImmunityTimer not set on: " + this.gameObject.name);
            ImmunityTime = 3.55f;
        }
        yield return new WaitForSeconds(ImmunityTime);
        CanTakeDamage = true;

    }



}
