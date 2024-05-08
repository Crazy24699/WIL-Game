using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTPatrol : BTNodeBase
{

    /// <summary>           This will only happen if the AI Needs to be improved. 
    /// To Patrol the area the enemy will be in, The enemy will have 4 spires that is only visible within the editor
    /// and it will need to choose a random co ordindate on the x and z plane within those spires. It can not choose
    /// a coordinate that is within 5 meters of the last one, a distance will be drawn from the current cord and the new
    /// cord. Then random waypoints will be added to the path there decided by dividing the distance by a certain number 
    /// which will determind the number of alternate waypoints. And then from that the waypoints will be altered bu a few 
    /// unites either positively or negatively
    /// </summary>
    /// 
                    ///The way the AI will be done. 
    ///The way the current Ai will choose a location will be randomly choosing between 4 spires on the map
    ///The chosen spire will be removed as to not allow the enemy to possibly get the same one indefinetly
    ///spires will have a value that will determine the next one, it is up to random chance but the further 
    ///a spire is from the current one, the more likely it is to be chosen

    protected EnemyBase EnemyAIScript;

    public List<Transform> SpirePoints;

    protected bool ReachedEndOfPath = false;

    public BTPatrol(GameObject EnemyAIRef)
    {

    }

    public override NodeStateOptions RunLogicAndState()
    {


        return NodeStateOptions.Failed;
    }

}
