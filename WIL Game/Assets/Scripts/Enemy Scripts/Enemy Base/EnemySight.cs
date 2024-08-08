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
        Debug.Log(Trigger.name+"        "+Trigger.tag);
        if (Trigger.CompareTag("Player"))
        {
            EnemyAIScript.SeenPlayer = true;
            EnemyAIScript.PlayerEscaped = false;
            EnemyAIScript.PatrolActive = false;
        }
    }

    private void OnTriggerExit(Collider Trigger)
    {
        if (Trigger.CompareTag("Player"))
        {
            //EnemyAIScript.SeenPlayer = false;
        }
    }



}
