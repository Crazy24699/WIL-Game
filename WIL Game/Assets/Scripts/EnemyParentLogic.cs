using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyParentLogic : MonoBehaviour
{
    private HashSet<GameObject> FightAreaEnemies = new HashSet<GameObject>();
    [SerializeField] public GameObject EnemyGroup;

    public List<GameObject> Enemys = new List<GameObject>();

    private void Start()
    {
        if (EnemyGroup == null)
        {
            EnemyGroup = this.transform.parent.Find("Enemy Parent").gameObject;
        }
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
            Enemy.GetComponent<BaseEnemy>().BaseStartup();
        }
        Enemys = FightAreaEnemies.ToList();
    }

    public void GetAllEnemies()
    {
        BaseEnemy[] Enemies = EnemyGroup.GetComponentsInChildren<BaseEnemy>(true);
        foreach (var EnemyChild in Enemies)
        {
            FightAreaEnemies.Add(EnemyChild.gameObject);
        }
        Enemys = FightAreaEnemies.ToList();
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.gameObject.layer==LayerMask.NameToLayer("Player"))
        {
            SpawnInEnemies();
        }
    }

}
