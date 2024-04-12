using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFunctionality : MonoBehaviour
{

    public int CurrentHealth;
    const int MaxHealth=6;
    
    private float ImmunityTimer;

    protected bool CanTakeDamage = true;
    protected bool CanHeal = true;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(TakeDamage(1));  
        }
    }


    public int Heal(int HealValue)
    {
        return CurrentHealth += HealValue;
    }

    public IEnumerator TakeDamage(int Damage)
    {
        if (CanTakeDamage)
        {
            CurrentHealth -= Damage;
            CanTakeDamage = false;
            yield return new WaitForSeconds(1);
            CanTakeDamage = true;
        }
        
    }

}
