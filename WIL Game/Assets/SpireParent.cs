using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpireParent : MonoBehaviour
{

    public int SpireParentLevel;

    public HashSet<SpireObject> SpireObjects = new HashSet<SpireObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (var SpireObject in this.transform.GetComponentsInChildren<SpireObject>())
        {
            Debug.Log("run");
            SpireObjects.Add(SpireObject);
        }
        foreach (var SpireScript in SpireObjects)
        {
            SpireScript.Startup();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
