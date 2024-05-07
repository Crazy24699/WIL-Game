using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpewerAi : EnemyBase
{
    
    private void Start()
    {
        BaseStartup();
    }

    // Update is called once per frame
    void Update()
    {
        HandleForce();
    }

    public void CreateBehaviourTree()
    {

    }

}
