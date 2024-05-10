using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpewerAi : EnemyBase
{

    public bool SearchSequenceActive = false;
    public bool SpeedBoost;

    private void Start()
    {
        StartCoroutine(BaseStartup());
    }

    // Update is called once per frame
    void Update()
    {
        if (!StartupRan)
        {
            return;
        }
        HandleForce();
    }

    public void CreateBehaviourTree()
    {

    }

    

}
