using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpireParent : MonoBehaviour
{

    public int SpireParentLevel;

    public HashSet<SpireObject> SpireOptions = new HashSet<SpireObject>();
    public List<BaseEnemy> EnemyChildrem = new List<BaseEnemy>();

    // Start is called before the first frame update
    void Start()
    {
        GetAllEnemies();
        PopulateSpires();
    }

    private void PopulateSpires()
    {
        foreach (var SpireObject in this.transform.GetComponentsInChildren<SpireObject>())
        {
            Debug.Log("run");
            SpireOptions.Add(SpireObject);
            SpireObject.LevelSpire = SpireParentLevel;
        }

        foreach (var SpireScript in SpireOptions)
        {
            SpireScript.Startup();
        }

        WorldHandler WorldHandler = FindObjectOfType<WorldHandler>();

        WorldHandler.AllSpires.Add(SpireParentLevel, SpireOptions.ToList());
        Debug.Log(WorldHandler.AllSpires.Count+"        "+this.name);
    }

    private void GetAllEnemies()
    {
        HashSet<BaseEnemy> Enemies = new HashSet<BaseEnemy>();
        BaseEnemy[] AllEnemies = transform.GetComponentsInChildren<BaseEnemy>(true);
        Enemies = AllEnemies.ToHashSet();
        EnemyChildrem = Enemies.ToList();
        foreach (var Enemy in EnemyChildrem)
        {
            Enemy.GetComponent<BTBaseEnemy>().SpireParentScript = this;
        }
    }

    private void GetSpawningGroup()
    {
        EnemyParentLogic EnemyParentHandler = GetComponentInChildren<EnemyParentLogic>(true);
    }

}