using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour
{
    protected int MaxHealth;
    [SerializeField]protected int CurrentHealth;

    public GameObject PlayerRef;

    [HideInInspector] public BTNodeBase RootNode;
    [HideInInspector] public List<BTNodeBase> AllNodeChoices;

    protected bool Alive = true;
    

    public virtual void BossStartup()
    {

    }

    public void HandleHealth(int HealthChange)
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


