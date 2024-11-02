using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebProjectile : ProjectileBase
{

    [SerializeField] private GameObject Webbing;

    private bool IsProjectile = true;
    private bool StartupRan = false;

    public void Startup()
    {


        StartupRan = true;
    } 

    // Update is called once per frame
    void Update()
    {
        if (!StartupRan) { return; }
        
    }


    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            if(IsProjectile)
            {
                Collision.transform.root.GetComponent<PlayerInteraction>().HandleHealth(-Damage);
                Destroy(this.gameObject);
                return;
            }

        }

        if(Collision.CompareTag("Ground") && IsProjectile)
        {
            IsProjectile = false;
        }
    }

    class AttackPhases
    {
        //public 
    }

}
