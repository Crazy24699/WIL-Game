using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTPatrol : BTNodeBase
{

    /// <summary>
    /// To Patrol the area the enemy will be in, The enemy will have 4 spires that is only visible within the editor
    /// and it will need to choose a random co ordindate on the x and z plane within those spires. It can not choose
    /// a coordinate that is within 5 meters of the last one, a distance will be drawn from the current cord and the new
    /// cord. Then random waypoints will be added to the path there decided by dividing the distance by a certain number 
    /// which will determind the number of alternate waypoints. And then from that the waypoints will be altered bu a few 
    /// unites either positively or negatively
    /// </summary>

    protected EnemyBase EnemyAIScript;

    protected HashSet<Transform> WaypointTransforms = new HashSet<Transform>();
    public List<Transform> SpirePoints;

    protected bool ReachedEndOfPath = false;

    public BTPatrol(GameObject EnemyAIRef)
    {

    }


    protected void GenerateNewPath()
    {

    }

    public override NodeStateOptions RunLogicAndState()
    {


        return NodeStateOptions.Failed;
    }

}
