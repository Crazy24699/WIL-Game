using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{

    [SerializeField] private GameObject BossToSpawn;

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            BossToSpawn.SetActive(true);

        }
    }
}
