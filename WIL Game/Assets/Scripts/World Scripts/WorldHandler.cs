using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldHandler : MonoBehaviour
{

    public Dictionary<int, List<SpireObject>> AllSpires = new Dictionary<int, List<SpireObject>>();

    public List<EnemyBase> AllEnemies = new List<EnemyBase>();
    public List<EnemyBase> EnemiesAttacking = new List<EnemyBase>();
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
