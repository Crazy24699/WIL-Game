using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTPatrol : MonoBehaviour
{

    /// <summary>
    /// To Patrol the area the enemy will be in, The enemy will have 4 spires that is only visible within the editor
    /// and it will need to choose a random co ordindate on the x and z plane within those spires. It can not choose
    /// a coordinate that is within 5 meters of the last one, a distance will be drawn from the current cord and the new
    /// cord. Then random waypoints will be added to the path there decided by dividing the distance by a certain number 
    /// which will determind the number of alternate waypoints. And then from that the waypoints will be altered bu a few 
    /// unites either positively or negatively
    /// </summary>


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
