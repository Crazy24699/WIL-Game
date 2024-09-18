using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpireParent : MonoBehaviour
{

    public int SpireParentLevel;

    public HashSet<SpireObject> SpireObjects = new HashSet<SpireObject>();

    // Start is called before the first frame update
    void Start()
    {
        PopulateSpires();
    }

    private void PopulateSpires()
    {
        foreach (var SpireObject in this.transform.GetComponentsInChildren<SpireObject>())
        {
            Debug.Log("run");
            SpireObjects.Add(SpireObject);
            SpireObject.LevelSpire = SpireParentLevel;
        }

        foreach (var SpireScript in SpireObjects)
        {
            SpireScript.Startup();
        }

        WorldHandler WorldHandler = FindObjectOfType<WorldHandler>();

        WorldHandler.AllSpires.Add(SpireParentLevel, SpireObjects.ToList());
        Debug.Log(WorldHandler.AllSpires.Count);
    }

    private void GetSpawningGroup()
    {
        EnemyParentLogic EnemyParentHandler = GetComponentInChildren<EnemyParentLogic>(true);
    }

}