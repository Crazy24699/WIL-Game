using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEvent : MonoBehaviour
{
    [SerializeField] private GameObject DamageObject;
    private GameObject PlayerRef;

    private void OnTriggerEnter(Collider Collision)
    {
        if (!Collision.CompareTag("Player")) { return; }
        if(Collision.GetComponent<PlayerInteraction>() != null)
        {
            PlayerRef = Collision.gameObject;
            DamageObject.SetActive(true);
            DamageObject.GetComponent<TutorialProjectile>().PlayerRef= PlayerRef;
            DamageObject.GetComponent<TutorialProjectile>().StartupRan = true;
        }
        
    }

}
