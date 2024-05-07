using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected int MaxHealth = 100;
    [SerializeField] protected int CurrentHealth;

    [Space(5)]
    [SerializeField] protected float ImmunityMaxTime;
    [SerializeField] protected float KnockbackPower;

    [SerializeField]protected float KnockbackTimer = 4f;
    protected float KnockbackTime;
    //[SerializeField] protected float CurrentImmunityTime;
    [Space(3)]
    protected float BaseMoveSpeed;
    protected float CurrentMoveSpeed;

    [Space(5)]
    protected GameObject EnemyRef;
    protected GameObject PlayerRef;

    [Space(10)]
    protected Rigidbody Rigidbody;

    [Space(5)]
    protected bool CanTakeDamage = true;
    [SerializeField]protected bool ReduceKnockbackForce = true;


    [HideInInspector] public BTNodeBase RootNode;
    [HideInInspector] public List<BTNodeBase> AllNodeChoices;


    public void BaseStartup()
    {
        CurrentHealth = MaxHealth;
        Rigidbody = GetComponent<Rigidbody>();
        EnemyRef = this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");

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
        Debug.Log(Rigidbody.velocity.x + " " + Mathf.Abs(Rigidbody.velocity.y) +" "+Mathf.Abs(Rigidbody.velocity.z));
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

}
