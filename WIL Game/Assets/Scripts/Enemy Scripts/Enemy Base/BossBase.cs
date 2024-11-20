using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossBase : MonoBehaviour
{
    protected int MaxHealth;
    [SerializeField]protected int CurrentHealth;

    public GameObject PlayerRef;
    [SerializeField] private GameObject EndZoneArea;


    public NavMeshAgent NavMeshRef;
    [HideInInspector] public BTNodeBase RootNode;
    [HideInInspector] public List<BTNodeBase> AllNodeChoices;
    protected bool StartupRan;

    [SerializeField]protected Slider HealthBar;
    protected bool Alive = true;
    //[HideInInspector] public bool BossBeaten = false;

    public virtual void BossStartup()
    {

    }

    public void RotateToTarget()
    {
        if (PlayerRef == null) { return; }
        Vector3 TargetDirection = PlayerRef.transform.position - this.transform.position;
        TargetDirection.y = 0.0f;
        Quaternion TargetRotation = Quaternion.LookRotation(TargetDirection);

        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, TargetRotation, (35f + 180) * Time.deltaTime);
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

    public virtual void HandleHealth(int HealthChange)
    {
        int ChangedHealth = (CurrentHealth + HealthChange);
        Die(ChangedHealth);


        CurrentHealth += HealthChange;
        HealthBar.value = CurrentHealth;
    }

    protected virtual void Die(int HealthCheck)
    {
        if (HealthCheck <= 0)
        {
            CurrentHealth = 0;
            if (!EndZoneArea)
            {
                Debug.LogError("Endzone not set");
                return;
            }
            EndZoneArea.SetActive(true);
            //Play Death animation
            return;
        }


    }


}


