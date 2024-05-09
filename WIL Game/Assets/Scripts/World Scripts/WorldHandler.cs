using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldHandler : MonoBehaviour
{

    public List<SpireObject> SpirePoints;
    

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
