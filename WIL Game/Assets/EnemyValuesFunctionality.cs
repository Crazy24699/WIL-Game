using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyValuesFunctionality : MonoBehaviour
{

    public int MaxHealth;
    public int CurrentHealth;


    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int HandleHealth(int HealthChange)
    {

        CurrentHealth += HealthChange;

        //Play health gained particle effect
        return CurrentHealth;
    }
}
