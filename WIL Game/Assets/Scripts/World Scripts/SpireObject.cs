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
        WaypointSpot = transform.Find("Location");

        foreach (var Spire in SpireParentScript.SpireOptions)
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
            Debug.Log(TriggerObject.name);
            if(TriggerObject.GetComponent<BTBaseEnemy>() == null)
            {
                return;
            }

            BTBaseEnemy EnemyRef = TriggerObject.GetComponent<BTBaseEnemy>();
            if (EnemyRef.SeenPlayer)
            {
                return;
            }

            if (EnemyRef.SpireLoaction == ThisSpire) 
            {
                EnemyRef.NewPatrolPoint(ThisSpire);
            }
        }
    }
}
