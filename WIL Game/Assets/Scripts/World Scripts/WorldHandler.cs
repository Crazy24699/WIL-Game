using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class WorldHandler : MonoBehaviour
{

    public Dictionary<int, List<SpireObject>> AllSpires = new Dictionary<int, List<SpireObject>>();

    public List<GameObject> AllEnemies = new List<GameObject>();
    public HashSet<GameObject> EnemiesAttacking = new HashSet<GameObject>();
    public List<GameObject> VisableEnemies = new List<GameObject>();

    public GameObject Boss1;
    public GameObject FinalBoss;
    public float CurrentTimescale;


    void Start()
    {
        //StartCoroutine(TempSetActiveEnemy());
        Debug.Log(AllSpires.Count);
        Debug.Log(AllSpires.ElementAt(0).Value.Count);
        
    }

    // Update is called once per frame
    void Update()
    {
        CurrentTimescale = Time.timeScale;
        VisableEnemies=EnemiesAttacking.ToList();
    }

    private IEnumerator TempSetActiveEnemy()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Rise");
        for (int i = 0; i < AllEnemies.Count; i++)
        {
            AllEnemies[i].gameObject.SetActive(false);
            if (i == 0)
            {
                AllEnemies[0].gameObject.SetActive(true);
            }
        }
    }

    public void SetNextActive(GameObject RemovedEnemy)
    {
        AllEnemies.Remove(RemovedEnemy.GetComponent<GameObject>());
        if(AllEnemies.Count > 0)
        {
            int Rnd = Random.Range(0, AllEnemies.Count);
            AllEnemies[Rnd].gameObject.SetActive(true);
        }

    }

    public void SetActiveArea(string ActiveArea)
    {
        if (ActiveArea == "Boss 1 Area")
        {
            Boss1.SetActive(true);
        }

        if(ActiveArea =="Final Boss Area")
        {
            FinalBoss.SetActive(true);
        }
    }

    public async void ReloadPathfindingGrid()
    {
        await PathGridReloadTimer();
    }

    public async Task PathGridReloadTimer ()
    {
        await Task.Delay(100);
    }

}
