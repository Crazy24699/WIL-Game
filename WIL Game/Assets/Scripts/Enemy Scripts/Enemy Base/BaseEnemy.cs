using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{
    #region Ints
    protected int MaxHealth = 2;
    [SerializeField] protected int CurrentHealth;
    #endregion

    #region Floats
    [Space(5)]
    protected float ImmunityMaxTime;
    protected float CurrentImmunTime;
    [SerializeField] protected float KnockbackPower;

    [SerializeField] protected float KnockbackTimer = 4f;
    protected float KnockbackTime;
    //[SerializeField] protected float CurrentImmunityTime;
    [Space(3)]
    [SerializeField] protected float BaseMoveSpeed;

    [Space(8)]
    public float CurrentPlayerDistance;
    protected float ImmunityTime;
    protected float MaxFollowDistance;
    public float MaxAttackDistance;
    [HideInInspector]public float CurrentMoveSpeed;
    #endregion

    #region Gameobjects and transforms
    [Space(15)]
    protected GameObject EnemyObjectRef;
    protected GameObject PlayerRef;

    [Space(5)]
    public Transform PlayerTarget;
    #endregion

    [Header("Booleans"), Space(5)]
    #region Bools
    protected bool CanTakeDamage = true;
    public bool SeenPlayer = false;
    public bool PatrolActive;
    public bool PlayerEscaped;
    protected bool ReduceKnockbackForce;
    public bool OnAttackingList = false;

    [SerializeField]protected bool StartupRan = false;
    [SerializeField] protected bool Alive = false;

    #endregion

    #region Scripts
    public NavMeshAgent NavMeshRef;
    protected WorldHandler WorldHandlerScript;
    [SerializeField] protected Slider HealthBar;
    protected Rigidbody RigidbodyRef;
    [SerializeField] protected LayerMask PlayerLayer;
    #endregion
    
    public void BaseStartup()
    {

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
        if(HealthBar == null)
        {
            HealthBar = transform.GetComponentInChildren<Slider>();
        }

        CustomStartup();
        HealthStartup();

        StartupRan = true;
        Alive = true;
    }

    protected virtual void CustomStartup()
    {

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
        if (!StartupRan) { return; }
        StartCoroutine(ImmunityTimer());
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
        
    }

    public void RotateToTarget()
    {
        Vector3 TargetDirection = PlayerRef.transform.position - this.transform.position;
        TargetDirection.y = 0.0f;
        Quaternion TargetRotation = Quaternion.LookRotation(TargetDirection);

        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, TargetRotation, 35f * Time.deltaTime);
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


    protected IEnumerator ImmunityTimer()
    {
        CanTakeDamage = false;
        if (ImmunityTime <= 0)
        {
            Debug.LogError("ImmunityTimer not set on: " + this.gameObject.name);
            ImmunityTime = 2.55f;
        }
        yield return new WaitForSeconds(ImmunityTime);
        CanTakeDamage = true;
    }
}
