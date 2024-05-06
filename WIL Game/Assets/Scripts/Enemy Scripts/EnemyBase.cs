using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected int MaxHealth = 100;
    [SerializeField] protected int CurrentHealth;

    [Space(5)]
    [SerializeField] protected float ImmunityMaxTime;
    //[SerializeField] protected float CurrentImmunityTime;
    [Space(3)]
    protected float BaseMoveSpeed;
    protected float CurrentMoveSpeed;

    [Space(5)]
    protected GameObject EnemyRef;
    protected GameObject PlayerRef;
    [Space(5)]
    protected bool CanTakeDamage = true;

    public void BaseStartup()
    {
        CurrentHealth = MaxHealth;

        EnemyRef =this.gameObject;
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

}
