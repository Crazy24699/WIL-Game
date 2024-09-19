using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBase : MonoBehaviour
{
    protected int MaxHealth;
    [SerializeField]protected int CurrentHealth;

    [HideInInspector]public GameObject PlayerRef;

    [HideInInspector] public BTNodeBase RootNode;
    [HideInInspector] public List<BTNodeBase> AllNodeChoices;
    protected bool StartupRan;

    protected bool Alive = true;
    [SerializeField]protected Slider HealthBar;

    public virtual void BossStartup()
    {

    }

    protected void HealtbarStartup()
    {
        if (HealthBar == null)
        {
            HealthBar = transform.GetComponentInChildren<Slider>();
            HealthBar.maxValue = MaxHealth;
            HealthBar.minValue = 0;
        }
        HealthBar.value = CurrentHealth;
    }

    public void HandleHealth(int HealthChange)
    {
        int ChangedHealth = (CurrentHealth + HealthChange);
        Die(ChangedHealth);


        CurrentHealth += HealthChange;
        HealthBar.value = CurrentHealth;
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


