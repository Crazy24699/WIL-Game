using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySight : MonoBehaviour
{

    protected EnemyBase EnemyAIScript;

    void Start()
    {
        EnemyAIScript = transform.GetComponentInParent<EnemyBase>();
    }

    private void OnTriggerEnter(Collider Trigger)
    {
        if (Trigger.CompareTag("Player"))
        {
            EnemyAIScript.SeenPlayer = true;
        }
    }

    private void OnTriggerExit(Collider Trigger)
    {
        if (Trigger.CompareTag("Player"))
        {
            EnemyAIScript.SeenPlayer = false;
        }
    }



}
