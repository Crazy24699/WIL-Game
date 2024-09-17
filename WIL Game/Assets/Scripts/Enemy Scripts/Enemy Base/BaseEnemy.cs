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

    //[SerializeField] protected float CurrentImmunityTime;
    [Space(3)]
    [SerializeField] protected float BaseMoveSpeed;

    [Space(8)]
    protected float CurrentPlayerDistance;
    protected float MaxFollowDistance;
    protected float MaxAttackDistance;

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

    protected bool StartupRan = false;
    [SerializeField] protected bool Alive = false;

    #endregion

    #region Scripts
    public NavMeshAgent NavMeshRef;
    protected WorldHandler WorldHandlerScript;
    [SerializeField] protected Slider HealthBar;
    protected Rigidbody RigidbodyRef;

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
        CustomStartup();
        HealthStartup();

        StartupRan = true;
        Alive = true;
    }

    protected virtual void CustomStartup()
    {

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

        StartCoroutine(ImmunityTimer());
    }

    public virtual void HandleHealth(int ChangeValue)
    {
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

    protected IEnumerator ImmunityTimer()
    {
        CanTakeDamage = false;
        yield return new WaitForSeconds(2.55f);
        CanTakeDamage = true;
    }
}
