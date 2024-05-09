using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpireObject : MonoBehaviour
{
    //The current level that the spire is a child of
    public int LevelSpire;

    public SpireParent SpireParentScript;

    public List<SpireObject> NeighboringSpires;

    public Transform WaypointSpot;
    public SpireObject ThisSpire;

    public void Startup()
    {
        ThisSpire = GetComponent<SpireObject>();
        SpireParentScript = transform.GetComponentInParent<SpireParent>();

        foreach (var Spire in SpireParentScript.SpireObjects)
        {
            if (!Spire.name.Equals(ThisSpire.name))
            {
                NeighboringSpires.Add(Spire);
            }
        }
    }

    private void OnTriggerEnter(Collider TriggerObject)
    {
        if (TriggerObject.CompareTag("Enemy"))
        {
            TriggerObject.GetComponent<EnemyBase>().SpireLoaction = ThisSpire;
        }
    }
}
