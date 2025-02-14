using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    private GameObject ChosenEnemyPrefab;
    private Transform SpawnPoint;

    private bool SpawnActive = false;

    public GameObject AliveEnemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnEnemy()
    {
        GameObject EnemyCreated = null;

        EnemyCreated = Instantiate(ChosenEnemyPrefab, SpawnPoint.position, Quaternion.identity);
    }

    public void HandleSpawningLogic()
    {
        SpawnEnemy();
    }

    private void CheckSpawnPossible()
    {

    }

    private void OnTriggerStay(Collider Collision)
    {
        if (SpawnActive) { return; }

        if (Collision.CompareTag("Player") && Collision.GetComponent<PlayerInteraction>())
        {
            SpawnActive = true;
        }

    }
}
