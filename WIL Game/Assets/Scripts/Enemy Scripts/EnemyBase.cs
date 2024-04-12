using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected int MaxHealth = 100;
    [SerializeField]protected int CurrentHealth;

    protected float BaseMoveSpeed;
    protected float CurrentMoveSpeed;

    protected GameObject EnemyRef;
    protected GameObject PlayerRef;


    public void BaseStartup()
    {
        CurrentHealth = MaxHealth;

        EnemyRef =this.gameObject;
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
    }

    public int HandleHealth(int HealthChange)
    {
        if (CurrentHealth > 0 || CurrentHealth < MaxHealth)
        {
            CurrentHealth -= HealthChange;

            //Play health gained particle effect
            return CurrentHealth;
        }
        return 0;
    }

}
