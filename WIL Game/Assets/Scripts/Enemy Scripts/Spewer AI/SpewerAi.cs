using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpewerAi : EnemyBase
{

    public bool SearchSequenceActive = false;
    public bool SpeedBoost;

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
