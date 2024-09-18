using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParentLogic : MonoBehaviour
{
    private HashSet<GameObject> FightAreaEnemies = new HashSet<GameObject>();
    [SerializeField] public GameObject EnemyGroup;

    private void Start()
    {
        GetAllEnemies();
        foreach (var Enemy in FightAreaEnemies)
        {
            Enemy.SetActive(false);
        }
    }

    public void SpawnInEnemies()
    {
        foreach (var Enemy in FightAreaEnemies)
        {
            Enemy.SetActive(true);
        }
    }

    public void GetAllEnemies()
    {
        BaseEnemy[] Enemies = this.GetComponentsInChildren<BaseEnemy>(true);
        foreach (var EnemyChild in Enemies)
        {
            FightAreaEnemies.Add(EnemyChild.gameObject);
        }
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            SpawnInEnemies();
        }
    }

}
