using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour
{
    [SerializeField] protected int MaxHealth;
    protected int CurrentHealth;

    public GameObject PlayerRef;

    [HideInInspector] public BTNodeBase RootNode;
    [HideInInspector] public List<BTNodeBase> AllNodeChoices;


    

    public virtual void BossStartup()
    {

    }

    protected void HandleHealth(int HealthChange)
    {
        int ChangedHealth = (CurrentHealth + HealthChange);
        Die(ChangedHealth);

        CurrentHealth += HealthChange;
    }

    protected void Die(int HealthCheck)
    {
        if (HealthCheck <= 0)
        {
            CurrentHealth = 0;
            //Play Death animation
            return;
        }


    }


}


